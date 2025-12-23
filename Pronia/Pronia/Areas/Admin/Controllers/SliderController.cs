using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pronia.Contexts;


namespace Pronia.Areas.Admin.Controllers;

[Area("Admin")]

public class SliderController : Controller
{
    
    
    private readonly ProniaDbContext _context;
    public SliderController(ProniaDbContext context)
    {
        _context = context;
    }
    // GET
    public async Task<IActionResult> Index()
    {
        var sliders =  await _context.Sliders.ToListAsync();
        return View(sliders);
    }
}


