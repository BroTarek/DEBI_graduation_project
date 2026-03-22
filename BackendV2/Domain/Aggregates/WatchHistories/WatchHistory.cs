using System;
using YouTubeClone.Domain.Base;
using YouTubeClone.Domain.ValueObjects;
using YouTubeClone.Domain.Exceptions;

namespace YouTubeClone.Domain.Aggregates.WatchHistories
{
    public class WatchHistory : AggregateRoot<WatchHistoryId>
    {
        public UserId UserId { get; private set; }
        public VideoId VideoId { get; private set; }
        public int WatchDurationSeconds { get; private set; }
        public DateTime LastWatchedAt { get; private set; }

        public WatchHistory(WatchHistoryId id, UserId userId, VideoId videoId, int watchDurationSeconds = 0) : base(id)
        {
            UserId = userId;
            VideoId = videoId;
            WatchDurationSeconds = watchDurationSeconds;
            LastWatchedAt = DateTime.UtcNow;
        }

        public void UpdateWatchDuration(int newDurationSeconds)
        {
            if (newDurationSeconds < WatchDurationSeconds)
                throw new DomainException("Cannot decrease watch duration.");
            WatchDurationSeconds = newDurationSeconds;
            LastWatchedAt = DateTime.UtcNow;
        }
    }
}
