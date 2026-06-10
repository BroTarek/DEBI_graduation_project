using Core.Domain.Contracts.Repos;
using YouTubeClone.Domain.Contracts;
using YouTubeClone.Domain.Contracts.Repos;
using YouTubeClone.Domain.Contracts.UOW;
using YouTubeClone.Persistance.Contexts;
using System;
using System.Collections;
using System.Threading.Tasks;

namespace YouTubeClone.Persistance.Implements
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly YouTubeCloneDbContext _context;
        private Hashtable _repositories;

        public UnitOfWork(YouTubeCloneDbContext context)
        {
            _context = context;
        }

        public IGenericRepo<TEntity, TKey> GetRepo<TEntity, TKey>() where TEntity : class, IEntity<TKey>
        {
            if (_repositories == null)
                _repositories = new Hashtable();

            var type = typeof(TEntity).Name;

            if (!_repositories.ContainsKey(type))
            {
                var repositoryType = typeof(GenericRepoImp<,>);
                var repositoryInstance = Activator.CreateInstance(repositoryType.MakeGenericType(typeof(TEntity), typeof(TKey)), _context);

                _repositories.Add(type, repositoryInstance);
            }

            return (IGenericRepo<TEntity, TKey>)_repositories[type];
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}
