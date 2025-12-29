using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Pronia.Contexts;
using Pronia.Models;
using Pronia.Areas.Admin.ViewModels.Product;
using Pronia.Extensions;

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
            ViewBag.Categories = new SelectList(_context.Categories.ToList(), "Id", "Name");
            ViewBag.Tags = new MultiSelectList(_context.Tags.ToList(), "Id", "Name");

            if (!ModelState.IsValid)
                return View(vm);

            // FILE VALIDATION
            if (!vm.Image.IsImage())
            {
                ModelState.AddModelError("Image", "Please select image file");
                return View(vm);
            }
            if (!vm.Image.LessThan(2))
            {
                ModelState.AddModelError("Image", "Image must be less than 2 MB");
                return View(vm);
            }

            string fileName = Guid.NewGuid() + Path.GetExtension(vm.Image.FileName);
            string savePath = Path.Combine(_env.WebRootPath, "admin/assets/images", fileName);

            using (FileStream fs = new FileStream(savePath, FileMode.Create))
            {
                await vm.Image.CopyToAsync(fs);
            }

            Product p = new Product
            {
                Name = vm.Name,
                Description = vm.Description,
                Price = vm.Price,
                ImagePath = $"/admin/assets/images/{fileName}",
                CategoryId = vm.CategoryId
            };

            await _context.Products.AddAsync(p);
            await _context.SaveChangesAsync();

            // TAG RELATIONS
            foreach (var tagId in vm.TagIds)
            {
                ProductTag pt = new ProductTag
                {
                    ProductId = p.Id,
                    TagId = tagId
                };

                await _context.ProductTags.AddAsync(pt);
            }

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
