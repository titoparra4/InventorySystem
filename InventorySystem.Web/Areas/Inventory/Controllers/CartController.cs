using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using InventorySystem.DataAccess.Repository.IRepository;
using InventorySystem.Models;
using InventorySystem.Models.ViewModels;
using InventorySystem.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Stripe;

namespace InventorySystem.Web.Areas.Inventory.Controllers
{
    [Area("Inventory")]
    public class CartController : Controller
    {
        private readonly IWorkUnit _workUnit;
        private readonly IEmailSender _emailSender;
        private readonly UserManager<IdentityUser> _userManager;

        [BindProperty]
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

        public IActionResult Proceed()
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

            ShoppingCartViewModel.Order.NameClient = ShoppingCartViewModel.Order.UserApplication.Name + " " +
                                                     ShoppingCartViewModel.Order.UserApplication.LastName;

            ShoppingCartViewModel.Order.Phone = ShoppingCartViewModel.Order.UserApplication.PhoneNumber;
            ShoppingCartViewModel.Order.Address = ShoppingCartViewModel.Order.UserApplication.Address;
            ShoppingCartViewModel.Order.City = ShoppingCartViewModel.Order.UserApplication.City;
            ShoppingCartViewModel.Order.Country = ShoppingCartViewModel.Order.UserApplication.Country;


            return View(ShoppingCartViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Proceed")]
        public IActionResult ProceedPost(string stripeToken)
        {
            var claimIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);

            ShoppingCartViewModel.Order.UserApplication = _workUnit.UserApplication.GetFirts(c => c.Id == claim.Value);
            ShoppingCartViewModel.ShoppingCartsList = _workUnit.ShoppingCart.GetAll(c=>c.UserApplicationId==claim.Value, IncludeProperties:"Product");
            ShoppingCartViewModel.Order.OrderStatus = DS.Pendingstatus;
            ShoppingCartViewModel.Order.PaymentStatus = DS.PaymentStatusPending;
            ShoppingCartViewModel.Order.UserApplicationId = claim.Value;
            ShoppingCartViewModel.Order.OrderDate = DateTime.Now;

            _workUnit.Order.Add(ShoppingCartViewModel.Order);
            _workUnit.Save();

            foreach (var item in ShoppingCartViewModel.ShoppingCartsList)
            {
                OrderDetail orderDetail = new OrderDetail()
                {
                    ProductId = item.ProductId,
                    OrderId = ShoppingCartViewModel.Order.Id,
                    Price = item.Product.Price,
                    Quantity = item.Quantity,
                };
                ShoppingCartViewModel.Order.OrderTotal += orderDetail.Quantity * orderDetail.Price;
                _workUnit.OrderDetail.Add(orderDetail);
                
            }

            _workUnit.ShoppingCart.RemoveRank(ShoppingCartViewModel.ShoppingCartsList);
            _workUnit.Save();
            HttpContext.Session.SetInt32(DS.ssShoppingCart, 0);

            if(stripeToken == null)
            {

            }
            else
            {
                var options = new ChargeCreateOptions
                {
                    Amount = Convert.ToInt32(ShoppingCartViewModel.Order.OrderTotal * 100),
                    Currency = "usd",
                    Description = "Number of order: " + ShoppingCartViewModel.Order.Id,
                    Source = stripeToken
                };
                var service = new ChargeService();
                Charge charge = service.Create(options);
                if(charge.BalanceTransactionId==null)
                {
                    ShoppingCartViewModel.Order.PaymentStatus = DS.PaymentStatusRejected;
                }
                else
                {
                    ShoppingCartViewModel.Order.TransactionId = charge.BalanceTransactionId;
                }
                if (charge.Status.ToLower()=="succeeded")
                {
                    ShoppingCartViewModel.Order.PaymentStatus = DS.PaymentStatusApproved;
                    ShoppingCartViewModel.Order.OrderStatus = DS.ApprovedStatus;
                    ShoppingCartViewModel.Order.PaymentDate = DateTime.Now;
                }
            }

            _workUnit.Save();

            return RedirectToAction("OrderConfirmation","Cart", new { id=ShoppingCartViewModel.Order.Id});

        }

        public IActionResult OrderConfirmation(int id)
        {
            return View(id); 
        }
    }
}
