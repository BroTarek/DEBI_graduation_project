using System;
using System.Collections.Generic;
using YouTubeClone.Domain.Base;
using YouTubeClone.Domain.Enums;
using YouTubeClone.Domain.Entities.Videos;

namespace YouTubeClone.Domain.Entities.Playlists
{
    public abstract class Playlist : Entity<Guid>
    {
        public Accessibility accessibility { get; set; }
        public virtual ICollection<Video> videos { get; set; } = new List<Video>();

        protected Playlist() { }
        protected Playlist(Guid id) : base(id) { }
    }
}
