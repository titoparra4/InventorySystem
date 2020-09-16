using InventorySystem.DataAccess.Data;
using InventorySystem.DataAccess.Repository.IRepository;
using InventorySystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InventorySystem.DataAccess.Repository
{
    public class WarehouseRepository : Repository<Warehouse>, IWarehouseRepository
    {
        private readonly ApplicationDbContext _db;

        public WarehouseRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(Warehouse warehouse)
        {
            var warehouseDb = _db.Warehouses.FirstOrDefault(b => b.Id == warehouse.Id);

            if (warehouseDb != null)
            {
                warehouseDb.Name = warehouse.Name;
                warehouseDb.Description = warehouse.Description;
                warehouseDb.Status = warehouse.Status;


            }    
        }
    }
}
