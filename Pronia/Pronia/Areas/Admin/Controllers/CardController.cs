using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Pronia.Contexts;
using Pronia.Models;

namespace Pronia.Areas.Admin.Controllers
{
    [Area("Admin")]
    [AutoValidateAntiforgeryToken]
    public class CardController : Controller
    {
        private readonly ProniaDbContext _context;

        public CardController(ProniaDbContext context)
        {
            _context = context;
        }

        
        public async Task<IActionResult> Index()
        {
            var cards = await _context.Cards.Include(c => c.Category).ToListAsync();
            return View(cards);
        }

        
        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.Categories = new SelectList(_context.Categories.ToList(), "Id", "Name");
            return View();
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Card card)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Categories = new SelectList(_context.Categories.ToList(), "Id", "Name");
                return View(card);
            }

            await _context.Cards.AddAsync(card);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // GET: Admin/Card/Update/5
        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            var card = await _context.Cards.FindAsync(id);
            if (card == null) return NotFound();

            ViewBag.Categories = new SelectList(_context.Categories.ToList(), "Id", "Name", card.CategoryId);
            return View(card);
        }

        // POST: Admin/Card/Update
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(Card card)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Categories = new SelectList(_context.Categories.ToList(), "Id", "Name", card.CategoryId);
                return View(card);
            }

            var existCard = await _context.Cards.FindAsync(card.Id);
            if (existCard == null) return BadRequest();

            existCard.Title = card.Title;
            existCard.Description = card.Description;
            existCard.ImageUrl = card.ImageUrl;
            existCard.CategoryId = card.CategoryId;

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return BadRequest();

            var card = await _context.Cards.FindAsync(id);
            if (card == null) return NotFound();

            _context.Cards.Remove(card);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
