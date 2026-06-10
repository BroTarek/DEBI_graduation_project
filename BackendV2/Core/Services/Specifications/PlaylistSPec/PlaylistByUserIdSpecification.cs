using SoftBridge.Services.Specification;
using YouTubeClone.Domain.Aggregates.Playlists;

namespace YouTubeClone.Core.Services.Specifications.PlaylistSpec
{
    public class PlaylistByUserIdSpecification : BaseSpecification<CustomPlaylist, PlaylistId>
    {
        public PlaylistByUserIdSpecification(string userId) : base(p => p.OwnerId == userId)
        {
            AddInclude(p => p.Videos);
        }
    }
}
