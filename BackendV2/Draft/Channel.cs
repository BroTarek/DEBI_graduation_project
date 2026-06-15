using System;
using YouTubeClone.Domain.Models;
using SoftBridge.Services.Specification;
using YouTubeClone.Shared.Common.Params;

namespace YouTubeClone.Services.Specifications
{
    public class ChannelVideosSpecification : BaseSpecification<Video, Guid>
    {
        public ChannelVideosSpecification(string channelId, QueryParams query)
            : base(v => v.channelId == channelId && 
                       v.video_Basics.PrivacyStatus == Accessibility.PUBLIC &&
                       (string.IsNullOrEmpty(query.Search) || 
                        v.video_Descriptive.Title.ToLower().Contains(query.Search)))
        {
            // Load components needed for the target DTO
            AddInclude(v => v.video_Basics);
            AddInclude(v => v.video_Descriptive);
            AddInclude(v => v.VideoStats);
            AddInclude(v => v.Temporal_Metadata);

            // Default order: Newest uploads first
            AddOrderByDescending(v => v.Temporal_Metadata.UploadDate);

            // Apply standard architecture pagination math
            ApplyPaging((query.PageIndex - 1) * query.PageSize, query.PageSize);
        }
    }
}

using System;
using YouTubeClone.Domain.Models;
using SoftBridge.Services.Specification;

namespace YouTubeClone.Services.Specifications
{
    public class ChannelProfileSpecification : BaseSpecification<Channel, Guid>
    {
        // Find by unique channel ID
        public ChannelProfileSpecification(Guid channelId)
            : base(c => c.Id == channelId)
        {
            AddInclude(c => c.ChannelProfile);
        }

        // Find by unique User owner context ID
        public ChannelProfileSpecification(string ownerUserId)
            : base(c => c.Owner.Id == ownerUserId)
        {
            AddInclude(c => c.ChannelProfile);
        }
    }
}

using System;

namespace YouTubeClone.Shared.Dto_s
{
    public class CreateChannelDTO
    {
        public string Name { get; set; } = string.Empty;
        public string ChannelsDescription { get; set; } = string.Empty;
        public string? Avatar { get; set; } // Nullable fields
        public string? GreaterImg { get; set; } // Thumbnail banner path
        public string Links { get; set; } = string.Empty;
    }

    public class ChannelProfileDTO
    {
        public string ChannelId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string ChannelsDescription { get; set; } = string.Empty;
        public string Avatar { get; set; } = string.Empty;
        public string GreaterImg { get; set; } = string.Empty;
        public string Links { get; set; } = string.Empty;
        public int SubscribersCount { get; set; }
    }

    public class ChannelVideoItemDTO
    {
        public string VideoId { get; set; } = string.Empty;
        public string VideoTitle { get; set; } = string.Empty;
        public string ThumbnailUrl { get; set; } = string.Empty;
        public string VideoUrl { get; set; } = string.Empty;
        public int ViewCount { get; set; }
        public DateTime UploadDate { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YouTubeClone.Domain.Contracts.UOW;
using YouTubeClone.Domain.Models;
using YouTubeClone.Domain.Models.Identity;
using YouTubeClone.Services.Specifications;
using YouTubeClone.Shared.Common;
using YouTubeClone.Shared.Common.Params;
using YouTubeClone.Shared.Dto_s;

namespace YouTubeClone.Services
{
    public interface IChannelService
    {
        Task<ChannelProfileDTO?> CreateChannelAsync(Guid userId, CreateChannelDTO dto);
        Task<ChannelProfileDTO?> GetChannelProfileAsync(Guid channelId);
        Task<Pagination<ChannelVideoItemDTO>> GetChannelVideosAsync(string channelId, QueryParams queryParams);
        Task<bool> RemoveChannelAsync(Guid channelId);
    }

    public class ChannelService : IChannelService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ChannelService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ChannelProfileDTO?> CreateChannelAsync(Guid userId, CreateChannelDTO dto)
        {
            var channelRepo = _unitOfWork.GetRepo<Channel, Guid>();
            var userRepo = _unitOfWork.GetRepo<ApplicationUser, string>(); // Identity storage context

            // 1. Guard check: Ensure user doesn't already own an active channel
            var checkSpec = new ChannelProfileSpecification(userId.ToString());
            var existingChannel = await channelRepo.GetByIdWithSpecificationsAsync(checkSpec);
            if (existingChannel != null) return null; // Conflict handling

            var ownerUser = await userRepo.GetByIdAsync(userId.ToString());
            if (ownerUser == null) return null;

            // 2. Map structural components from your UML composition rules
            var newChannel = new Channel
            {
                Owner = ownerUser,
                ChannelProfile = new ChannelProfile
                {
                    name = dto.Name,
                    channelsDescription = dto.ChannelsDescription,
                    links = dto.Links,
                    subscribersCount = 0,
                    // Fallbacks applied dynamically if incoming parameters are null
                    avatar = dto.Avatar ?? $"https://api.dicebear.com/7.x/initials/svg?seed={Uri.EscapeDataString(dto.Name)}",
                    greaterImg = dto.GreaterImg ?? "https://images.unsplash.com/photo-1618005182384-a83a8bd57fbe"
                },
                videos = new List<Video>(),
                posts = new List<Post>(),
                channelPlaylists = new List<ChannelPlaylist>()
            };

            await channelRepo.AddAsync(newChannel);
            await _unitOfWork.SaveChangesAsync();

            return new ChannelProfileDTO
            {
                ChannelId = newChannel.Id.ToString(),
                Name = newChannel.ChannelProfile.name,
                ChannelsDescription = newChannel.ChannelProfile.channelsDescription,
                Avatar = newChannel.ChannelProfile.avatar,
                GreaterImg = newChannel.ChannelProfile.greaterImg,
                Links = newChannel.ChannelProfile.links,
                SubscribersCount = 0
            };
        }

        public async Task<ChannelProfileDTO?> GetChannelProfileAsync(Guid channelId)
        {
            var channelRepo = _unitOfWork.GetRepo<Channel, Guid>();
            var spec = new ChannelProfileSpecification(channelId);
            var channel = await channelRepo.GetByIdWithSpecificationsAsync(spec);

            if (channel == null || channel.ChannelProfile == null) return null;

            return new ChannelProfileDTO
            {
                ChannelId = channel.Id.ToString(),
                Name = channel.ChannelProfile.name,
                ChannelsDescription = channel.ChannelProfile.channelsDescription,
                Avatar = channel.ChannelProfile.avatar,
                GreaterImg = channel.ChannelProfile.greaterImg,
                Links = channel.ChannelProfile.links,
                SubscribersCount = channel.ChannelProfile.subscribersCount
            };
        }

        public async Task<Pagination<ChannelVideoItemDTO>> GetChannelVideosAsync(string channelId, QueryParams queryParams)
        {
            var videoRepo = _unitOfWork.GetRepo<Video, Guid>();

            var countSpec = new ChannelVideosSpecification(channelId, queryParams);
            int totalCount = await videoRepo.CountAsync(countSpec);

            var dataSpec = new ChannelVideosSpecification(channelId, queryParams);
            var videos = await videoRepo.GetAllWithSpecificationAsync(dataSpec);

            var dtos = videos.Select(v => new ChannelVideoItemDTO
            {
                VideoId = v.video_Basics.VideoId,
                VideoTitle = v.video_Descriptive.Title,
                ThumbnailUrl = v.video_Basics.ThumbnailUrl,
                VideoUrl = v.video_Basics.videoUrl,
                ViewCount = v.VideoStats.watchCount,
                UploadDate = v.Temporal_Metadata.UploadDate
            });

            return new Pagination<ChannelVideoItemDTO>(queryParams.PageIndex, queryParams.PageSize, totalCount, dtos);
        }

        public async Task<bool> RemoveChannelAsync(Guid channelId)
        {
            var channelRepo = _unitOfWork.GetRepo<Channel, Guid>();
            var channel = await channelRepo.GetByIdAsync(channelId);

            if (channel == null) return false;

            // Cascade deletion mechanics will clean up dependent entities automatically via EF Core
            await channelRepo.DeleteAsync(channel);
            return await _unitOfWork.SaveChangesAsync() > 0;
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
            // Simulated User Claim tracking framework context
            var mockCurrentUserId = Guid.Parse("22222222-2222-2222-2222-222222222222");

            var profile = await _channelService.CreateChannelAsync(mockCurrentUserId, dto);
            
            if (profile == null)
            {
                return BadRequest(new ApiResponse<ChannelProfileDTO>("User already registers an active content creation channel asset or account mapping is corrupted.", 400));
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

            return Ok(new ApiResponse<object>(new { RemovedChannelId = channelId }, "Channel registry and dependent data trees pruned successfully from platform nodes."));
        }
    }
}