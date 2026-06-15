using System;

namespace YouTubeClone.Domain.Entities.Playlists
{
    public class CustomPlaylist : Playlist
    {
        public string name { get; set; } = string.Empty;
        public string ownerId { get; set; } = string.Empty;

        public CustomPlaylist() { }
        public CustomPlaylist(Guid id) : base(id) { }
    }
}
