using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace localization.Repository
{
    public interface IUnitOfWork : IDisposable
    {
        IRepository<TEntity> GetRepository<TEntity>() where TEntity : class;
        Task SaveAsync();
    }
}
