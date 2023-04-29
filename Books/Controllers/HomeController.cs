using Books.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Books.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<ActionResult> Index()
        {
            DateTime start = DateTime.Today.AddDays(-6);
            DateTime end = DateTime.Today;

            var books = await _context.Book.ToListAsync();
            var orders = await _context.Orders.ToListAsync();

            List<Order> weeklyorder = orders.Where(o => o.Bought >= start && o.Bought <= end).ToList();

            //Total Orders
            int totalOrders = orders.Count;
            ViewBag.TotalOrders = totalOrders;

            //Total Income
            var set = orders.Join(books, o => o.BookId, b => b.Id, (o, b) => new { o.CustomerId, o.BookId, b.Price, b.Genre });
            var sum = set.Sum(o => o.Price);
            ViewBag.Sum = sum.ToString("C0");

            //Doughnut Chart
            ViewBag.Doughnut = books.GroupBy(i => i.Genre).Select(i => new
            {
                category = i.First().Genre,
                amount = i.Count(),
                formattedamount = ((double)i.Count() / books.Count) > 0 ? ((double)i.Count() / books.Count).ToString("P") : null
            }).OrderByDescending(x => x.amount);

            //Spline Chart
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
            return View();
        }
    }
    class SplineChartData
    {
        public string Day = null!;
        public int Orders;
    }
}