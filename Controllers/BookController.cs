#nullable disable
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Web.Data;
using Web.Models.BO;

namespace Web.Controllers
{
    public class BookController : BaseController
    {
        private readonly ApplicationDbContext _context;

        public BookController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
        }

        // GET: Livres
        public async Task<IActionResult> Index()
        {
            var user = GetUserId();
            var role = GetRole();
            IList<Book> books;
            if (role == "membre")
            {
                books = await _context.Books.Where(x => x.Status == BookStatus.Available || x.UserId == user).ToListAsync();
            }
            else
            {
                books = await _context.Books.ToListAsync();
            }
            return View(books);
        }

        // GET: Livres/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var role = GetRole();
            Book book;
            if (role == "membre")
            {
                book = await _context.Books.Where(x => x.Status == BookStatus.Available).FirstOrDefaultAsync(x => x.Id == id);
            }
            else
            {
                book = await _context.Books.FirstOrDefaultAsync(x => x.Id == id);
            }

            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }

        // GET: Livres/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Livres/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Content,CreatedDate,Status,UserId")] Book livre)
        {
            if (ModelState.IsValid)
            {
                livre.UserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                livre.CreatedDate = DateTime.Now;
                _context.Add(livre);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(livre);
        }

        // GET: Livres/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var userId = GetUserId();
            var role = GetRole();
            Book book;
            if (role == "membre" || role == "bibliothecaire")
            {
                book = await _context.Books.Where(x => x.UserId == userId).FirstOrDefaultAsync(x => x.Id == id);
            }
            else
            {
                book = await _context.Books.FindAsync(id);
            }
            
            if (book == null) return NotFound();

            return View(book);
        }

        // POST: Livres/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Content,CreatedDate,Status,UserId")] Book livre)
        {
            if (id != livre.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(livre);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LivreExists(livre.Id))
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
            return View(livre);
        }

        // GET: Livres/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var userId = GetUserId();
            var role = GetRole();
            Book book;
            if (role == "membre" || role == "bibliothecaire")
            {
                book = await _context.Books.Where(x => x.UserId == userId).FirstOrDefaultAsync(x => x.Id == id);
            }
            else
            {
                book = await _context.Books.FindAsync(id);
            }

            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }

        // POST: Livres/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var livre = await _context.Books.FindAsync(id);
            _context.Books.Remove(livre);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "bibliothecaire, administrator")]
        public async Task<IActionResult> ApproveOrReject(int id, BookStatus status)
        {
            var livre = await _context.Books.FindAsync(id);

            if (livre == null) return NotFound();

            livre.Status = status;
            _context.Books.Update(livre);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private string GetUserId()
        {
            return User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        }

        private string GetRole()
        {
            return User.FindFirst(ClaimTypes.Role)?.Value;
        }

        private bool LivreExists(int id)
        {
            return _context.Books.Any(e => e.Id == id);
        }
    }
}
