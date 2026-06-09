using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Makanak.Domain.Contracts.UOW;
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
            var historyRepo = _unitOfWork.GetRepo<WatchHistory, WatchHistoryId>();
            
            // Attempt to find existing history for the owner
            var histories = await historyRepo.FindAsync(wh => wh.Owner.Id.Value == ownerId);
            var history = histories.FirstOrDefault();

            if (history == null)
            {
                var userRepo = _unitOfWork.GetRepo<User, UserId>();
                var owner = await userRepo.GetByIdAsync(new UserId(ownerId));

                // If user exists, create their watch history aggregate
                if (owner != null)
                {
                    history = new WatchHistory(new WatchHistoryId(Guid.NewGuid()), owner);
                    await historyRepo.AddAsync(history);
                }
            }

            return history;
        }

        public async Task AddToWatchHistoryAsync(Guid videoId, Guid ownerId)
        {
            var history = await GetOrCreateWatchHistoryAsync(ownerId);
            if (history == null) return;

            var videoRepo = _unitOfWork.GetRepo<Video, VideoId>();
            var video = await videoRepo.GetByIdAsync(new VideoId(videoId));

            if (video != null && !history.Videos.Any(v => v.Id.Value == videoId))
            {
                history.AddVideo(video);
                await _unitOfWork.SaveChangesAsync();
            }
        }

        public async Task RemoveFromWatchHistoryAsync(Guid videoId, Guid ownerId)
        {
            var history = await GetOrCreateWatchHistoryAsync(ownerId);
            if (history == null) return;

            var video = history.Videos.FirstOrDefault(v => v.Id.Value == videoId);
            if (video != null)
            {
                history.RemoveVideo(video);
                await _unitOfWork.SaveChangesAsync();
            }
        }

        public async Task ClearWatchHistoryAsync(Guid ownerId)
        {
            var history = await GetOrCreateWatchHistoryAsync(ownerId);
            if (history == null) return;

            history.ClearVideos();
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<IReadOnlyList<WatchHistoryVideoDto>> GetWatchHistoryAsync(Guid ownerId)
        {
            var history = await GetOrCreateWatchHistoryAsync(ownerId);
            if (history == null) return new List<WatchHistoryVideoDto>();

            return history.Videos.Select(v => new WatchHistoryVideoDto
            {
                Id = v.Id.Value,
                Title = v.Descriptive?.Title ?? string.Empty,
                ViewCount = v.Stats?.WatchCount ?? 0,
                ThumbnailURL = v.Basics?.ThumbnailUrl ?? string.Empty,
                PublishDate = v.TemporalMetadata?.UploadDate ?? DateTime.UtcNow
            }).ToList();
        }
    }
}
