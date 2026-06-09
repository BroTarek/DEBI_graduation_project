using Makanak.Presentation.Controllers;
using Makanak.Shared.Responses;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using YouTubeClone.Domain.Services;
using YouTubeClone.Shared.DTOs.WatchHistories;

namespace YouTubeClone.Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WatchHistoryController : BaseController
    {
        private readonly IWatchHistoryService _watchHistoryService;

        public WatchHistoryController(IWatchHistoryService watchHistoryService)
        {
            _watchHistoryService = watchHistoryService;
        }

        [HttpPost("getWatchHistroy")]
        public async Task<IActionResult> GetWatchHistoryVideos([FromBody] WatchHistoryRequestDto request)
        {
            if (request == null || request.UserId == Guid.Empty)
                return BadRequest(new ApiResponse<string>("UserId is required.", 400));

            var history = await _watchHistoryService.GetWatchHistoryAsync(request.UserId);
            return Ok(new ApiResponse<IReadOnlyList<WatchHistoryVideoDto>>(history, "Watch history retrieved successfully."));
        }

        [HttpPost("clearWatchHistroyVideo")]
        public async Task<IActionResult> ClearWatchHistoryVideo([FromBody] WatchHistoryRequestDto request)
        {
            if (request == null || request.UserId == Guid.Empty)
                return BadRequest(new ApiResponse<string>("UserId is required.", 400));

            await _watchHistoryService.ClearWatchHistoryAsync(request.UserId);
            return Ok(new ApiResponse<string>("Watch history cleared successfully."));
        }
        
        [HttpPost("deleteFromWatchHistory")]
        public async Task<IActionResult> DeleteFromWatchHistory([FromQuery] Guid videoId, [FromBody] WatchHistoryRequestDto request)
        {
            if (request == null || request.UserId == Guid.Empty)
                return BadRequest(new ApiResponse<string>("UserId is required.", 400));

            if (videoId == Guid.Empty)
                return BadRequest(new ApiResponse<string>("VideoId is required.", 400));

            await _watchHistoryService.RemoveFromWatchHistoryAsync(videoId, request.UserId);
            return Ok(new ApiResponse<string>("Video removed from watch history."));
        }
    }
}