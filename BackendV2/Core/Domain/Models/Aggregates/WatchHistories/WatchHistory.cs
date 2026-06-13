using System;
using System.Collections.Generic;
using YouTubeClone.Domain.Base;
using YouTubeClone.Domain.ValueObjects;
using YouTubeClone.Domain.Aggregates.Users;
using YouTubeClone.Domain.Aggregates.Videos;

namespace YouTubeClone.Domain.Aggregates.WatchHistories
{
    public class WatchHistory : AggregateRoot<WatchHistoryId>
    {
        public User Owner { get; private set; }

        private readonly List<Video> _videos = new();
        public IReadOnlyList<Video> Videos => _videos.AsReadOnly();

        // EF Core requires a parameterless constructor
        private WatchHistory() { }

        public WatchHistory(WatchHistoryId id, User owner) : base(id)
        {
            Owner = owner;
        }

        public void AddVideo(Video video)
        {
            _videos.Add(video);
        }

        public void RemoveVideo(Video video)
        {
            _videos.Remove(video);
        }

        public void ClearVideos()
        {
            _videos.Clear();
        }
    }
}
