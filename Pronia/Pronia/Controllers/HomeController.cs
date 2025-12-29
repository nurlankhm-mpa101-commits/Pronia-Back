using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pronia.Contexts;
using Pronia.Models;

namespace Pronia.Controllers;

public class HomeController : Controller
{
    
    private readonly ProniaDbContext _context;

    public HomeController(ProniaDbContext context)
    {
        _context = context;
        
        
    }
    
    
    public async Task<IActionResult> Index()
    {
        List<Card> cards = await _context.Cards.ToListAsync();
        List<Product> products = await  _context.Products.ToListAsync();
        
        var model = Tuple.Create(cards, products);
        
        return View(model);
    }
    
    public IActionResult Products()
    {
        var products = _context.Products.ToList();
        return View(products);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
    
  
}