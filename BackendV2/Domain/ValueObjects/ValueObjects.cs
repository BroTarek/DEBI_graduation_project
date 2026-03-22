using System;
using YouTubeClone.Domain.Exceptions;

namespace YouTubeClone.Domain.ValueObjects
{
    public record UserId(Guid Value);
    public record ChannelId(Guid Value);
    public record VideoId(Guid Value);
    public record PlaylistId(Guid Value);
    public record CommentId(Guid Value);
    public record SubscriptionId(Guid Value);
    public record WatchHistoryId(Guid Value);
    public record UserInteractionId(Guid Value);

    public record Username(string Value)
    {
        public Username() : this(string.Empty) { }
        public Username(string value)
        {
            if (string.IsNullOrWhiteSpace(value) || value.Length > 50)
                throw new DomainException("Username must be 1-50 characters.");
            Value = value;
        }
    }

    public record Email(string Value)
    {
        public Email() : this(string.Empty) { }
        public Email(string value)
        {
            if (!value.Contains('@')) // simplified
                throw new DomainException("Invalid email format.");
            Value = value;
        }
    }

    public record PasswordHash(string Value);

    public record AvatarUrl(string Value);

    public record ChannelName(string Value)
    {
        public ChannelName() : this(string.Empty) { }
        public ChannelName(string value)
        {
            if (string.IsNullOrWhiteSpace(value) || value.Length > 100)
                throw new DomainException("Channel name must be 1-100 characters.");
            Value = value;
        }
    }

    public record ChannelDescription(string Value);

    public record Title(string Value)
    {
        public Title() : this(string.Empty) { }
        public Title(string value)
        {
            if (string.IsNullOrWhiteSpace(value) || value.Length > 200)
                throw new DomainException("Title must be 1-200 characters.");
            Value = value;
        }
    }

    public record Description(string Value);

    public record Duration(int Seconds)
    {
        public Duration() : this(0) { }
        public Duration(int seconds)
        {
            if (seconds < 0 || seconds > 43200) // max 12 hours
                throw new DomainException("Invalid duration.");
            Seconds = seconds;
        }
    }

    public record ThumbnailUrl(string Value);

    public enum PrivacyStatus { Public, Unlisted, Private }

    public record Category(string Value);

    public record Tag(string Value);

    public record InteractionTarget
    {
        public enum TargetType { Video, Post, Comment }
        public TargetType Type { get; }
        public Guid Id { get; }

        public InteractionTarget(TargetType type, Guid id)
        {
            Type = type;
            Id = id;
        }
    }

    public enum InteractionType { Like, Dislike, Save, Share }
}
