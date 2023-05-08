using Books.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using static System.Reflection.Metadata.BlobBuilder;

namespace Books.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<ActionResult> Weekly()
        {
            Stopwatch watch = new Stopwatch();
            DateTime start = DateTime.Today.AddDays(-6);
            DateTime end = DateTime.Today;
            var orders = await _context.Orders.ToListAsync();
            watch.Start();
            //Spline Chart
            List<Order> weeklyorder = orders.Where(o => o.Bought >= start && o.Bought <= end).ToList();
            List<SplineChartData> splineorder = weeklyorder.GroupBy(x => x.Bought).Select(k => new SplineChartData()
            {
                Day = k.First().Bought.ToString("dd-MMM"),
                Orders = k.Count(),
            }).ToList();
            string[] last7days = Enumerable.Range(0, 7).Select(i => start.AddDays(i).ToString("dd-MMM")).ToArray();
            ViewBag.SplineChartData = from day in last7days
                                      join o in splineorder on day equals o.Day into DayOrder
                                      from o in DayOrder.DefaultIfEmpty()
                                      select new
                                      {
                                          Day = day,
                                          Orders = o == null ? 0 : o.Orders
                                      };
            watch.Stop();
            ViewBag.SplineChartDataTime = watch.Elapsed.TotalMilliseconds + "ms";
            return View();
        }
        public async Task<ActionResult> Genre()
        {
            Stopwatch watch = new Stopwatch();
            _context.Database.SetCommandTimeout(120);
            watch.Start();
            //Doughnut Chart
            int count = _context.Book.Count();
            ViewBag.Doughnut = _context.Book.GroupBy(x => x.Genre).Select(a => new
            {
                category = a.Key,
                amount = a.Count(),
                formattedamount = ((double)a.Count() / count) > 0 ? ((double)a.Count() / count).ToString("P") : null
            }).OrderByDescending(a => a.amount);
            watch.Stop();
            ViewBag.DoughnutTime = watch.Elapsed.TotalMilliseconds + "ms";
            return View();
        }
        public ActionResult Recent()
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();
            //Recent orders and customers
            ViewBag.Recent = _context.Orders.OrderByDescending(x => x.Bought).Take(5)
                .Join(_context.Book, o => o.BookId, b => b.Id, (o, b) => new { o.CustomerId, o.BookId, b.Name, o.Bought })
                .Join(_context.Customers, o => o.CustomerId, c => c.Id, (o, c) => new { BName = o.Name, CName = c.Name, Date = o.Bought }).ToList();
            watch.Stop();
            ViewBag.RecentTime = watch.Elapsed.TotalMilliseconds + "ms";
            return View();
        }
        public ActionResult Top()
        {
            Stopwatch watch = new Stopwatch();
            _context.Database.SetCommandTimeout(120);
            watch.Start();
            //Top 5 books
            ViewBag.TopBook = _context.Orders.GroupBy(x => x.BookId).Select(x => new
            {
                BookId = x.Key,
                count = x.Count(),
            }).OrderByDescending(x=>x.count).Take(5)
            .Join(_context.Book, o => o.BookId, b => b.Id,
                (o, b) => new
                {
                    b.Id,
                    b.Name,
                    b.Author,
                    b.Genre
                });
            watch.Stop();
            ViewBag.TopBookTime = watch.Elapsed.TotalMilliseconds + "ms";
            return View();
        }
        public ActionResult Index()
        {
            Stopwatch watch = new Stopwatch();

            watch.Start();
            //Total Orders
            int totalOrders = _context.Orders.Count();
            ViewBag.TotalOrders = totalOrders;
            watch.Stop();
            ViewBag.TotalOrdersTime = watch.Elapsed.TotalMilliseconds + "ms";

            watch.Restart();
            //Total Income
            var set = _context.Orders.Join(_context.Book, o => o.BookId, b => b.Id, (o, b) => new { o.CustomerId, o.BookId, b.Price, b.Genre });
            var sum = set.Sum(o => o.Price);
            ViewBag.Sum = sum.ToString("C0");
            watch.Stop();
            ViewBag.SumTime = watch.Elapsed.TotalMilliseconds + "ms";

            List<double> SQL = new List<double>() { 12, 13, 23, 13, 13 };
            List<double> COUCH = new List<double> { 52, 56, 129, 53, 36 };
            List<int> query = new List<int> { 0, 1, 2, 3, 4 };
            ViewBag.Hieunang = from q in query
                               join couch in COUCH on q equals COUCH.IndexOf(couch) into Tray
                               join sql in SQL on q equals SQL.IndexOf(sql) into Exe
                               select new
                               {
                                   q,
                                   val1 = COUCH[q],
                                   val2 = SQL[q]
                               };
            return View();
        }
    }
    class SplineChartData
    {
        public string Day = null!;
        public int Orders;
    }
    public static class Helper
    {
        public static List<T> RawSqlQuery<T>(ApplicationDbContext context, string query, Func<DbDataReader, T> map)
        {
            using (var command = context.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = query;
                command.CommandType = CommandType.Text;

                context.Database.OpenConnection();

                using (var result = command.ExecuteReader())
                {
                    var entities = new List<T>();

                    while (result.Read())
                    {
                        entities.Add(map(result));
                    }

                    return entities;
                }
            }
        }
    }
    class PieChartData
    {
        public string category;
        public double amount;
        public string formattedamount;
    }
}