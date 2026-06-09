using System;
using System.Collections.Generic;
using YouTubeClone.Domain.Base;
using YouTubeClone.Domain.ValueObjects;
using YouTubeClone.Domain.Aggregates.Videos;

namespace YouTubeClone.Domain.Aggregates.Playlists
{
    public abstract class Playlist : AggregateRoot<PlaylistId>
    {
        public Accessibility Accessibility { get; private set; }

        private readonly List<Video> _videos = new();
        public IReadOnlyList<Video> Videos => _videos.AsReadOnly();

        protected Playlist(PlaylistId id, Accessibility accessibility) : base(id)
        {
            Accessibility = accessibility;
        }

        public void AddVideo(Video video)
        {
            _videos.Add(video);
        }

        public void RemoveVideo(Video video)
        {
            _videos.Remove(video);
        }
    }
}
