using System;
using System.Collections.Generic;
using YouTubeClone.Domain.Base;
using YouTubeClone.Domain.Models.Identity;
using YouTubeClone.Domain.Entities.Playlists;
using YouTubeClone.Domain.Entities.Videos;

namespace YouTubeClone.Domain.Entities.Channels
{
    public class Channel : Entity<Guid>
    {
        public string OwnerId { get; set; } = string.Empty;
        public virtual ApplicationUser Owner { get; set; } = null!;
        public ChannelProfile ChannelProfile { get; set; } = null!;

        public virtual ICollection<Video> videos { get; set; } = new List<Video>();
        public virtual ICollection<Post> posts { get; set; } = new List<Post>();
        public virtual ICollection<ChannelPlaylist> channelPlaylists { get; set; } = new List<ChannelPlaylist>();

        public Channel() { }
        public Channel(Guid id) : base(id) { }
    }
}
