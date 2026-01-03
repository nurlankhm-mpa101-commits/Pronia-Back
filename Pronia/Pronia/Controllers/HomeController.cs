using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pronia.Contexts;
using Pronia.Models;
using Pronia.ViewModels;

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
        // Создаем нашу посылку
        HomeVM vm = new HomeVM();
            
        // Заполняем её данными из таблиц базы данных
        vm.Cards = _context.Cards.ToList();
        vm.Products = _context.Products.ToList();

        // Отправляем посылку во View
        return View(vm);
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