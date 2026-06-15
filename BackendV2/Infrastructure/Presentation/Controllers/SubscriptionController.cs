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
    public class SubscriptionController : ControllerBase
    {
        private readonly ISubscriptionService _subscriptionService;

        public SubscriptionController(ISubscriptionService subscriptionService)
        {
            _subscriptionService = subscriptionService;
        }

        [HttpGet("subscriptionPageVideos")]
        public async Task<ActionResult<ApiResponse<Pagination<SubscribedChannelsVideosDTO>>>> GetSubscriptionFeedVideos([FromQuery] QueryParams queryParams)
        {
            var mockUserId = Guid.Parse("22222222-2222-2222-2222-222222222222"); 
            
            var feedResult = await _subscriptionService.GetSubscriptionFeedVideosAsync(queryParams, mockUserId);
            return Ok(new ApiResponse<Pagination<SubscribedChannelsVideosDTO>>(feedResult, "Subscription feed loaded."));
        }

        [HttpPost("toggleSubscription/{channelId}")]
        public async Task<ActionResult<ApiResponse<object>>> ToggleSubscription(string channelId)
        {
            var mockUserId = Guid.Parse("22222222-2222-2222-2222-222222222222");

            bool isSubscribed = await _subscriptionService.ToggleSubscriptionAsync(mockUserId, channelId);
            
            string statusMessage = isSubscribed ? "Successfully subscribed." : "Successfully unsubscribed.";
            return Ok(new ApiResponse<object>(new { IsSubscribed = isSubscribed }, statusMessage));
        }

        [HttpGet("subscribedChannels")]
        public async Task<ActionResult<ApiResponse<Pagination<SubscribedChannelsDTO>>>> GetMySubscribedChannels([FromQuery] QueryParams queryParams)
        {
            var mockUserId = Guid.Parse("22222222-2222-2222-2222-222222222222");

            var channelsResult = await _subscriptionService.GetSubscribedChannelsAsync(queryParams, mockUserId);
            return Ok(new ApiResponse<Pagination<SubscribedChannelsDTO>>(channelsResult, "Subscribed channels list loaded."));
        }
    }
}
