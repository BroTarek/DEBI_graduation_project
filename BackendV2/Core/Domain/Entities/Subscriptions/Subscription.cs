using System;
using YouTubeClone.Domain.Base;
using YouTubeClone.Domain.Entities.Channels;

namespace YouTubeClone.Domain.Entities.Subscriptions
{
    public class Subscription : Entity<Guid>
    {
        public string ownerId { get; set; } = string.Empty;
        public Guid ChannelId { get; set; }
        public virtual Channel Channel { get; set; } = null!;

        public Subscription() { }
        public Subscription(Guid id) : base(id) { }
    }
}
