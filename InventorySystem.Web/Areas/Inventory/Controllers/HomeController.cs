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
using InventorySystem.DataAccess.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using InventorySystem.Utilities;

namespace InventorySystem.Web.Areas.Inventario.Controllers
{
    [Area("Inventory")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IWorkUnit _workUnit;
        private readonly ApplicationDbContext _db;

        [BindProperty]
        public ShoppingCartViewModel ShoppingCartViewModel { get; set; }

        public HomeController(ILogger<HomeController> logger, IWorkUnit workUnit, ApplicationDbContext db)
        {
            _logger = logger;
            _workUnit = workUnit;
            _db = db;
        }

        public IActionResult Index()
        {
            IEnumerable<Product> productList = _workUnit.Product.GetAll(IncludeProperties: "Category,Brand");

            var claimIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);
            if(claim!=null)
            {

                var productsNumber = _workUnit.ShoppingCart.GetAll(c => c.UserApplicationId ==
                                                                    claim.Value).ToList().Count();

                HttpContext.Session.SetInt32(DS.ssShoppingCart, productsNumber);
            }


            return View(productList);
        }

        public IActionResult Detail(int id)
        {
            ShoppingCartViewModel = new ShoppingCartViewModel();
            ShoppingCartViewModel.Company = _db.Companies.FirstOrDefault();
            ShoppingCartViewModel.WarehouseProduct = _db.WarehouseProducts.Include(p => p.Product.Category).Include(p => p.Product.Brand)
                                                         .FirstOrDefault(b => b.ProductId == id && b.WarehouseId == ShoppingCartViewModel.Company.WarehouseSaleId);

            if(ShoppingCartViewModel.WarehouseProduct==null)
            {
                return RedirectToAction("Index");
            }
            else
            {
                ShoppingCartViewModel.ShoppingCart = new ShoppingCart()
                {
                    Product = ShoppingCartViewModel.WarehouseProduct.Product,
                    ProductId = ShoppingCartViewModel.WarehouseProduct.ProductId
                };
                return View(ShoppingCartViewModel);
            }

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public IActionResult Detail(ShoppingCartViewModel shoppingCartViewModel)
        {
            var claimIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);
            shoppingCartViewModel.ShoppingCart.UserApplicationId = claim.Value;

            ShoppingCart cartDb = _workUnit.ShoppingCart.GetFirts(
                    u=>u.UserApplicationId == shoppingCartViewModel.ShoppingCart.UserApplicationId
                    && u.ProductId == shoppingCartViewModel.ShoppingCart.ProductId,
                    IncludeProperties:"Product"
                );
            if (cartDb ==null)
            {
                _workUnit.ShoppingCart.Add(shoppingCartViewModel.ShoppingCart);
            }
            else
            {
                cartDb.Quantity += shoppingCartViewModel.ShoppingCart.Quantity;
                _workUnit.ShoppingCart.Update(cartDb);
            }
            _workUnit.Save();

            var productsNumber = _workUnit.ShoppingCart.GetAll(c => c.UserApplicationId ==
                                                                shoppingCartViewModel.ShoppingCart.UserApplicationId).ToList().Count();

            HttpContext.Session.SetInt32(DS.ssShoppingCart, productsNumber);

            return RedirectToAction(nameof(Index));
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
