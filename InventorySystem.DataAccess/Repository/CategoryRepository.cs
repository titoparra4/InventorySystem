using InventorySystem.DataAccess.Data;
using InventorySystem.DataAccess.Repository.IRepository;
using InventorySystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InventorySystem.DataAccess.Repository
{
    public class CategoryRepository : Repository<Category>, ICategoryRepository
    {
        private readonly ApplicationDbContext _db;
        public CategoryRepository(ApplicationDbContext db) :base(db)
        {
            _db = db;
        }

        public void Update(Category category)
        {
            var categoryDb = _db.Categories.FirstOrDefault(b => b.Id == category.Id);
            if(category != null)
            {
                categoryDb.Name = category.Name;
                categoryDb.Status = category.Status;
            }
        }
    }
}
