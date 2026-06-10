using YouTubeClone.Domain.Contracts.Repos;
using YouTubeClone.Domain.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YouTubeClone.Domain.Contracts.UOW
{
    public interface IUnitOfWork
    {
        IGenericRepo<TEntity, TKey> GetRepo<TEntity, TKey>() where TEntity : class, IEntity<TKey>;
        public Task<int> SaveChangesAsync();
    }
}