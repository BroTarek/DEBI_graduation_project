
namespace YouTubeClone.Shared.Dto_s
{
    public class SubscribedChannelsDTO
    {
        public string ChannelId { get; set; } = string.Empty;
        public string ChannelName { get; set; } = string.Empty;
        public string ChannelAvatar { get; set; } = string.Empty;
        public int SubscribersCount { get; set; }
    }

    public class SubscribedChannelsVideosDTO
    {
        public string VideoId { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string ThumbnailUrl { get; set; } = string.Empty;
        public string VideoUrl { get; set; } = string.Empty;
        public int Duration { get; set; }
        public int WatchCount { get; set; }
        public string ChannelName { get; set; } = string.Empty;
        public string ChannelAvatar { get; set; } = string.Empty;
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using YouTubeClone.Domain.Models;
using SoftBridge.Services.Specification;
using YouTubeClone.Shared.Common.Params;

namespace YouTubeClone.Services.Specifications
{
    public class SubscribedChannelsVideosSpecification : BaseSpecification<Video, Guid>
    {
        public SubscribedChannelsVideosSpecification(QueryParams query, List<string> followedChannelIds)
            : base(v => followedChannelIds.Contains(v.channelId) && 
                       v.video_Basics.PrivacyStatus == Accessibility.PUBLIC &&
                       (string.IsNullOrEmpty(query.Search) || 
                        v.video_Descriptive.Title.ToLower().Contains(query.Search)))
        {
            AddInclude(v => v.video_Basics);
            AddInclude(v => v.video_Descriptive);
            AddInclude(v => v.VideoStats);
            AddInclude("Channel.ChannelProfile");

            // Default sort: Show freshest subscription videos first
            AddOrderByDescending(v => v.Temporal_Metadata.UploadDate);

            ApplyPaging((query.PageIndex - 1) * query.PageSize, query.PageSize);
        }
    }
}

using System;
using YouTubeClone.Domain.Models;
using SoftBridge.Services.Specification;
using YouTubeClone.Shared.Common.Params;

namespace YouTubeClone.Services.Specifications
{
    public class SubscriptionSpecification : BaseSpecification<Subscription, Guid>
    {
        public SubscriptionSpecification(QueryParams query, Guid userId)
            : base(s => s.ownerId == userId.ToString() && 
                       (string.IsNullOrEmpty(query.Search) || 
                        s.Channel.ChannelProfile.name.ToLower().Contains(query.Search)))
        {
            // Pull in the profile for UI presentation details
            AddInclude("Channel.ChannelProfile");
            
            // Standard dynamic paging translation
            ApplyPaging((query.PageIndex - 1) * query.PageSize, query.PageSize);
        }
        
        // Lightweight overload to check a single relationship existence (for Toggling)
        public SubscriptionSpecification(Guid userId, string channelId)
            : base(s => s.ownerId == userId.ToString() && s.ChannelId == channelId)
        {
        }
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
    public interface ISubscriptionService
    {
        Task<Pagination<SubscribedChannelsDTO>> GetSubscribedChannelsAsync(QueryParams queryParams, Guid userId);
        Task<Pagination<SubscribedChannelsVideosDTO>> GetSubscriptionFeedVideosAsync(QueryParams queryParams, Guid userId);
        Task<bool> ToggleSubscriptionAsync(Guid userId, string channelId);
    }

    public class SubscriptionService : ISubscriptionService
    {
        private readonly IUnitOfWork _unitOfWork;

        public SubscriptionService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Pagination<SubscribedChannelsDTO>> GetSubscribedChannelsAsync(QueryParams queryParams, Guid userId)
        {
            var subRepo = _unitOfWork.GetRepo<Subscription, Guid>();
            
            var specForCount = new SubscriptionSpecification(queryParams, userId);
            int totalCount = await subRepo.CountAsync(specForCount);

            var specForData = new SubscriptionSpecification(queryParams, userId);
            var subscriptions = await subRepo.GetAllWithSpecificationAsync(specForData);

            var dtos = subscriptions.Select(s => new SubscribedChannelsDTO
            {
                ChannelId = s.ChannelId,
                ChannelName = s.Channel?.ChannelProfile?.name ?? string.Empty,
                ChannelAvatar = s.Channel?.ChannelProfile?.avatar ?? string.Empty,
                SubscribersCount = s.Channel?.ChannelProfile?.subscribersCount ?? 0
            });

            return new Pagination<SubscribedChannelsDTO>(queryParams.PageIndex, queryParams.PageSize, totalCount, dtos);
        }

        public async Task<Pagination<SubscribedChannelsVideosDTO>> GetSubscriptionFeedVideosAsync(QueryParams queryParams, Guid userId)
        {
            var subRepo = _unitOfWork.GetRepo<Subscription, Guid>();
            var videoRepo = _unitOfWork.GetRepo<Video, Guid>();

            // 1. Find all channel IDs this user follows (Unpaginated check to build our list filter)
            var cleanSubCheckSpec = new SubscriptionSpecification(new QueryParams { PageSize = 1000 }, userId);
            var userSubs = await subRepo.GetAllWithSpecificationAsync(cleanSubCheckSpec);
            var followedChannelIds = userSubs.Select(s => s.ChannelId).ToList();

            if (!followedChannelIds.Any())
            {
                return new Pagination<SubscribedChannelsVideosDTO>(queryParams.PageIndex, queryParams.PageSize, 0, Enumerable.Empty<SubscribedChannelsVideosDTO>());
            }

            // 2. Query against our dedicated video specification matching followed content creators
            var videoCountSpec = new SubscribedChannelsVideosSpecification(queryParams, followedChannelIds);
            int totalCount = await videoRepo.CountAsync(videoCountSpec);

            var videoDataSpec = new SubscribedChannelsVideosSpecification(queryParams, followedChannelIds);
            var videos = await videoRepo.GetAllWithSpecificationAsync(videoDataSpec);

            var dtos = videos.Select(v => new SubscribedChannelsVideosDTO
            {
                VideoId = v.video_Basics.VideoId,
                Title = v.video_Descriptive.Title,
                ThumbnailUrl = v.video_Basics.ThumbnailUrl,
                VideoUrl = v.video_Basics.videoUrl,
                Duration = v.video_Technical_details.duration,
                WatchCount = v.VideoStats.watchCount,
                ChannelName = v.Channel?.ChannelProfile?.name ?? string.Empty,
                ChannelAvatar = v.Channel?.ChannelProfile?.avatar ?? string.Empty
            });

            return new Pagination<SubscribedChannelsVideosDTO>(queryParams.PageIndex, queryParams.PageSize, totalCount, dtos);
        }

        public async Task<bool> ToggleSubscriptionAsync(Guid userId, string channelId)
        {
            var subRepo = _unitOfWork.GetRepo<Subscription, Guid>();
            var channelRepo = _unitOfWork.GetRepo<Channel, Guid>();

            // Verify channel target exists
            var targetChannel = (await channelRepo.GetAllAsync()).FirstOrDefault(c => c.Id.ToString() == channelId);
            if (targetChannel == null) return false;

            var checkSpec = new SubscriptionSpecification(userId, channelId);
            var existingSub = await subRepo.GetByIdWithSpecificationsAsync(checkSpec);

            bool isSubscribedNow;

            if (existingSub != null)
            {
                // Unsubscribe action
                await subRepo.DeleteAsync(existingSub);
                targetChannel.ChannelProfile.subscribersCount = Math.Max(0, targetChannel.ChannelProfile.subscribersCount - 1);
                isSubscribedNow = false;
            }
            else
            {
                // Subscribe action
                var newSub = new Subscription
                {
                    ownerId = userId.ToString(),
                    ChannelId = channelId
                };
                await subRepo.AddAsync(newSub);
                targetChannel.ChannelProfile.subscribersCount++;
                isSubscribedNow = true;
            }

            await channelRepo.UpdateAsync(targetChannel);
            await _unitOfWork.SaveChangesAsync();
            
            return isSubscribedNow;
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
            // Simulated User ID until Authentication Context extraction filters are configured
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