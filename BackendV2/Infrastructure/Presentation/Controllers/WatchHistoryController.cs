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

            return Ok(new ApiResponse<object>(new { DeletedVideoId = videoId }, "Video successfully cleared from watch history logs."));
        }
    }
}
