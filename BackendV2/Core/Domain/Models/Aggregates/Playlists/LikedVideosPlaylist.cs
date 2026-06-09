using System;

namespace YouTubeClone.Domain.Aggregates.Playlists
{
    public class LikedVideosPlaylist : Playlist
    {
        public string OwnerId { get; private set; }

        public LikedVideosPlaylist(PlaylistId id, Accessibility accessibility, string ownerId) 
            : base(id, accessibility)
        {
            OwnerId = ownerId;
        }
    }
}
