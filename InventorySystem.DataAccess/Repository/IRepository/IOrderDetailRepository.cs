using InventorySystem.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace InventorySystem.DataAccess.Repository.IRepository
{
    public interface IOrderDetailRepository: IRepository<OrderDetail>
    {
        void Update(OrderDetail orderDetail);
    }
}
