using Makanak.Domain.Contracts;

namespace YouTubeClone.Domain.Base
{
    public abstract class Entity<TId> : IEntity<TId>
    {
        public TId Id { get; set; }
        
        protected Entity(TId id) => Id = id;
        protected Entity() { }

        public override bool Equals(object? obj)
        {
            if (obj is not Entity<TId> other) return false;
            if (ReferenceEquals(this, other)) return true;
            return Id!.Equals(other.Id);
        }

        public override int GetHashCode() => Id!.GetHashCode();
    }
}
