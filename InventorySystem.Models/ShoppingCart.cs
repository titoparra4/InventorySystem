using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace InventorySystem.Models
{
    public class ShoppingCart
    {
        public ShoppingCart()
        {
            Quantity = 1;
        }

        [Key]
        public int Id { get; set; }

        [Required]
        public string UserApplicationId { get; set; }

        [ForeignKey("UserApplicationId")]
        public UserApplication UserApplication { get; set; }

        [Required]
        public int ProductId { get; set; }

        [ForeignKey("ProductId")]
        public Product Product { get; set; }

        [Required]
        [Range(1,1000,ErrorMessage = "Enter a value from 1 to 1000")]
        public int Quantity { get; set; }

        [NotMapped]
        public double Price { get; set; }
    }
}
