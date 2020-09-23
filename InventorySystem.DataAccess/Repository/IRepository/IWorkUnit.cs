using System;
using System.Collections.Generic;
using System.Text;

namespace InventorySystem.DataAccess.Repository.IRepository
{
    public interface IWorkUnit : IDisposable
    {
        IWarehouseRepository Warehouse { get; }
        ICategoryRepository Category { get; }
        IBrandRepository Brand { get; }
        IProductRepository Product { get; }
        IUserApplicationRepository UserApplication { get; }
        ICompanyRepository Company { get; }

        void Save();
    }
}
