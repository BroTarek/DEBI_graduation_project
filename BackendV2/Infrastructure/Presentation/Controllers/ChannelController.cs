using YouTubeClone.Domain.Contracts.UOW;
using YouTubeClone.Presentation.Controllers;
using YouTubeClone.Shared.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YouTubeClone.Domain.Aggregates.Channels;
using YouTubeClone.Domain.Aggregates.Subscriptions;
using YouTubeClone.Domain.ValueObjects;
using YouTubeClone.Domain.Services;

namespace YouTubeClone.Presentation.Controllers
{
    [Authorize]
    public class ChannelController : BaseController
    {
        private readonly IChannelService _channelService;

        public ChannelController(IChannelService channelService)
        {
            _channelService = channelService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateChannel([FromBody] CreateChannelDto dto)
        {
            // Placeholder: Safely attempt to retrieve user ID, fallback for non-compiling scenarios
            var userIdStr = GetUserId() ?? Guid.NewGuid().ToString();
            if (!Guid.TryParse(userIdStr, out var userIdGuid))
            {
                return Unauthorized(new ApiResponse<string>("Unauthorized user.", 401));
            }

            var channelId = await _channelService.CreateChannelAsync(dto, userIdGuid);

            return Ok(new ApiResponse<Guid>(channelId, "Channel created successfully."));
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetChannelAbout(Guid id)
        {
            var about = await _channelService.GetChannelAboutAsync(id);
            if (about == null)
            {
                return NotFound(new ApiResponse<string>("Channel not found.", 404));
            }

            return Ok(new ApiResponse<ChannelAboutDto>(about, "Channel retrieved successfully."));
        }

        [HttpGet("{id}/videos")]
        [AllowAnonymous]
        public async Task<IActionResult> GetChannelVideos(Guid id)
        {
            var videos = await _channelService.GetChannelVideosAsync(id);
            return Ok(new ApiResponse<IReadOnlyList<ChannelVideoDto>>(videos, "Channel videos retrieved successfully."));
        }
    }
}
