using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using InventorySystem.Models.ViewModels;
using InventorySystem.DataAccess.Repository.IRepository;
using InventorySystem.Models;

namespace InventorySystem.Web.Areas.Inventario.Controllers
{
    [Area("Inventory")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IWorkUnit _workUnit;

        public HomeController(ILogger<HomeController> logger, IWorkUnit workUnit)
        {
            _logger = logger;
            _workUnit = workUnit;
        }

        public IActionResult Index()
        {
            IEnumerable<Product> productList = _workUnit.Product.GetAll(IncludeProperties: "Category,Brand");
            return View(productList);
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
}
