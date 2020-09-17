using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace InventorySystem.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        [Display(Name = "Serial number")]
        public string SerialNumber { get; set; }

        [Required]
        [MaxLength(60)]
        [Display(Name = "Description")]
        public string Description { get; set; }

        [Required]
        [Range(1, 100000000)]
        [Display(Name = "Price")]
        public double Price { get; set; }

        [Required]
        [Range(1, 100000000)]
        [Display(Name ="Cost")]
        public double Cost { get; set; }

        public string ImageUrl { get; set; }

        [Required]
        public int CategoryId { get; set; }

        [ForeignKey("CategoryId")]
        public Category Category { get; set; }

        [Required]
        public int BrandId { get; set; }

        [ForeignKey("BrandId")]
        public Brand Brand { get; set; }

        public int? PaterId { get; set; }
        public virtual Product Pater { get; set; }
    }
}
