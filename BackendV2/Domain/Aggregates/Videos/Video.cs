using System;
using System.Collections.Generic;
using YouTubeClone.Domain.Base;
using YouTubeClone.Domain.ValueObjects;
using YouTubeClone.Domain.Exceptions;

namespace YouTubeClone.Domain.Aggregates.Videos
{
    public class Video : AggregateRoot<VideoId>
    {
        public ChannelId ChannelId { get; private set; }
        public Title Title { get; private set; }
        public Description Description { get; private set; }
        public Duration Duration { get; private set; }
        public ThumbnailUrl ThumbnailUrl { get; private set; }
        public PrivacyStatus PrivacyStatus { get; private set; }
        public Category Category { get; private set; }
        public List<Tag> Tags { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime UpdatedAt { get; private set; }
        public int ViewCount { get; private set; }

        private readonly List<Comment> _comments = new();
        public IReadOnlyList<Comment> Comments => _comments.AsReadOnly();

        public Video(VideoId id, ChannelId channelId, Title title, Description description, Duration duration, ThumbnailUrl thumbnailUrl, PrivacyStatus privacyStatus, Category category, List<Tag> tags) : base(id)
        {
            ChannelId = channelId;
            Title = title;
            Description = description;
            Duration = duration;
            ThumbnailUrl = thumbnailUrl;
            PrivacyStatus = privacyStatus;
            Category = category;
            Tags = tags;
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }

        public void ChangePrivacy(PrivacyStatus newStatus, UserId requestingUserId, UserId channelOwnerId)
        {
            if (requestingUserId != channelOwnerId)
                throw new DomainException("Only channel owner can change video privacy.");
            PrivacyStatus = newStatus;
            UpdatedAt = DateTime.UtcNow;
        }

        public void AddComment(Comment comment)
        {
            _comments.Add(comment);
            // Raise domain event: VideoCommentAdded
            // AddDomainEvent(new VideoCommentAdded(comment.Id));
        }

        public void IncrementViewCount()
        {
            ViewCount++;
        }
    }
}
