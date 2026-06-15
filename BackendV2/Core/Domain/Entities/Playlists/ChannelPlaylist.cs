using System;
using YouTubeClone.Domain.Enums;

namespace YouTubeClone.Domain.Entities.Playlists
{
    public class ChannelPlaylist : Playlist
    {
        public Guid channelId { get; set; }
        public string description { get; set; } = string.Empty;

        public ChannelPlaylist() { }
        public ChannelPlaylist(Guid id) : base(id) { }
    }
}
