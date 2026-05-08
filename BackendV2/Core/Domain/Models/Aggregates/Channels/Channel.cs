using System;
using YouTubeClone.Domain.Base;
using YouTubeClone.Domain.ValueObjects;

namespace YouTubeClone.Domain.Aggregates.Channels
{
    public class Channel : AggregateRoot<ChannelId>
    {
        public UserId OwnerId { get; private set; }
        public ChannelName Name { get; private set; }
        public ChannelDescription Description { get; private set; }
        public DateTime CreatedAt { get; private set; }

        public Channel(ChannelId id, UserId ownerId, ChannelName name, ChannelDescription description) : base(id)
        {
            OwnerId = ownerId;
            Name = name;
            Description = description;
            CreatedAt = DateTime.UtcNow;
        }

        public void UpdateDetails(ChannelName name, ChannelDescription description)
        {
            Name = name;
            Description = description;
        }

    }
}
