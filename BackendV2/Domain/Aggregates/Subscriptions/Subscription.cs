using System;
using YouTubeClone.Domain.Base;
using YouTubeClone.Domain.ValueObjects;

namespace YouTubeClone.Domain.Aggregates.Subscriptions
{
    public class Subscription : AggregateRoot<SubscriptionId>
    {
        public UserId SubscriberId { get; private set; }
        public ChannelId ChannelId { get; private set; }
        public DateTime CreatedAt { get; private set; }

        public Subscription(SubscriptionId id, UserId subscriberId, ChannelId channelId) : base(id)
        {
            SubscriberId = subscriberId;
            ChannelId = channelId;
            CreatedAt = DateTime.UtcNow;
        }
    }
}
