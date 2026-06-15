using System;
using YouTubeClone.Domain.Models;
using SoftBridge.Services.Specification;
using YouTubeClone.Shared.Common.Params;

namespace YouTubeClone.Services.Specifications
{
    public class WatchHistoryVideosSpecification : BaseSpecification<Video, Guid>
    {
        // Constructor to fetch videos belonging to a specific user's Watch History
        public WatchHistoryVideosSpecification(QueryParams query, Guid userId)
            : base(v => v.WatchHistories.Any(wh => wh.owner.Id == userId) &&
                       (string.IsNullOrEmpty(query.Search) || 
                        v.video_Descriptive.Title.ToLower().Contains(query.Search)))
        {
            // Eager load only the specific metadata components needed for the DTO
            AddInclude(v => v.video_Basics);
            AddInclude(v => v.video_Descriptive);
            AddInclude(v => v.video_Technical_details);

            // Order by most recently added to watch history if your join table tracks it, 
            // otherwise default to Title or structural fallback
            AddOrderBy(v => v.video_Descriptive.Title);

            // Standard dynamic paging calculation
            ApplyPaging((query.PageIndex - 1) * query.PageSize, query.PageSize);
        }
    }
}

namespace YouTubeClone.Shared.Dto_s
{
    public class WatchHistoryVideoDTO
    {
        public string VideoId { get; set; } = string.Empty;
        public string VideoUrl { get; set; } = string.Empty;
        public string VideoTitle { get; set; } = string.Empty;
        public int VideoLength { get; set; } // Map from video_Technical_details.duration
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YouTubeClone.Domain.Contracts.UOW;
using YouTubeClone.Domain.Models;
using YouTubeClone.Services.Specifications;
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

            // 1. Get the total count of videos in this user's watch history matching criteria
            var countSpec = new WatchHistoryVideosSpecification(queryParams, userId);
            int totalCount = await videoRepo.CountAsync(countSpec);

            // 2. Fetch the paginated subset of videos
            var dataSpec = new WatchHistoryVideosSpecification(queryParams, userId);
            var videos = await videoRepo.GetAllWithSpecificationAsync(dataSpec);

            // 3. Project into our streamlined DTO payload
            var dtos = videos.Select(v => new WatchHistoryVideoDTO
            {
                VideoId = v.video_Basics.VideoId,
                VideoUrl = v.video_Basics.videoUrl,
                VideoTitle = v.video_Descriptive.Title,
                VideoLength = v.video_Technical_details.duration
            });

            return new Pagination<WatchHistoryVideoDTO>(queryParams.PageIndex, queryParams.PageSize, totalCount, dtos);
        }

        public async Task<bool> DeleteVideoFromWatchHistoryAsync(Guid userId, string videoId)
        {
            var historyRepo = _unitOfWork.GetRepo<WatchHistory, Guid>();
            
            // Fetch the user's WatchHistory entity along with its loaded videos list
            // We use standard string include to access the many-to-many relationship collection
            var historySpec = new WatchHistorySpecification(userId); // Reusing your existing tracking spec base
            var watchHistory = await historyRepo.GetByIdWithSpecificationsAsync(historySpec);

            if (watchHistory == null || watchHistory.videos == null)
            {
                return false;
            }

            // Find the targeted video record inside the collection tracking link
            var videoToRemove = watchHistory.videos.FirstOrDefault(v => v.video_Basics.VideoId == videoId);
            
            if (videoToRemove == null)
            {
                return false; // Video wasn't in their history
            }

            // Remove relation link from the collection tracking mapping array
            watchHistory.videos.Remove(videoToRemove);
            
            await historyRepo.UpdateAsync(watchHistory);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }
    }
}


using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using YouTubeClone.Services;
using YouTubeClone.Shared.Common;
using YouTubeClone.Shared.Common.Params;
using YouTubeClone.Shared.Dto_s;
using YouTubeClone.Shared.Responses;

namespace YouTubeClone.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WatchHistoryController : ControllerBase
    {
        private readonly IWatchHistoryService _watchHistoryService;

        public WatchHistoryController(IWatchHistoryService watchHistoryService)
        {
            _watchHistoryService = watchHistoryService;
        }

        [HttpGet("getWatchHistoryVideos")]
        public async Task<ActionResult<ApiResponse<Pagination<WatchHistoryVideoDTO>>>> GetWatchHistoryVideos([FromQuery] QueryParams queryParams)
        {
            // Simulated User Context extraction
            var mockUserId = Guid.Parse("22222222-2222-2222-2222-222222222222");

            var historyFeed = await _watchHistoryService.GetWatchHistoryVideosAsync(queryParams, mockUserId);
            return Ok(new ApiResponse<Pagination<WatchHistoryVideoDTO>>(historyFeed, "Watch history logs loaded successfully."));
        }

        [HttpDelete("deleteVideoFromWatchHistory/{videoId}")]
        public async Task<ActionResult<ApiResponse<object>>> DeleteVideoFromWatchHistory(string videoId)
        {
            var mockUserId = Guid.Parse("22222222-2222-2222-2222-222222222222");

            bool isDeleted = await _watchHistoryService.DeleteVideoFromWatchHistoryAsync(mockUserId, videoId);

            if (!isDeleted)
            {
                return BadRequest(new ApiResponse<object>("Target video log could not be located or removed.", 400));
            }

            return Ok(new ApiResponse<object>(new { DeletedVideoId = videoId }, "Video successfully cleared from your watch history logs."));
        }
    }
}