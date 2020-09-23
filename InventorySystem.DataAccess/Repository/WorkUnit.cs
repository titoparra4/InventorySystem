using InventorySystem.DataAccess.Data;
using InventorySystem.DataAccess.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Text;

namespace InventorySystem.DataAccess.Repository
{
    public class WorkUnit : IWorkUnit
    {
        private readonly ApplicationDbContext _db;
        public IWarehouseRepository Warehouse { get; private set; }
        public ICategoryRepository Category { get; private set; }
        public IBrandRepository Brand { get; private set; }
        public IProductRepository Product { get; private set; }
        public IUserApplicationRepository UserApplication { get; private set; }
        public ICompanyRepository Company { get; private set; }

        public IShoppingCartRepository ShoppingCart { get; private set; }

        public IOrderRepository Order { get; private set; }

        public IOrderDetailRepository OrderDetail { get; private set; }

        public WorkUnit(ApplicationDbContext db)
        {
            _db = db;
            Warehouse = new WarehouseRepository(_db);
            Category = new CategoryRepository(_db);
            Brand = new BrandRepository(_db);
            Product = new ProductRepository(_db);
            UserApplication = new UserApplicationRepository(_db);
            Company = new CompanyRepository(_db);
            ShoppingCart = new ShoppingCartRepository(_db);
            Order = new OrderRepository(_db);
            OrderDetail = new OrderDetailRepository(_db);
        }

        public void Save()
        {
            _db.SaveChanges();
        }

        public void Dispose()
        {
            _db.Dispose();
        }
    }
}
