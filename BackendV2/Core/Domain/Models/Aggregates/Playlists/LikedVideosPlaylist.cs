using System;

namespace YouTubeClone.Domain.Aggregates.Playlists
{
    public class LikedVideosPlaylist : Playlist
    {
        public string OwnerId { get; private set; }

        // EF Core requires a parameterless constructor
        private LikedVideosPlaylist() { }

        public LikedVideosPlaylist(PlaylistId id, Accessibility accessibility, string ownerId) 
            : base(id, "Liked Videos", "Your liked videos", "", accessibility)
        {
            OwnerId = ownerId;
        }
    }
}
