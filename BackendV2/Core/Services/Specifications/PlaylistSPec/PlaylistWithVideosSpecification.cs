using SoftBridge.Services.Specification;
using YouTubeClone.Domain.Aggregates.Playlists;

namespace YouTubeClone.Core.Services.Specifications.PlaylistSpec
{
    public class PlaylistWithVideosSpecification : BaseSpecification<Playlist, PlaylistId>
    {
        public PlaylistWithVideosSpecification(PlaylistId playlistId) : base(p => p.Id.Value == playlistId.Value)
        {
            AddInclude(p => p.Videos);
        }
    }
}
