using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YouTubeClone.Domain.Contracts.UOW;
using YouTubeClone.Domain.Aggregates.Users;
using YouTubeClone.Domain.Aggregates.Videos;
using YouTubeClone.Domain.Aggregates.WatchHistories;
using YouTubeClone.Domain.ValueObjects;
using YouTubeClone.Shared.DTOs.WatchHistories;

namespace YouTubeClone.Domain.Services
{
    public class WatchHistoryService : IWatchHistoryService
    {
        private readonly IUnitOfWork _unitOfWork;

        public WatchHistoryService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        private async Task<WatchHistory> GetOrCreateWatchHistoryAsync(Guid ownerId)
        {
            return null;
        }

        public async Task AddToWatchHistoryAsync(Guid videoId, Guid ownerId)
        {
        }

        public async Task RemoveFromWatchHistoryAsync(Guid videoId, Guid ownerId)
        {
        }

        public async Task ClearWatchHistoryAsync(Guid ownerId)
        {
        }

        public async Task<IReadOnlyList<WatchHistoryVideoDto>> GetWatchHistoryAsync(Guid ownerId)
        {
            return new List<WatchHistoryVideoDto>();
        }
    }
}
