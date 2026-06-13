using System;
using System.Collections.Generic;
using YouTubeClone.Domain.Base;
using YouTubeClone.Domain.ValueObjects;
using YouTubeClone.Domain.Aggregates.Channels;

namespace YouTubeClone.Domain.Aggregates.Subscriptions
{
    public class Subscriptions : AggregateRoot<SubscriptionId>
    {
        public string OwnerId { get; private set; }

        private readonly List<Channel> _channels = new();
        public IReadOnlyList<Channel> Channels => _channels.AsReadOnly();

        // EF Core requires a parameterless constructor
        private Subscriptions() { }

        public Subscriptions(SubscriptionId id, string ownerId) : base(id)
        {
            OwnerId = ownerId;
        }

        public void TrackChannel(Channel channel)
        {
            _channels.Add(channel);
        }

        public void UntrackChannel(Channel channel)
        {
            _channels.Remove(channel);
        }
    }
}
