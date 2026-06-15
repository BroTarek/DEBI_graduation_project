using System;
using System.Linq;
using YouTubeClone.Domain.Entities.Playlists;
using SoftBridge.Services.Specification;
using YouTubeClone.Shared.Common.Params;
using YouTubeClone.Shared.Common;

namespace YouTubeClone.Services.Specifications
{
    public class PlaylistWithVideosSpecification : BaseSpecification<Playlist, Guid>
    {
        public PlaylistWithVideosSpecification(Guid playlistId, QueryParams queryParams)
            : base(p => p.Id == playlistId)
        {
            AddInclude("videos.video_Basics");
            AddInclude("videos.video_Descriptive");
            AddInclude("videos.Temporal_Metadata");

            if (queryParams.Sort == SortingOptionsEnum.DateCreatedAsc)
            {
                AddOrderBy(p => p.videos.Select(v => v.Temporal_Metadata.UploadDate));
            }
            else
            {
                AddOrderByDescending(p => p.videos.Select(v => v.Temporal_Metadata.UploadDate));
            }
        }
    }

    public class OwnerPlaylistsSpecification : BaseSpecification<Playlist, Guid>
    {
        public OwnerPlaylistsSpecification(string ownerId, string label)
            : base(p => (label.ToLower() == "channel" && p is ChannelPlaylist && ((ChannelPlaylist)p).channelId == Guid.Parse(ownerId)) ||
                       (label.ToLower() == "custom" && p is CustomPlaylist && ((CustomPlaylist)p).ownerId == ownerId))
        {
            AddInclude("videos.video_Basics");
        }
    }
}
