using YouTubeClone.Domain.Aggregates.Playlists;
using YouTubeClone.Domain.ValueObjects;

namespace YouTubeClone.Services.Specifications.PlaylistSpec
{
    public class PlaylistWithItemsSpecification : SoftBridge.Services.Specification.BaseSpecification<Playlist, PlaylistId>
    {
        public PlaylistWithItemsSpecification(PlaylistId playlistId) : base(p => p.Id == playlistId)
        {
            AddInclude("VideoItems");
        }
    }
}