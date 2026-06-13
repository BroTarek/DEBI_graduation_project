namespace YouTubeClone.Domain.Base
{
    public abstract class AggregateRoot<TKey> : Entity<TKey>
    {
        protected AggregateRoot(TKey id) : base(id)
        {
        }

        protected AggregateRoot() { }
    }
}
