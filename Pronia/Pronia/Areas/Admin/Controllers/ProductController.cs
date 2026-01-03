using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Pronia.Contexts;
using Pronia.Models;
using Pronia.Areas.Admin.ViewModels.Product; // Убедись, что тут лежит ProductUpdateVM
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

        // 1. СПИСОК ТОВАРОВ
        public async Task<IActionResult> Index()
        {
            var products = await _context.Products.Include(p => p.Category).ToListAsync();
            return View(products);
        }

        // 2. СОЗДАНИЕ (GET)
        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.Categories = new SelectList(_context.Categories.ToList(), "Id", "Name");
            ViewBag.Tags = new MultiSelectList(_context.Tags.ToList(), "Id", "Name");
            return View();
        }

        // 3. СОЗДАНИЕ (POST)
        [HttpPost]
        public async Task<IActionResult> Create(ProductCreateVM vm)
        {
            ViewBag.Categories = new SelectList(_context.Categories.ToList(), "Id", "Name");
            ViewBag.Tags = new MultiSelectList(_context.Tags.ToList(), "Id", "Name");

            if (!ModelState.IsValid) return View(vm);

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
            // Сохраняем в ту же папку, что и раньше
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

            if (vm.TagIds != null)
            {
                foreach (var tagId in vm.TagIds)
                {
                    await _context.ProductTags.AddAsync(new ProductTag { ProductId = p.Id, TagId = tagId });
                }
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        // 4. РЕДАКТИРОВАНИЕ (GET)
        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            var product = await _context.Products
                .Include(p => p.ProductTags)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null) return NotFound();

            ProductUpdateVM updateVM = new ProductUpdateVM
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                CategoryId = product.CategoryId,
                ExistingImagePath = product.ImagePath,
                TagIds = product.ProductTags.Select(pt => pt.TagId).ToList()
            };

            ViewBag.Categories = new SelectList(_context.Categories, "Id", "Name", product.CategoryId);
            ViewBag.Tags = new MultiSelectList(_context.Tags, "Id", "Name", updateVM.TagIds);
            
            return View(updateVM);
        }

        // 5. РЕДАКТИРОВАНИЕ (POST)
        [HttpPost]
        public async Task<IActionResult> Update(ProductUpdateVM vm)
        {
            ViewBag.Categories = new SelectList(_context.Categories, "Id", "Name", vm.CategoryId);
            ViewBag.Tags = new MultiSelectList(_context.Tags, "Id", "Name", vm.TagIds);

            if (!ModelState.IsValid) return View(vm);

            var product = await _context.Products
                .Include(p => p.ProductTags)
                .FirstOrDefaultAsync(p => p.Id == vm.Id);

            if (product == null) return NotFound();

            // Если загружена новая картинка
            if (vm.ImageFile != null)
            {
                if (!vm.ImageFile.IsImage())
                {
                    ModelState.AddModelError("ImageFile", "Please select image file");
                    return View(vm);
                }
                
                // Удаляем старую картинку (опционально, но полезно)
                string oldFilePath = Path.Combine(_env.WebRootPath, product.ImagePath.TrimStart('/'));
                if (System.IO.File.Exists(oldFilePath)) System.IO.File.Delete(oldFilePath);

                string fileName = Guid.NewGuid() + Path.GetExtension(vm.ImageFile.FileName);
                string savePath = Path.Combine(_env.WebRootPath, "admin/assets/images", fileName);

                using (FileStream fs = new FileStream(savePath, FileMode.Create))
                {
                    await vm.ImageFile.CopyToAsync(fs);
                }
                product.ImagePath = $"/admin/assets/images/{fileName}";
            }

            product.Name = vm.Name;
            product.Description = vm.Description;
            product.Price = vm.Price;
            product.CategoryId = vm.CategoryId;

            // Обновляем теги: удаляем старые и добавляем новые
            _context.ProductTags.RemoveRange(product.ProductTags);
            if (vm.TagIds != null)
            {
                foreach (var tagId in vm.TagIds)
                {
                    product.ProductTags.Add(new ProductTag { ProductId = product.Id, TagId = tagId });
                }
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // 6. УДАЛЕНИЕ (Через Fetch/SweetAlert)
        [HttpPost] 
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null) return Json(new { success = false, message = "Товар не найден" });

            // Удаляем файл изображения с диска перед удалением записи из базы
            if (!string.IsNullOrEmpty(product.ImagePath))
            {
                // Убираем начальный слэш, чтобы путь собрался правильно
                string relativePath = product.ImagePath.TrimStart('/');
                string fullPath = Path.Combine(_env.WebRootPath, relativePath);

                if (System.IO.File.Exists(fullPath))
                {
                    System.IO.File.Delete(fullPath);
                }
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return Json(new { success = true });
        }
    }
}