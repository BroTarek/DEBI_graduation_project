using System;
using System.Collections.Generic;
using YouTubeClone.Domain.Base;
using YouTubeClone.Domain.ValueObjects;
using YouTubeClone.Domain.Aggregates.Channels;

namespace YouTubeClone.Domain.Aggregates.Users
{
    public class User : AggregateRoot<UserId>
    {
        public Username Username { get; private set; }
        public Email Email { get; private set; }
        public PasswordHash PasswordHash { get; private set; }
        public AvatarUrl AvatarUrl { get; private set; }
        public DateTime CreatedAt { get; private set; }

        private readonly List<Channel> _channels = new();
        public IReadOnlyList<Channel> Channels => _channels.AsReadOnly();

        public User(UserId id, Username username, Email email, PasswordHash passwordHash, AvatarUrl avatarUrl) : base(id)
        {
            Username = username;
            Email = email;
            PasswordHash = passwordHash;
            AvatarUrl = avatarUrl;
            CreatedAt = DateTime.UtcNow;
        }

        public Channel CreateChannel(ChannelId channelId, ChannelName name, ChannelDescription description)
        {
            var channel = new Channel(channelId, this.Id, name, description);
            _channels.Add(channel);
            return channel;
        }
    }
}
