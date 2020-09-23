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
    [Authorize(Roles = DS.Role_Admin)]
    public class CompanyController : Controller
    {
        private readonly IWorkUnit _workUnit;
        private readonly IWebHostEnvironment _hostEviroment;
        public CompanyController(IWorkUnit workUnit, IWebHostEnvironment hostEnvironment)
        {
            _workUnit = workUnit;
            _hostEviroment = hostEnvironment;
        }
        public IActionResult Index()
        {
            var company = _workUnit.Company.GetAll();

            return View(company);
        }

        public IActionResult Upsert(int? id)
        {
            CompanyViewModel companyViewModel = new CompanyViewModel() {

                Company = new Company(),
                WarehouseList = _workUnit.Warehouse.GetAll().Select(c => new SelectListItem { 
                    Text = c.Name,
                    Value = c.Id.ToString()
                }),
            };

            if(id == null)
            {
                //This is to create a new record
                return View(companyViewModel);
            }
            //This is to update
            companyViewModel.Company = _workUnit.Company.Get(id.GetValueOrDefault());
            if(companyViewModel.Company == null)
            {
                return NotFound();
            }
            return View(companyViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(CompanyViewModel companyViewModel)
        {
            //Upload Images
            string webRootPath = _hostEviroment.WebRootPath;
            var files = HttpContext.Request.Form.Files;
            if(files.Count>0)
            {
                string filename = Guid.NewGuid().ToString();
                var uploads = Path.Combine(webRootPath, @"images\company");
                var extension = Path.GetExtension(files[0].FileName);
                if (companyViewModel.Company.LogoUrl!=null)
                {
                    var imagenPath = Path.Combine(webRootPath, companyViewModel.Company.LogoUrl.TrimStart('\\'));
                    if(System.IO.File.Exists(imagenPath))
                    {
                        System.IO.File.Delete(imagenPath);
                    }
                }

                using (var filesStreams = new FileStream(Path.Combine(uploads,filename + extension), FileMode.Create))
                {
                    files[0].CopyTo(filesStreams);
                }
                companyViewModel.Company.LogoUrl = @"\images\company\" + filename + extension;
            }
            else
            {
                if(companyViewModel.Company.Id!=0)
                {
                    Company companyDB = _workUnit.Company.Get(companyViewModel.Company.Id);
                    companyViewModel.Company.LogoUrl = companyDB.LogoUrl;
                }
            }

            if(ModelState.IsValid)
            {
                if(companyViewModel.Company.Id==0)
                {
                    _workUnit.Company.Add(companyViewModel.Company);
                }
                else
                {
                    _workUnit.Company.Update(companyViewModel.Company);
                }
                _workUnit.Save();
                return RedirectToAction(nameof(Index));
            }
            else
            {
                companyViewModel.WarehouseList = _workUnit.Warehouse.GetAll().Select(c => new SelectListItem
                {
                    Text = c.Name,
                    Value = c.Id.ToString()
                });

                if (companyViewModel.Company.Id !=0)
                {
                    companyViewModel.Company = _workUnit.Company.Get(companyViewModel.Company.Id);
                }
            }
            return View(companyViewModel.Company);
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var all = _workUnit.Company.GetAll(IncludeProperties: "Warehouse");
            return Json(new { data = all });
        }

    }
}
