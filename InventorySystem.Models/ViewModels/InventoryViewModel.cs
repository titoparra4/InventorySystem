using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Text;

namespace InventorySystem.Models.ViewModels
{
    public class InventoryViewModel
    {
        public Inventory Inventory { get; set; }

        public InventoryDetail InventoryDetail { get; set; }

        public List<InventoryDetail> InventoryDetails { get; set; }

        public IEnumerable<SelectListItem>  WarehouseList { get; set; }

        public IEnumerable<SelectListItem> ProductList { get; set; }


    }
}
