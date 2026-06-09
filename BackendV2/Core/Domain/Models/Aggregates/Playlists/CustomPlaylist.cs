using System;

namespace YouTubeClone.Domain.Aggregates.Playlists
{
    public class CustomPlaylist : Playlist
    {
        public string Name { get; private set; }
        public string OwnerId { get; private set; }

        public CustomPlaylist(PlaylistId id, Accessibility accessibility, string name, string ownerId) 
            : base(id, accessibility)
        {
            Name = name;
            OwnerId = ownerId;
        }
    }
}
