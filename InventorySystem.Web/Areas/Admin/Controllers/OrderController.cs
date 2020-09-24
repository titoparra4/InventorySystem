using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using InventorySystem.DataAccess.Repository.IRepository;
using InventorySystem.Models;
using InventorySystem.Models.ViewModels;
using InventorySystem.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe;

namespace InventorySystem.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class OrderController : Controller
    {
        private readonly IWorkUnit _workUnit;

        [BindProperty]
        public OrderDetailViewModel OrderDetailViewModel { get; set; }

        public OrderController(IWorkUnit workUnit)
        {
            _workUnit = workUnit;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Detail(int id)
        {
            OrderDetailViewModel = new OrderDetailViewModel()
            {
                Order = _workUnit.Order.GetFirts(o=>o.Id == id, IncludeProperties:"UserApplication"),
                OrderDetailList = _workUnit.OrderDetail.GetAll(o=>o.OrderId==id, IncludeProperties:"Product")
            };

            return View(OrderDetailViewModel);
        }

        [Authorize(Roles =DS.Role_Admin+","+DS.Role_Sales)]
        public IActionResult Process(int id)
        {
            Models.Order order = _workUnit.Order.GetFirts(o=>o.Id == id);
            order.OrderStatus = DS.StatusProcess;
            _workUnit.Save();
            return RedirectToAction("Index"); ;
        }

        [HttpPost]
        [Authorize(Roles = DS.Role_Admin + "," + DS.Role_Sales)]
        public IActionResult SendOrder()
        {
            Models.Order order = _workUnit.Order.GetFirts(o => o.Id == OrderDetailViewModel.Order.Id);
            order.DeliveryNumber = OrderDetailViewModel.Order.DeliveryNumber;
            order.Carrier = OrderDetailViewModel.Order.Carrier;
            order.OrderStatus = DS.SubmittedStatus;
            order.ShippingDate = DateTime.Now;
            _workUnit.Save();
            return RedirectToAction("Index");
        }

        [Authorize(Roles = DS.Role_Admin + "," + DS.Role_Sales)]
        public IActionResult CancelOrder(int id)
        {
            Models.Order order = _workUnit.Order.GetFirts(o => o.Id == id);

            if(order.PaymentStatus == DS.PaymentStatusApproved)
            {
                var options = new RefundCreateOptions
                {
                    Amount = Convert.ToInt32(order.OrderTotal * 100),
                    Reason = RefundReasons.RequestedByCustomer,
                    Charge = order.TransactionId
                };
                var service = new RefundService();
                Refund refund = service.Create(options);
                order.OrderStatus = DS.ReturnedStatus;
                order.PaymentStatus = DS.ReturnedStatus;
            }
            else
            {
                order.OrderStatus = DS.CanceledStatus;
                order.PaymentStatus = DS.CanceledStatus;
            }
            _workUnit.Save();
            return RedirectToAction("Index");
        }


        [HttpGet]
        public IActionResult GetOrderList(string status)
        {
            var claimIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);
            IEnumerable<Models.Order> orderList;
            if (User.IsInRole(DS.Role_Admin) || User.IsInRole(DS.Role_Sales))
            {
                orderList = _workUnit.Order.GetAll(IncludeProperties: "UserApplication");
            }
            else
            {
                orderList = _workUnit.Order.GetAll(o => o.UserApplicationId == claim.Value, IncludeProperties: "UserApplication");
            }

            switch (status)
            {
                case "pending":
                    orderList = orderList.Where(o => o.PaymentStatus == DS.PaymentStatusPending ||
                                                   o.PaymentStatus == DS.PaymentStatusDelayed);
                    break;
                case "approved":
                    orderList = orderList.Where(o => o.PaymentStatus == DS.PaymentStatusApproved);
                    break;
                case "rejected":
                    orderList = orderList.Where(o => o.OrderStatus == DS.ReturnedStatus ||
                                                    o.OrderStatus == DS.CanceledStatus);
                    break;
                case "completed":
                    orderList = orderList.Where(o => o.OrderStatus == DS.SubmittedStatus);
                    break;
                default:
                    break;
            }


            return Json(new { data = orderList });
        }
    }
}
