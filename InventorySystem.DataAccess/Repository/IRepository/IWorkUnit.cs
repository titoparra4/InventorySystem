using System;
using System.Collections.Generic;
using System.Text;

namespace InventorySystem.DataAccess.Repository.IRepository
{
    public interface IWorkUnit : IDisposable
    {
        IWarehouseRepository Warehouse { get; }

        void Save();
    }
}
