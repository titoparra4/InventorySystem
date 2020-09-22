using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using InventorySystem.DataAccess.Data;
using InventorySystem.Models;
using InventorySystem.Models.ViewModels;
using InventorySystem.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace InventorySystem.Web.Areas.Inventory.Controllers
{
    [Area("Inventory")]
    [Authorize(Roles = DS.Role_Admin+","+DS.Role_Inventary)]
    public class InventoryController : Controller
    {
        private readonly ApplicationDbContext _db;

        [BindProperty]
        public InventoryViewModel inventoryVW { get; set; }

        public InventoryController(ApplicationDbContext db)
        {
            _db = db;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult NewInventory(int? inventoryId)
        {
            inventoryVW = new InventoryViewModel();

            inventoryVW.WarehouseList = _db.Warehouses.ToList().Select(w => new SelectListItem
            {
                Text = w.Name,
                Value = w.Id.ToString()
            });

            inventoryVW.ProductList = _db.Products.ToList().Select(p => new SelectListItem
            {
                Text = p.Description,
                Value = p.Id.ToString()
            });

            inventoryVW.InventoryDetails = new List<InventoryDetail>();

            if(inventoryId!=null)
            {
                inventoryVW.Inventory = _db.Inventories.SingleOrDefault(i => i.Id == inventoryId);
                inventoryVW.InventoryDetails = _db.InventoryDetails.Include(p => p.Product).Include(b => b.Product.Brand).
                    Where(d => d.InventoryId == inventoryId).ToList();
            }

            return View(inventoryVW); 
        }

        [HttpPost]
        public IActionResult AddProductPost(int product, int quantity, int inventoryId)
        {
            inventoryVW.Inventory.Id = inventoryId;
            if(inventoryVW.Inventory.Id==0)
            {
                inventoryVW.Inventory.Status = false;
                inventoryVW.Inventory.InitialDate = DateTime.Now;
                var claimIdentity = (ClaimsIdentity)User.Identity;
                var claim = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);
                inventoryVW.Inventory.UserApplicationId = claim.Value;

                _db.Inventories.Add(inventoryVW.Inventory);
                _db.SaveChanges();
            }
            else
            {
                inventoryVW.Inventory = _db.Inventories.SingleOrDefault(i => i.Id== inventoryId);
            }

            var warehouseProduct = _db.WarehouseProducts.Include(w => w.Product).FirstOrDefault(b => b.ProductId == product && 
                                                                                                b.WarehouseId == inventoryVW.Inventory.WarehouseId);

            var detail = _db.InventoryDetails.Include(p => p.Product).FirstOrDefault(d => d.ProductId == product &&
                                                                                     d.InventoryId == inventoryVW.Inventory.Id);

            if(detail==null)
            {
                inventoryVW.InventoryDetail = new InventoryDetail();
                inventoryVW.InventoryDetail.ProductId = product;
                inventoryVW.InventoryDetail.InventoryId = inventoryVW.Inventory.Id;
                if(warehouseProduct!=null)
                {
                    inventoryVW.InventoryDetail.PreviousStock = warehouseProduct.Quantity;
                }
                else
                {
                    inventoryVW.InventoryDetail.PreviousStock = 0;
                }
                inventoryVW.InventoryDetail.Quantity = quantity;
                _db.InventoryDetails.Add(inventoryVW.InventoryDetail);
                _db.SaveChanges();
            }
            else
            {
                detail.Quantity += quantity;
                _db.SaveChanges();
            }
            return RedirectToAction("NewInventory", new { inventoryId = inventoryVW.Inventory.Id });
        }

        public IActionResult Plus(int Id)
        {
            inventoryVW = new InventoryViewModel();
            var detail = _db.InventoryDetails.FirstOrDefault(d => d.Id == Id);
            inventoryVW.Inventory = _db.Inventories.FirstOrDefault(i => i.Id == detail.InventoryId);

            detail.Quantity += 1;
            _db.SaveChanges();
            return RedirectToAction("NewInventory", new { inventoryId = inventoryVW.Inventory.Id });
        }

        public IActionResult Less(int Id)
        {
            inventoryVW = new InventoryViewModel();
            var detail = _db.InventoryDetails.FirstOrDefault(d => d.Id == Id);
            inventoryVW.Inventory = _db.Inventories.FirstOrDefault(i => i.Id == detail.InventoryId);
            
            if(detail.Quantity==1)
            {
                _db.InventoryDetails.Remove(detail);
                _db.SaveChanges();
            }
            else
            {
                detail.Quantity -= 1;
                _db.SaveChanges();
            }
            return RedirectToAction("NewInventory", new { inventoryId = inventoryVW.Inventory.Id });
        }

        public IActionResult GenerateStock(int Id)
        {
            var inventory = _db.Inventories.FirstOrDefault(i => i.Id == Id);
            var detailList = _db.InventoryDetails.Where(d => d.InventoryId == Id);

            foreach (var item in detailList)
            {
                var warehouseProduct = _db.WarehouseProducts.Include(p => p.Product).FirstOrDefault(w => w.ProductId == item.ProductId
                                                                     && w.WarehouseId == inventory.WarehouseId);

                if(warehouseProduct!=null)
                {
                    warehouseProduct.Quantity += item.Quantity;
                }
                else
                {
                    warehouseProduct = new WarehouseProduct();
                    warehouseProduct.WarehouseId = inventory.WarehouseId;
                    warehouseProduct.ProductId = item.ProductId;
                    warehouseProduct.Quantity = item.Quantity;
                    _db.WarehouseProducts.Add(warehouseProduct);
                }      
            }

            inventory.Status = true;
            inventory.FinalDate = DateTime.Now;
            _db.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Record()
        {
            return View();
        }

        public IActionResult HistoryDetail(int id)
        {
            var detailHistory = _db.InventoryDetails.Include(p => p.Product).Include(b => b.Product.Brand).Where(d => d.InventoryId == id);
            return View(detailHistory);
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var all = _db.WarehouseProducts.Include(w => w.Warehouse).Include(p => p.Product).ToList();
            return Json(new { data = all });
        }

        [HttpGet]
        public IActionResult GetRecord()
        {
            var all = _db.Inventories.Include(w => w.Warehouse).Include(u => u.UserApplication).Where(i => i.Status == true).ToList();
            return Json(new { data = all });
        }


    }
}
