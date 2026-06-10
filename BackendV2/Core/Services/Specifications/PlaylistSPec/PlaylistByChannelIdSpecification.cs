using SoftBridge.Services.Specification;
using YouTubeClone.Domain.Aggregates.Playlists;

namespace YouTubeClone.Core.Services.Specifications.PlaylistSpec
{
    public class PlaylistByChannelIdSpecification : BaseSpecification<ChannelPlaylist, PlaylistId>
    {
        public PlaylistByChannelIdSpecification(string channelId) : base(p => p.ChannelId == channelId)
        {
            AddInclude(p => p.Videos);
        }
    }
}
