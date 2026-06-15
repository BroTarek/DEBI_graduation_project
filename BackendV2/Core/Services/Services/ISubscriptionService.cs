using System;
using System.Threading.Tasks;
using YouTubeClone.Shared.Common;
using YouTubeClone.Shared.Common.Params;
using YouTubeClone.Shared.Dto_s;

namespace YouTubeClone.Services
{
    public interface ISubscriptionService
    {
        Task<Pagination<SubscribedChannelsDTO>> GetSubscribedChannelsAsync(QueryParams queryParams, Guid userId);
        Task<Pagination<SubscribedChannelsVideosDTO>> GetSubscriptionFeedVideosAsync(QueryParams queryParams, Guid userId);
        Task<bool> ToggleSubscriptionAsync(Guid userId, string channelId);
    }
}
