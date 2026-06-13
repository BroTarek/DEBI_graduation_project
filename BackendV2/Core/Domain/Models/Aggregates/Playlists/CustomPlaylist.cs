using System;
using YouTubeClone.Domain.ValueObjects;

namespace YouTubeClone.Domain.Aggregates.Playlists
{
    public class CustomPlaylist : Playlist
    {
        public string OwnerId { get; private set; }

        // EF Core requires a parameterless constructor
        private CustomPlaylist() { }

        public CustomPlaylist(PlaylistId id, string ownerId, string name, string description, string thumbnailUrl, Accessibility accessibility) 
            : base(id, name, description, thumbnailUrl, accessibility)
        {
            OwnerId = ownerId;
        }
    }
}
