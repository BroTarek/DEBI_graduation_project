using System;
using YouTubeClone.Domain.ValueObjects;

namespace YouTubeClone.Domain.Aggregates.Playlists
{
    public class ChannelPlaylist : Playlist
    {
        public string ChannelId { get; private set; }

        public ChannelPlaylist(PlaylistId id, string channelId, string name, string description, string thumbnailUrl, Accessibility accessibility) 
            : base(id, name, description, thumbnailUrl, accessibility)
        {
            ChannelId = channelId;
        }
    }
}
