using InventorySystem.DataAccess.Data;
using InventorySystem.DataAccess.Repository.IRepository;
using InventorySystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InventorySystem.DataAccess.Repository
{
    public class CompanyRepository : Repository<Company>, ICompanyRepository
    {
        private readonly ApplicationDbContext _db;

        public CompanyRepository(ApplicationDbContext db):base(db)
        {
            _db = db;
        }
        public void Update(Company company)
        {
            var companyDb = _db.Companies.FirstOrDefault(c => c.Id == company.Id);
            if(companyDb!=null)
            {
                if(company.LogoUrl!=null)
                {
                    companyDb.LogoUrl = company.LogoUrl;
                }
                companyDb.Name = company.Name;
                companyDb.Description = company.Description;
                companyDb.Address = company.Address;
                companyDb.WarehouseSaleId = company.WarehouseSaleId;
                companyDb.Country = company.Country;
                companyDb.City = company.City;
                companyDb.Phone = company.Phone;
            }
        }
    }
}
