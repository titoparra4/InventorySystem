using System;
using System.Collections.Generic;
using System.Text;

namespace InventorySystem.Models.ViewModels
{
    public class OrderDetailViewModel
    {
        public Company Company { get; set; }

        public Order Order { get; set; }

        public IEnumerable<OrderDetail> OrderDetailList { get; set; }
    }
}
