using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace localization.Repository
{
    public interface IRepository<T> where T : class
    {
        void Add(T entity);
        void Delete(T entity);
        void Update(T entity);
        T GetById(long Id);
        T GetById(string id);
        //T GetByPublicId(string id);
        IEnumerable<T> All();
        IQueryable<T> AllQueryable();
        IEnumerable<T> Find(Expression<Func<T, bool>> predicate);
    }
}
