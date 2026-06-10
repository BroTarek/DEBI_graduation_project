using System;
using System.Collections.Generic;
using YouTubeClone.Domain.Base;
using YouTubeClone.Domain.ValueObjects;
using YouTubeClone.Domain.Aggregates.Videos;

namespace YouTubeClone.Domain.Aggregates.Playlists
{
    public abstract class Playlist : AggregateRoot<PlaylistId>
    {
        public string Name { get; protected set; }
        public string Description { get; protected set; }
        public string ThumbnailUrl { get; protected set; }
        public Accessibility Accessibility { get; protected set; }

        private readonly List<Video> _videos = new();
        public IReadOnlyList<Video> Videos => _videos.AsReadOnly();

        protected Playlist(PlaylistId id, string name, string description, string thumbnailUrl, Accessibility accessibility) : base(id)
        {
            Name = name;
            Description = description;
            ThumbnailUrl = thumbnailUrl;
            Accessibility = accessibility;
        }

        public void UpdateDetails(string name, string description, Accessibility accessibility)
        {
            Name = name;
            Description = description;
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
