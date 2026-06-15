using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YouTubeClone.Domain.Contracts.UOW;
using YouTubeClone.Domain.Entities.Videos;
using YouTubeClone.Domain.Entities.WatchHistories;
using YouTubeClone.Services.Specifications;
using YouTubeClone.Shared.Common;
using YouTubeClone.Shared.Common.Params;
using YouTubeClone.Shared.Dto_s;

namespace YouTubeClone.Services
{
    public class WatchHistoryService : IWatchHistoryService
    {
        private readonly IUnitOfWork _unitOfWork;

        public WatchHistoryService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Pagination<WatchHistoryVideoDTO>> GetWatchHistoryVideosAsync(QueryParams queryParams, Guid userId)
        {
            var videoRepo = _unitOfWork.GetRepo<Video, Guid>();

            var countSpec = new WatchHistoryVideosSpecification(queryParams, userId);
            int totalCount = await videoRepo.CountAsync(countSpec);

            var dataSpec = new WatchHistoryVideosSpecification(queryParams, userId);
            var videos = await videoRepo.GetAllWithSpecificationAsync(dataSpec);

            var dtos = videos.Select(v => new WatchHistoryVideoDTO
            {
                VideoId = v.video_Basics.VideoId.ToString(),
                VideoUrl = v.video_Basics.videoUrl,
                VideoTitle = v.video_Descriptive.Title,
                VideoLength = v.video_Technical_details.duration
            });

            return new Pagination<WatchHistoryVideoDTO>(queryParams.PageIndex, queryParams.PageSize, totalCount, dtos);
        }

        public async Task<bool> DeleteVideoFromWatchHistoryAsync(Guid userId, string videoId)
        {
            var historyRepo = _unitOfWork.GetRepo<WatchHistory, Guid>();
            
            var historySpec = new WatchHistorySpecification(userId);
            var watchHistory = await historyRepo.GetByIdWithSpecificationsAsync(historySpec);

            if (watchHistory == null || watchHistory.videos == null)
            {
                return false;
            }

            var videoToRemove = watchHistory.videos.FirstOrDefault(v => v.video_Basics.VideoId == Guid.Parse(videoId));
            
            if (videoToRemove == null)
            {
                return false;
            }

            watchHistory.videos.Remove(videoToRemove);
            
            await historyRepo.UpdateAsync(watchHistory);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }
    }
}
