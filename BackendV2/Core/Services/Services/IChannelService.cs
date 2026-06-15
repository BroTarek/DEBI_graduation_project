using System;
using System.Threading.Tasks;
using YouTubeClone.Shared.Common;
using YouTubeClone.Shared.Common.Params;
using YouTubeClone.Shared.Dto_s;

namespace YouTubeClone.Services
{
    public interface IChannelService
    {
        Task<ChannelProfileDTO?> CreateChannelAsync(Guid userId, CreateChannelDTO dto);
        Task<ChannelProfileDTO?> GetChannelProfileAsync(Guid channelId);
        Task<Pagination<ChannelVideoItemDTO>> GetChannelVideosAsync(string channelId, QueryParams queryParams);
        Task<bool> RemoveChannelAsync(Guid channelId);
    }
}
