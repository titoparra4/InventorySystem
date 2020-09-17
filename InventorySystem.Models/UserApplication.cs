using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace InventorySystem.Models
{
    public class UserApplication : IdentityUser
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public string LastName { get; set; }

        public string Address { get; set; }

        public string City { get; set; }

        public string Country { get; set; }

        [NotMapped]
        public string Role { get; set; }
    }
}
