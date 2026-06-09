using System;

namespace YouTubeClone.Domain.Aggregates.Playlists
{
    public class ChannelPlaylist : Playlist
    {
        public string ChannelId { get; private set; }
        public string Description { get; private set; }

        public ChannelPlaylist(PlaylistId id, Accessibility accessibility, string channelId, string description) 
            : base(id, accessibility)
        {
            ChannelId = channelId;
            Description = description;
        }
    }
}
