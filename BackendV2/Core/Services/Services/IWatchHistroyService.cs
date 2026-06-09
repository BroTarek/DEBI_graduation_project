using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using YouTubeClone.Shared.DTOs.WatchHistories;

namespace YouTubeClone.Domain.Services
{
    public interface IWatchHistoryService
    {
        Task AddToWatchHistoryAsync(Guid videoId, Guid ownerId);
        Task RemoveFromWatchHistoryAsync(Guid videoId, Guid ownerId);
        Task ClearWatchHistoryAsync(Guid ownerId);
        Task<IReadOnlyList<WatchHistoryVideoDto>> GetWatchHistoryAsync(Guid ownerId);
    }
}
