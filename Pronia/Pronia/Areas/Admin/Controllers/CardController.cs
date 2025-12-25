using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pronia.Contexts;
using Pronia.Models;


namespace Pronia.Areas.Admin.Controllers;

[Area("Admin")]
[AutoValidateAntiforgeryToken]
public class CardController : Controller
{
    
    
    private readonly ProniaDbContext _context;
    public CardController(ProniaDbContext context)
    {
        _context = context;
    }
    // GET
    public async Task<IActionResult> Index()
    {
        var cards =  await _context.Cards.ToListAsync();
        return View(cards);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View();

    }
    
    [HttpPost]
    public async Task<IActionResult> Create(Card card)
    {
        if (!ModelState.IsValid)
        {
            return View();
        }
        
        await _context.Cards.AddAsync(card);
        await _context.SaveChangesAsync();
            
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int? id)
    {
        var card = await _context.Cards.FindAsync(id);
        if (card == null)
            return NotFound();
        _context.Cards.Remove(card);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
      
    }

    [HttpGet]
    public async Task<IActionResult> Update(int id)
    {
        var card = await _context.Cards.FindAsync(id);
        if (card is null)
            return NotFound();
        
        return View(card);
    }

    [HttpPost]
    public async Task<IActionResult> Update(Card card)
    {
        if (!ModelState.IsValid)
            return View();
        
        var existCard =await _context.Cards.FindAsync(card.Id);
        
        if (existCard is null)
            return BadRequest();
        
        existCard.Title = card.Title;
        existCard.Description = card.Description;
        existCard.ImageUrl = card.ImageUrl;
        
        await _context.SaveChangesAsync();
        
        return RedirectToAction(nameof(Index));
        
    }
    
    
}