using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Linq.Expressions;

namespace InventorySystem.DataAccess.Repository.IRepository
{
    public interface IRepository<T> where T : class
    {
        T Get(int id);

        IEnumerable<T> GetAll(
            Expression<Func<T, bool>> filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
            string IncludeProperties = null
        );

        T GetFirts(
            Expression<Func<T, bool>> filter = null,
            string IncludeProperties = null
        );

        void Add(T entity);

        void Remove(int id);

        void Remove(T entity);

        void RemoveRank(IEnumerable<T> entity);
    }
}
