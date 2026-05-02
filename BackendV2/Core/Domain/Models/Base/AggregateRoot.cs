using System.Collections.Generic;

namespace YouTubeClone.Domain.Base
{
    public abstract class AggregateRoot<TId> : Entity<TId>
    {
        protected AggregateRoot(TId id) : base(id) { }
        protected AggregateRoot() { }
        
        private readonly List<object> _domainEvents = new();
        public IReadOnlyList<object> DomainEvents => _domainEvents.AsReadOnly();

        protected void AddDomainEvent(object domainEvent) => _domainEvents.Add(domainEvent);
        public void ClearDomainEvents() => _domainEvents.Clear();
    }
}
