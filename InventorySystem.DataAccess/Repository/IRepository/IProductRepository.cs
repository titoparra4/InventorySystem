using InventorySystem.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace InventorySystem.DataAccess.Repository.IRepository
{
    public interface IProductRepository : IRepository<Product>
    {
        void Update(Product product);
    }
}
