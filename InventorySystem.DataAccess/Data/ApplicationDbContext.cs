using System;
using System.Collections.Generic;
using System.Text;
using InventorySystem.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace InventorySystem.DataAccess.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Warehouse> Warehouses { get; set; }

        public DbSet<Category> Categories { get; set; }
    }
}
