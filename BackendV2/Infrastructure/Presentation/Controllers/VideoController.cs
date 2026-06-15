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
    public class VideoController : ControllerBase
    {
        private readonly IVideoService _videoService;

        public VideoController(IVideoService videoService)
        {
            _videoService = videoService;
        }

        [HttpGet("homePageVideos")]
        public async Task<ActionResult<ApiResponse<Pagination<HomePageVideoDTO>>>> GetHomePageVideos([FromQuery] QueryParams queryParams)
        {
            var result = await _videoService.GetHomePageVideosAsync(queryParams);
            return Ok(new ApiResponse<Pagination<HomePageVideoDTO>>(result, "Home page videos loaded successfully."));
        }

        [HttpPost("watchVideo/{videoId}")]
        public async Task<ActionResult<ApiResponse<WatchVideoDetailDTO>>> WatchVideo(Guid videoId)
        {
            var mockCurrentUserId = Guid.Parse("22222222-2222-2222-2222-222222222222"); 
            
            var videoDetails = await _videoService.WatchVideoAsync(videoId, mockCurrentUserId);
            
            if (videoDetails == null)
            {
                return NotFound(new ApiResponse<WatchVideoDetailDTO>("Requested video could not be found.", 404));
            }

            return Ok(new ApiResponse<WatchVideoDetailDTO>(videoDetails, "Video metadata package compiled successfully."));
        }

        [HttpPost("uploadVideo")]
        public async Task<ActionResult<ApiResponse<Guid>>> UploadVideo([FromForm] UploadVideoDto dto, [FromQuery] string channelId)
        {
            var res = await _videoService.UploadVideoAsync(dto, channelId);
            return Ok(new ApiResponse<Guid>(res, "Video uploaded successfully."));
        }
    }
}