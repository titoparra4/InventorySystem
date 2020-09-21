using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InventorySystem.DataAccess.Repository.IRepository;
using InventorySystem.Models;
using InventorySystem.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InventorySystem.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = DS.Role_Admin)]
    public class BrandController : Controller
    {
        private readonly IWorkUnit _workUnit;
        public BrandController(IWorkUnit workUnit)
        {
            _workUnit = workUnit;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Upsert(int? id)
        {
            Brand brand = new Brand();
            if(id == null)
            {
                //This is to create a new record
                return View(brand);
            }
            //This is to update
            brand = _workUnit.Brand.Get(id.GetValueOrDefault());
            if(brand == null)
            {
                return NotFound();
            }
            return View(brand);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(Brand brand)
        {
            if(ModelState.IsValid)
            {
                if(brand.Id==0)
                {
                    _workUnit.Brand.Add(brand);
                }
                else
                {
                    _workUnit.Brand.Update(brand);
                }
                _workUnit.Save();
                return RedirectToAction(nameof(Index));
            }
            return View(brand);
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var all = _workUnit.Brand.GetAll();
            return Json(new { data = all });
        }

        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var brandDb = _workUnit.Brand.Get(id);
            if(brandDb == null)
            {
                return Json(new { success = false, message = "Delete failed" });
            }
            _workUnit.Brand.Remove(brandDb);
            _workUnit.Save();
            return Json(new { success = true, message = "Brand successfully deleted" });
        }
    }
}
