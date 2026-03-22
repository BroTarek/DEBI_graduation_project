using System;
using YouTubeClone.Domain.Base;
using YouTubeClone.Domain.ValueObjects;

namespace YouTubeClone.Domain.Aggregates.Playlists
{
    public class PlaylistVideoItem : Entity<VideoId> 
    {
        // Using VideoId as identification since it's unique within a playlist context, 
        // or a synthetic GUID. The base Entity<TId> expects an ID.
        public VideoId VideoId => Id;
        public int Position { get; private set; }

        public PlaylistVideoItem(VideoId videoId, int position) : base(videoId)
        {
            Position = position;
        }

        internal void UpdatePosition(int newPosition)
        {
            Position = newPosition;
        }
    }
}
