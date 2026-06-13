using YouTubeClone.Domain.Contracts;

namespace YouTubeClone.Domain.Base
{
    public abstract class Entity<TKey> : IEntity<TKey>
    {
        public TKey Id { get; set; }

        protected Entity(TKey id)
        {
            Id = id;
        }

        protected Entity() { }
    }
}
