using Microsoft.AspNetCore.Mvc;
using Pronia.Contexts;

namespace Pronia.Areas.Admin.Controllers;

[Area("Admin")]
public class CategoryController : Controller
{
    private readonly ProniaDbContext _context;

    public CategoryController(ProniaDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var categories = _context.Categories.ToList();
        return View(categories);
    }
}