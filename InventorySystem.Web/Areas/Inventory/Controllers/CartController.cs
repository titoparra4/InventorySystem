using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using InventorySystem.DataAccess.Repository.IRepository;
using InventorySystem.Models.ViewModels;
using InventorySystem.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;

namespace InventorySystem.Web.Areas.Inventory.Controllers
{
    [Area("Inventory")]
    public class CartController : Controller
    {
        private readonly IWorkUnit _workUnit;
        private readonly IEmailSender _emailSender;
        private readonly UserManager<IdentityUser> _userManager;

        public ShoppingCartViewModel ShoppingCartViewModel   { get; set; }

        public CartController(IWorkUnit workUnit, IEmailSender emailSender, UserManager<IdentityUser> userManager )
        {
            _workUnit = workUnit;
            _emailSender = emailSender;
            _userManager = userManager;
        }


        public IActionResult Index()
        {
            var claimIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);

            ShoppingCartViewModel = new ShoppingCartViewModel()
            {
                Order = new Models.Order(),
                ShoppingCartsList = _workUnit.ShoppingCart.GetAll(u => u.UserApplicationId == claim.Value, IncludeProperties: "Product")
            };

            ShoppingCartViewModel.Order.OrderTotal = 0;
            ShoppingCartViewModel.Order.UserApplication = _workUnit.UserApplication.GetFirts(u => u.Id == claim.Value);

            foreach (var list in ShoppingCartViewModel.ShoppingCartsList)
            {
                list.Price = list.Product.Price;
                ShoppingCartViewModel.Order.OrderTotal += (list.Price * list.Quantity);
            }

            return View(ShoppingCartViewModel);
        }

        public IActionResult plus(int cartId)
        {
            var shoppingCart = _workUnit.ShoppingCart.GetFirts(c => c.Id == cartId, IncludeProperties: "Product");
            shoppingCart.Quantity += 1;
            _workUnit.Save();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult less(int cartId)
        {
            var shoppingCart = _workUnit.ShoppingCart.GetFirts(c => c.Id == cartId, IncludeProperties: "Product");
            if(shoppingCart.Quantity==1)
            {
                var productsNumber = _workUnit.ShoppingCart.GetAll(u => u.UserApplicationId == shoppingCart.UserApplicationId).ToList().Count();

                _workUnit.ShoppingCart.Remove(shoppingCart);
                _workUnit.Save();
                HttpContext.Session.SetInt32(DS.ssShoppingCart, productsNumber - 1);

            }
            else
            {
                shoppingCart.Quantity -= 1;
                _workUnit.Save();
            }
            return RedirectToAction(nameof(Index));
        }

        public IActionResult remove(int cartId)
        {
            var shoppingCart = _workUnit.ShoppingCart.GetFirts(c => c.Id == cartId, IncludeProperties: "Product");
           
            var productsNumber = _workUnit.ShoppingCart.GetAll(u => u.UserApplicationId == shoppingCart.UserApplicationId).ToList().Count();

                _workUnit.ShoppingCart.Remove(shoppingCart);
                _workUnit.Save();
                HttpContext.Session.SetInt32(DS.ssShoppingCart, productsNumber - 1);

           
            return RedirectToAction(nameof(Index));
        }
    }
}
