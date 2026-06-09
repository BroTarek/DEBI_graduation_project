using Makanak.Presentation.Controllers;
using Makanak.Shared.Responses;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using YouTubeClone.Domain.Services;

namespace YouTubeClone.Presentation.Controllers
{
    public class VideoController : BaseController
    {
        private readonly IVideoService _videoService;

        // Controller only depends on the service contract interface
        public VideoController(IVideoService videoService)
        {
            _videoService = videoService;
        }

        [HttpGet("/HomePageVideos")]
        public async Task<IActionResult> GetVideos([FromQuery] int skip = 0, [FromQuery] int take = 10)
        {
            var homePageVideos = await _videoService.GetHomePageVideosAsync(skip, take);
            return Ok(new ApiResponse<List<HomePageVideo>>(homePageVideos));
        }

        [HttpGet("/watch")]
        public async Task<IActionResult> GetVideo([FromQuery] Guid videoId)
        {
            var videoWatchData = await _videoService.GetWatchPageVideoAsync(videoId);
            //var videoWatchData = await _watchHistoryService.addToWatchHistory(videoId);
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