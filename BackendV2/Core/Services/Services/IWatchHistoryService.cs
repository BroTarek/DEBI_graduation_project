using System;
using System.Threading.Tasks;
using YouTubeClone.Shared.Common;
using YouTubeClone.Shared.Common.Params;
using YouTubeClone.Shared.Dto_s;

namespace YouTubeClone.Services
{
    public interface IWatchHistoryService
    {
        Task<Pagination<WatchHistoryVideoDTO>> GetWatchHistoryVideosAsync(QueryParams queryParams, Guid userId);
        Task<bool> DeleteVideoFromWatchHistoryAsync(Guid userId, string videoId);
    }
}
