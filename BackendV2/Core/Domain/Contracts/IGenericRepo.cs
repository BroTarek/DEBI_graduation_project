using YouTubeClone.Domain.Contracts.Specifications;
using YouTubeClone.Domain.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YouTubeClone.Domain.Contracts.Repos
{
    public interface IGenericRepo<TEntity, TKey> where TEntity : class, IEntity<TKey>
    {
        public Task<IEnumerable<TEntity>> GetAllAsync();
        public Task<IEnumerable<TEntity>> GetAllWithSpecificationAsync(ISpecifications<TEntity, TKey> specifications);
        public Task<TEntity> GetByIdAsync(TKey id);
        public Task<TEntity> GetByIdWithSpecificationsAsync(ISpecifications<TEntity, TKey> specifications);

        public Task<int> CountAsync(ISpecifications<TEntity, TKey> specifications);
        public Task AddAsync(TEntity entity);
        public Task UpdateAsync(TEntity entity);
        public Task DeleteAsync(TEntity entity);
        public Task DeleteRangeAsync(IEnumerable<TEntity> entities);

    }
}