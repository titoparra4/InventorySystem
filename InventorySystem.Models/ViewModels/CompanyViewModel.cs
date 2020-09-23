using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Text;

namespace InventorySystem.Models.ViewModels
{
    public class CompanyViewModel
    {
        public Company Company { get; set; }
        public IEnumerable<SelectListItem> WarehouseList { get; set; }
    }
}
