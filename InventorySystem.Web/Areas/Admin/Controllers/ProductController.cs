using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using InventorySystem.DataAccess.Repository.IRepository;
using InventorySystem.Models;
using InventorySystem.Models.ViewModels;
using InventorySystem.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace InventorySystem.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = DS.Role_Admin+","+DS.Role_Inventary)]
    public class ProductController : Controller
    {
        private readonly IWorkUnit _workUnit;
        private readonly IWebHostEnvironment _hostEviroment;
        public ProductController(IWorkUnit workUnit, IWebHostEnvironment hostEnvironment)
        {
            _workUnit = workUnit;
            _hostEviroment = hostEnvironment;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Upsert(int? id)
        {
            ProductViewModel productViewModel = new ProductViewModel() {

                Product = new Product(),
                CategoryList = _workUnit.Category.GetAll().Select(c => new SelectListItem { 
                    Text = c.Name,
                    Value = c.Id.ToString()
                }),
                BrandList = _workUnit.Brand.GetAll().Select(b => new SelectListItem { 
                    Text = b.Name,
                    Value = b.Id.ToString()
                }),
                PaterList = _workUnit.Product.GetAll().Select(p => new SelectListItem
                {
                    Text = p.Description,
                    Value = p.Id.ToString()
                })
            };
            if(id == null)
            {
                //This is to create a new record
                return View(productViewModel);
            }
            //This is to update
            productViewModel.Product = _workUnit.Product.Get(id.GetValueOrDefault());
            if(productViewModel.Product == null)
            {
                return NotFound();
            }
            return View(productViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(ProductViewModel productViewModel)
        {
            //Upload Images
            string webRootPath = _hostEviroment.WebRootPath;
            var files = HttpContext.Request.Form.Files;
            if(files.Count>0)
            {
                string filename = Guid.NewGuid().ToString();
                var uploads = Path.Combine(webRootPath, @"images\products");
                var extension = Path.GetExtension(files[0].FileName);
                if (productViewModel.Product.ImageUrl!=null)
                {
                    var imagenPath = Path.Combine(webRootPath, productViewModel.Product.ImageUrl.TrimStart('\\'));
                    if(System.IO.File.Exists(imagenPath))
                    {
                        System.IO.File.Delete(imagenPath);
                    }
                }

                using (var filesStreams = new FileStream(Path.Combine(uploads,filename + extension), FileMode.Create))
                {
                    files[0].CopyTo(filesStreams);
                }
                productViewModel.Product.ImageUrl = @"\images\products\" + filename + extension;
            }
            else
            {
                if(productViewModel.Product.Id!=0)
                {
                    Product productDB = _workUnit.Product.Get(productViewModel.Product.Id);
                    productViewModel.Product.ImageUrl = productDB.ImageUrl;
                }
            }

            if(ModelState.IsValid)
            {
                if(productViewModel.Product.Id==0)
                {
                    _workUnit.Product.Add(productViewModel.Product);
                }
                else
                {
                    _workUnit.Product.Update(productViewModel.Product);
                }
                _workUnit.Save();
                return RedirectToAction(nameof(Index));
            }
            else
            {
                productViewModel.CategoryList = _workUnit.Category.GetAll().Select(c => new SelectListItem
                {
                    Text = c.Name,
                    Value = c.Id.ToString()
                });
                productViewModel.BrandList = _workUnit.Brand.GetAll().Select(b => new SelectListItem
                {
                    Text = b.Name,
                    Value = b.Id.ToString()
                });
                productViewModel.PaterList = _workUnit.Product.GetAll().Select(p => new SelectListItem
                {
                    Text = p.Description,
                    Value = p.Id.ToString()
                });

                if (productViewModel.Product.Id !=0)
                {
                    productViewModel.Product = _workUnit.Product.Get(productViewModel.Product.Id);
                }
            }
            return View(productViewModel.Product);
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var all = _workUnit.Product.GetAll(IncludeProperties: "Category,Brand");
            return Json(new { data = all });
        }

        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var productDb = _workUnit.Product.Get(id);
            if(productDb == null)
            {
                return Json(new { success = false, message = "Delete failed" });
            }
            //Delete Image
            string webRootPath = _hostEviroment.WebRootPath;
            var imagenPath = Path.Combine(webRootPath, productDb.ImageUrl.TrimStart('\\'));
            if (System.IO.File.Exists(imagenPath))
            {
                System.IO.File.Delete(imagenPath);
            }

            _workUnit.Product.Remove(productDb);
            _workUnit.Save();
            return Json(new { success = true, message = "Product successfully deleted" });
        }
    }
}
