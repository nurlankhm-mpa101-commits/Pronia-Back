using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Pronia.Contexts;
using Pronia.Models;
using Pronia.Areas.Admin.ViewModels.Product;

namespace Pronia.Areas.Admin.Controllers
{
    [Area("Admin")]
    [AutoValidateAntiforgeryToken]
    public class ProductController : Controller
    {
        private readonly ProniaDbContext _context;
        private readonly IWebHostEnvironment _env;

        public ProductController(ProniaDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        public async Task<IActionResult> Index()
        {
            var products = await _context.Products.Include(p => p.Category).ToListAsync();
            return View(products);
        }

        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.Categories = new SelectList(_context.Categories.ToList(), "Id", "Name");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(ProductCreateVM vm)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Categories = new SelectList(_context.Categories.ToList(), "Id", "Name", vm.CategoryId);
                return View(vm);
            }

            string fileName = Guid.NewGuid().ToString() + Path.GetExtension(vm.Image.FileName);
            string path = Path.Combine(_env.WebRootPath, "uploads", fileName);

            using (FileStream fs = new(path, FileMode.Create))
            {
                await vm.Image.CopyToAsync(fs);
            }

            Product p = new()
            {
                Name = vm.Name,
                Description = vm.Description,
                Price = vm.Price,
                ImagePath = "/uploads/" + fileName,
                CategoryId = vm.CategoryId
            };

            await _context.Products.AddAsync(p);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null) return NotFound();

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return Json(new { success = true });
        }
    }
}
