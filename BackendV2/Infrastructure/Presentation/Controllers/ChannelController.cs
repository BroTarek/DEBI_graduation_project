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
    public class ChannelController : ControllerBase
    {
        private readonly IChannelService _channelService;

        public ChannelController(IChannelService channelService)
        {
            _channelService = channelService;
        }

        [HttpPost("createChannel")]
        public async Task<ActionResult<ApiResponse<ChannelProfileDTO>>> CreateChannel([FromBody] CreateChannelDTO dto)
        {
            var mockCurrentUserId = Guid.Parse("22222222-2222-2222-2222-222222222222");

            var profile = await _channelService.CreateChannelAsync(mockCurrentUserId, dto);
            
            if (profile == null)
            {
                return BadRequest(new ApiResponse<ChannelProfileDTO>("User already registers an active channel or account mapping is corrupted.", 400));
            }

            return Ok(new ApiResponse<ChannelProfileDTO>(profile, "Channel asset instantiated safely under owner domain."));
        }

        [HttpGet("getChannelProfile/{channelId}")]
        public async Task<ActionResult<ApiResponse<ChannelProfileDTO>>> GetChannelProfile(Guid channelId)
        {
            var profile = await _channelService.GetChannelProfileAsync(channelId);
            
            if (profile == null)
            {
                return NotFound(new ApiResponse<ChannelProfileDTO>("Target channel metadata workspace not located.", 404));
            }

            return Ok(new ApiResponse<ChannelProfileDTO>(profile, "Channel profile details extracted cleanly."));
        }

        [HttpGet("getChannelVideos/{channelId}")]
        public async Task<ActionResult<ApiResponse<Pagination<ChannelVideoItemDTO>>>> GetChannelVideos(string channelId, [FromQuery] QueryParams queryParams)
        {
            var paginatedVideos = await _channelService.GetChannelVideosAsync(channelId, queryParams);
            return Ok(new ApiResponse<Pagination<ChannelVideoItemDTO>>(paginatedVideos, "Channel uploaded publications page compiled successfully."));
        }

        [HttpDelete("removeChannel/{channelId}")]
        public async Task<ActionResult<ApiResponse<object>>> RemoveChannel(Guid channelId)
        {
            var success = await _channelService.RemoveChannelAsync(channelId);
            
            if (!success)
            {
                return NotFound(new ApiResponse<object>("Target channel record could not be isolated for processing lifecycle completion.", 404));
            }

            return Ok(new ApiResponse<object>(new { RemovedChannelId = channelId }, "Channel registry and dependent data trees pruned successfully."));
        }
    }
}
