using YouTubeClone.Domain.Contracts.Repos;
using YouTubeClone.Domain.Contracts;
using YouTubeClone.Domain.Contracts.Specifications;
using YouTubeClone.Persistance.Contexts;
using YouTubeClone.Persistance.Evaluator;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace YouTubeClone.Persistance.Implements
{
    public class GenericRepoImp<TEntity, TKey> : IGenericRepo<TEntity, TKey> where TEntity : class, IEntity<TKey>
    {
        private readonly YouTubeCloneDbContext _context;

        public GenericRepoImp(YouTubeCloneDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<TEntity>> GetAllAsync()
        {
            return await _context.Set<TEntity>().ToListAsync();
        }

        public async Task<IEnumerable<TEntity>> GetAllWithSpecificationAsync(ISpecifications<TEntity, TKey> specifications)
        {
            return await ApplySpecification(specifications).ToListAsync();
        }

        public async Task<TEntity> GetByIdAsync(TKey id)
        {
            return await _context.Set<TEntity>().FindAsync(id);
        }

        public async Task<TEntity> GetByIdWithSpecificationsAsync(ISpecifications<TEntity, TKey> specifications)
        {
            return await ApplySpecification(specifications).FirstOrDefaultAsync();
        }

        public async Task<int> CountAsync(ISpecifications<TEntity, TKey> specifications)
        {
            return await ApplySpecification(specifications).CountAsync();
        }

        public async Task AddAsync(TEntity entity)
        {
            await _context.Set<TEntity>().AddAsync(entity);
        }

        public async Task UpdateAsync(TEntity entity)
        {
            _context.Set<TEntity>().Update(entity);
            await Task.CompletedTask;
        }

        public async Task DeleteAsync(TEntity entity)
        {
            _context.Set<TEntity>().Remove(entity);
            await Task.CompletedTask;
        }

        public async Task DeleteRangeAsync(IEnumerable<TEntity> entities)
        {
            _context.Set<TEntity>().RemoveRange(entities);
            await Task.CompletedTask;
        }

        private IQueryable<TEntity> ApplySpecification(ISpecifications<TEntity, TKey> specifications)
        {
            return SpecificationEvaluator.GenerateQueery(_context.Set<TEntity>().AsQueryable(), specifications);
        }
    }
}
