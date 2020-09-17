using InventorySystem.DataAccess.Data;
using InventorySystem.DataAccess.Repository.IRepository;
using InventorySystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InventorySystem.DataAccess.Repository
{
    public class UserApplicationRepository : Repository<UserApplication>, IUserApplicationRepository
    {

        private readonly ApplicationDbContext _db;

        public UserApplicationRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }
        
    }
}
