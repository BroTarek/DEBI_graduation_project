using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using YouTubeClone.Core.Services;
using YouTubeClone.Presentation.Controllers;
using YouTubeClone.Shared.Responses;

namespace YouTubeClone.Presentation.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public class SubscribtionController : BaseController
    {
        private readonly ISubscribtionService _subscribtionService;

        public SubscribtionController(ISubscribtionService subscribtionService)
        {
            _subscribtionService = subscribtionService;
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponse<List<SubscribtionBadgeDTO>>>> GetSubscribtionedChannels(Guid id)
        {
            var result = await _subscribtionService.GetAllSubscribedChannels(id);
            return Ok(new ApiResponse<List<SubscribtionBadgeDTO>>(result, "Subscribed channels retrieved successfully."));
        }  

        [HttpGet("{id}/videos")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponse<List<SubscribedChannelsVideosDTO>>>> GetSubscribedChannelsVideos(Guid id)
        { 
            var result = await _subscribtionService.GetAllSubscribedChannelsVideos(id);
            return Ok(new ApiResponse<List<SubscribedChannelsVideosDTO>>(result, "Subscribed channels videos retrieved successfully."));
        }

        [HttpGet("{id}/posts")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponse<List<SubscribedChannelsPostsDTO>>>> GetSubscribedChannelsPosts(Guid id)
        {
            var result = await _subscribtionService.GetAllSubscribedChannelsPosts(id);
            return Ok(new ApiResponse<List<SubscribedChannelsPostsDTO>>(result, "Subscribed channels posts retrieved successfully."));
        }

        [HttpPost("{id}/subscribe")]
        public async Task<IActionResult> Subscribe(Guid id)
        {
            var userIdStr = GetUserId();
            if (string.IsNullOrEmpty(userIdStr) || !Guid.TryParse(userIdStr, out var userIdGuid))
            {
                return Unauthorized(new ApiResponse<string>("Unauthorized user.", 401));
            }

            await _subscribtionService.AddToSubscribtions(id, userIdGuid);
            return Ok(new ApiResponse<string>("Subscribed successfully."));
        }

        [HttpPost("{id}/unsubscribe")]
        public async Task<IActionResult> Unsubscribe(Guid id)
        {
            var userIdStr = GetUserId();
            if (string.IsNullOrEmpty(userIdStr) || !Guid.TryParse(userIdStr, out var userIdGuid))
            {
                return Unauthorized(new ApiResponse<string>("Unauthorized user.", 401));
            }

            await _subscribtionService.RemoveFromSubscribtions(id, userIdGuid);
            return Ok(new ApiResponse<string>("Unsubscribed successfully."));
        }
    }
}
