using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace InventorySystem.Models
{
    public class Inventory
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Display(Name ="User")]
        public string UserApplicationId { get; set; }

        [ForeignKey("UserApplicationId")]
        public UserApplication UserApplication { get; set; }

        [Required]
        [Display (Name = "Initial Date")]
        [DisplayFormat(ApplyFormatInEditMode = false, DataFormatString = "{0:dd-MM-yyyy HH:mm}")]
        public DateTime InitialDate { get; set; }

        [Display(Name = "Final Date")]
        [DisplayFormat(ApplyFormatInEditMode = false, DataFormatString = "{0:dd-MM-yyyy HH:mm}")]
        public DateTime FinalDate { get; set; }

        [Required]
        [Display (Name = "Warehouse")]
        public int WarehouseId { get; set; }

        [ForeignKey("WarehouseId")]
        public Warehouse Warehouse { get; set; }

        public bool Status { get; set; }
    }
}
