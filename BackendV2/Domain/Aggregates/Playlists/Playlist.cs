using System;
using System.Collections.Generic;
using System.Linq;
using YouTubeClone.Domain.Base;
using YouTubeClone.Domain.ValueObjects;
using YouTubeClone.Domain.Exceptions;

namespace YouTubeClone.Domain.Aggregates.Playlists
{
    public class Playlist : AggregateRoot<PlaylistId>
    {
        public ChannelId ChannelId { get; private set; }
        public ChannelName Name { get; private set; } // PlaylistName in prompt but ChannelName record was defined or similar, wait
        public Description Description { get; private set; }
        public ThumbnailUrl ThumbnailUrl { get; private set; }
        public bool IsPublic { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime UpdatedAt { get; private set; }

        private readonly List<PlaylistVideoItem> _videoItems = new();
        public IReadOnlyList<PlaylistVideoItem> VideoItems => _videoItems.OrderBy(vi => vi.Position).ToList().AsReadOnly();

        public Playlist(PlaylistId id, ChannelId channelId, ChannelName name, Description description, ThumbnailUrl thumbnailUrl, bool isPublic) : base(id)
        {
            ChannelId = channelId;
            Name = name;
            Description = description;
            ThumbnailUrl = thumbnailUrl;
            IsPublic = isPublic;
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }

        public void AddVideo(VideoId videoId, int position)
        {
            if (_videoItems.Any(v => v.VideoId == videoId))
                throw new DomainException("Video already in playlist.");
            _videoItems.Add(new PlaylistVideoItem(videoId, position));
            UpdatedAt = DateTime.UtcNow;
        }

        public void ReorderVideos(Dictionary<VideoId, int> newPositions)
        {
            foreach (var item in _videoItems)
            {
                if (newPositions.TryGetValue(item.VideoId, out int newPosition))
                {
                    item.UpdatePosition(newPosition);
                }
            }
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
