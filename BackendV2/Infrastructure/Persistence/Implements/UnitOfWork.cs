using Core.Domain.Contracts.Repos;
using Makanak.Domain.Contracts;
using Makanak.Domain.Contracts.Repos;
using Makanak.Domain.Contracts.UOW;
using Makanak.Persistance.Contexts;
using System;
using System.Collections;
using System.Threading.Tasks;

namespace Makanak.Persistance.Implements
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly MakanakDbContext _context;
        private Hashtable _repositories;

        public UnitOfWork(MakanakDbContext context)
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
