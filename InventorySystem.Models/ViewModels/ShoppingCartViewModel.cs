﻿using System;
using System.Collections.Generic;
using System.Text;

namespace InventorySystem.Models.ViewModels
{
    public class ShoppingCartViewModel
    {
        public Company Company { get; set; }
        public  WarehouseProduct WarehouseProduct { get; set; }
        public ShoppingCart ShoppingCart { get; set; }
        public IEnumerable<ShoppingCart> ShoppingCartsList { get; set; }
        public Order Order { get; set; }

        public IEnumerable<OrderDetail> OrderDetailsList { get; set; }


    }
}
