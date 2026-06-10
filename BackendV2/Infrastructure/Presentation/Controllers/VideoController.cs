using YouTubeClone.Presentation.Controllers;
using YouTubeClone.Shared.Responses;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using YouTubeClone.Domain.Services;

using YouTubeClone.Shared.DTOs.WatchHistories;

namespace YouTubeClone.Presentation.Controllers
{
    public class VideoController : BaseController
    {
        private readonly IVideoService _videoService;
        private readonly IWatchHistoryService _watchHistoryService;

        // Controller only depends on the service contract interface
        public VideoController(IVideoService videoService, IWatchHistoryService watchHistoryService)
        {
            _videoService = videoService;
            _watchHistoryService = watchHistoryService;
        }

        [HttpGet("/HomePageVideos")]
        public async Task<IActionResult> GetVideos([FromQuery] int skip = 0, [FromQuery] int take = 10)
        {
            var homePageVideos = await _videoService.GetHomePageVideosAsync(skip, take);
            return Ok(new ApiResponse<List<HomePageVideo>>(homePageVideos));
        }

        [HttpGet("/watch")]
        public async Task<IActionResult> GetVideo([FromQuery] Guid videoId, [FromBody] WatchHistoryRequestDto request = null)
        {
            var videoWatchData = await _videoService.GetWatchPageVideoAsync(videoId);
            
            if (request != null && request.UserId != Guid.Empty)
            {
                await _watchHistoryService.AddToWatchHistoryAsync(videoId, request.UserId);
            }

            if (videoWatchData == null) return NotFound();

            return Ok(new ApiResponse<VideoWatchDto>(videoWatchData));
        }

        [HttpPost("/upload")]
        public async Task<IActionResult> UploadVideo([FromForm] UploadVideoDto dto, [FromQuery] string channelId)
        {
            if (string.IsNullOrEmpty(channelId))
            {
                return BadRequest(new ApiResponse<string>("Channel ID is required."));
            }

            var videoId = await _videoService.UploadVideoAsync(dto, channelId);

            return Ok(new ApiResponse<Guid>(videoId, "Video uploaded successfully."));
        }
    }
}