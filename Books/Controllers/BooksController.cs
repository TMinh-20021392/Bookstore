using Books.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Books.Controllers
{
    public class BooksController : Controller
    {
        private readonly ApplicationDbContext _context;
        // GET: Demo/GetImageFromByteArray
        public string GetImageFromByteArray(byte[] byteData)
        {
            //Convert byte arry to base64string
            string imreBase64Data = Convert.ToBase64String(byteData);
            return string.Format("data:image/png;base64,{0}", imreBase64Data);
        }
        public byte[] ImgToByteArray(IFormFile img)
        {
            using (MemoryStream mStream = new())
            {
                img.CopyTo(mStream);
                return mStream.ToArray();
            }
        }
        public BooksController(ApplicationDbContext context)
        {
            _context = context;
        }
        private static List<SelectListItem> GetPageSizes(int selectedPageSize = 5)
        {
            var pagesSizes = new List<SelectListItem>();

            if (selectedPageSize == 5)
                pagesSizes.Add(new SelectListItem("5", "5", true));
            else
                pagesSizes.Add(new SelectListItem("5", "5"));

            for (int lp = 10; lp <= 100; lp += 10)
            {
                if (lp == selectedPageSize)
                { pagesSizes.Add(new SelectListItem(lp.ToString(), lp.ToString(), true)); }
                else
                    pagesSizes.Add(new SelectListItem(lp.ToString(), lp.ToString()));
            }

            return pagesSizes;
        }
        public IActionResult Index(string SearchText = "", int pg = 1, int pageSize = 5)
        {
            List<Book> Book;
            List<Book> b;

            if (pg < 1) pg = 1;
            int recsCount = _context.Book.Count();

            int recSkip = (pg - 1) * pageSize;


            if (SearchText != "" && SearchText != null)
            {
                b = _context.Book.Where(p => p.Name.Contains(SearchText)).Skip(recSkip).Take(pageSize).ToList();
            }
            else
            {
                b = _context.Book.Skip(recSkip).Take(pageSize).ToList();
            }


            Pager SearchPager = new(recsCount, pg, pageSize) { Action = "Index", Controller = "Books", SearchText = SearchText };
            ViewBag.SearchPager = SearchPager;

            ViewBag.PageSizes = GetPageSizes(pageSize);
            return View(b.ToList());

        }

        // GET: Books/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Book == null)
            {
                return NotFound();
            }

            var book = await _context.Book
                .FirstOrDefaultAsync(m => m.Id == id);
            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }

        // GET: Books/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Books/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Author,Price,Genre")] Book book)
        {
            IFormFile theFile = HttpContext.Request.Form.Files[0];

            if (ModelState.IsValid)
            {
                if (theFile.Length > 0)
                {
                    book.Image = ImgToByteArray(theFile);
                }
                else
                {
                    return View(book);
                }
                _context.Add(book);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(book);
        }

        // GET: Books/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Book == null)
            {
                return NotFound();
            }

            var book = await _context.Book.FindAsync(id);
            if (book == null)
            {
                return NotFound();
            }
            return View(book);
        }

        // POST: Books/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([Bind("Id,Name,Author,Price,Genre,Image")] Book book)
        {
            IFormFile theFile = HttpContext.Request.Form.Files[0];
            if (theFile.Length > 0)
            {
                book.Image = ImgToByteArray(theFile);
            }
            else
            {
                return View();
            }
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(book);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BookExists(book.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(book);
        }

        // GET: Books/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Book == null)
            {
                return NotFound();
            }

            var book = await _context.Book
                .FirstOrDefaultAsync(m => m.Id == id);
            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }

        // POST: Books/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Book == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Book'  is null.");
            }
            var book = await _context.Book.FindAsync(id);
            if (book != null)
            {
                _context.Book.Remove(book);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BookExists(int id)
        {
            return (_context.Book?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
