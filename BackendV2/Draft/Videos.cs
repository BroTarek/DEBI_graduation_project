using System;
using YouTubeClone.Domain.Models; // Assuming this is where your domain entities sit
using SoftBridge.Services.Specification;
using YouTubeClone.Shared.Common.Params;

namespace YouTubeClone.Services.Specifications
{
    public class VideoWithDetailsSpecification : BaseSpecification<Video, Guid>
    {
        // Constructor for Home Page (Paginated, Filtered, and Sorted)
        public VideoWithDetailsSpecification(QueryParams queryParams)
            : base(v => v.video_Basics.PrivacyStatus == Accessibility.PUBLIC &&
                       (string.IsNullOrEmpty(queryParams.Search) || 
                        v.video_Descriptive.Title.ToLower().Contains(queryParams.Search)))
        {
            // Eager load the 1:1 components and navigation properties from your diagram
            AddInclude(v => v.video_Basics);
            AddInclude(v => v.video_Descriptive);
            AddInclude(v => v.video_Technical_details);
            AddInclude(v => v.VideoStats);
            
            // Nested string-based include for the Channel and its Profile
            AddInclude("Channel.ChannelProfile");

            // Handle Sorting based on your custom SortingOptionsEnum
            if (queryParams.Sort.HasValue)
            {
                switch (queryParams.Sort.Value)
                {
                    case SortingOptionsEnum.NameAsc:
                        AddOrderBy(v => v.video_Descriptive.Title);
                        break;
                    case SortingOptionsEnum.NameDesc:
                        AddOrderByDescending(v => v.video_Descriptive.Title);
                        break;
                    case SortingOptionsEnum.DateCreatedAsc:
                        AddOrderBy(v => v.Temporal_Metadata.UploadDate);
                        break;
                    case SortingOptionsEnum.DateCreatedDesc:
                    default:
                        AddOrderByDescending(v => v.Temporal_Metadata.UploadDate);
                        break;
                }
            }
            else
            {
                // Default fallback sort
                AddOrderByDescending(v => v.Temporal_Metadata.UploadDate);
            }

            // Apply Paging safely using base params
            ApplyPaging((queryParams.PageIndex - 1) * queryParams.PageSize, queryParams.PageSize);
        }

        // Constructor for Single Video Watch View
        public VideoWithDetailsSpecification(Guid videoId) 
            : base(v => v.video_Basics.VideoId == videoId.ToString())
        {
            AddInclude(v => v.video_Basics);
            AddInclude(v => v.video_Descriptive);
            AddInclude(v => v.video_Technical_details);
            AddInclude(v => v.VideoStats);
            AddInclude("Channel.ChannelProfile");
            AddInclude("comments.parentComment"); // If you want to load comments
        }
    }
}

using System;
using YouTubeClone.Domain.Models;
using SoftBridge.Services.Specification;

namespace YouTubeClone.Services.Specifications
{
    public class WatchHistorySpecification : BaseSpecification<WatchHistory, Guid>
    {
        public WatchHistorySpecification(Guid userId) 
            : base(wh => wh.owner.Id == userId) // Assuming User has an Id matching TKey
        {
            // Pull in the collection of watched videos and their necessary metadata
            AddInclude("videos.video_Basics");
            AddInclude("videos.video_Descriptive");
            AddInclude("videos.VideoStats");
            AddInclude("videos.Channel");
        }
    }
}
using System;
using System.Collections.Generic;

namespace YouTubeClone.Shared.Dto_s
{
    public class WatchVideoDetailDTO
    {
        public string VideoId { get; set; } = string.Empty;
        public string VideoUrl { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public List<string> Tags { get; set; } = new();
        
        // Stats
        public int WatchCount { get; set; }
        public int LikesCount { get; set; }
        public int DislikesCount { get; set; }

        // Channel Quick Info
        public string ChannelId { get; set; } = string.Empty;
        public string ChannelName { get; set; } = string.Empty;
        public string ChannelAvatar { get; set; } = string.Empty;

        // Comments Section
        public List<VideoCommentDTO> Comments { get; set; } = new();
    }

    public class VideoCommentDTO
    {
        public string CommentId { get; set; } = string.Empty;
        public string AuthorId { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string? ParentCommentId { get; set; }
        
        // This allows the front-end to render nested reply trees easily
        public List<VideoCommentDTO> Replies { get; set; } = new();
    }
}
namespace YouTubeClone.Shared.Dto_s
{
    public class HomePageVideoDTO
    {
        public string VideoId { get; set; } = string.Empty;
        public string ThumbnailUrl { get; set; } = string.Empty;
        public int VideoLength { get; set; } // items from video_Technical_details
        public string VideoTitle { get; set; } = string.Empty;
        public int ViewCount { get; set; }
        public string ChannelAvatar { get; set; } = string.Empty;
        public string ChannelName { get; set; } = string.Empty;
    }

    public class WatchHistoryVideoDTO
    {
        public string VideoId { get; set; } = string.Empty;
        public string ThumbnailUrl { get; set; } = string.Empty;
        public int VideoLength { get; set; }
        public string VideoTitle { get; set; } = string.Empty;
        public int ViewCount { get; set; }
        public string ChannelName { get; set; } = string.Empty;
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

using YouTubeClone.Persistance.Services.Storage2;

namespace YouTubeClone.Services
{
    public class VideoService : IVideoService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UploadContext _uploadContext;


        public VideoService(IUnitOfWork unitOfWork, UploadContext uploadContext)
        {
            _unitOfWork = unitOfWork;
            _uploadContext = uploadContext;
            
        }
        public async Task<Guid> UploadVideoAsync(UploadVideoDto dto, string channelId, string preferredProvider = "CLOUDINARY")
        {
            // 1. Upload the video to the cloud via context
            var videoUrl = await _uploadContext.UploadVideoAsync(dto.VideoFile, preferredProvider);

            // 2. Upload the thumbnail locally to wwwroot/uploads/thumbnails
            var thumbnailUrl = await _uploadContext.UploadImageAsync(dto.ThumbnailFile, "thumbnails");

            // 3. Construct Aggregate
            var videoIdGuid = Guid.NewGuid();
            // Assuming VideoId expects Guid or we cast it if it's a strongly typed id.
            var videoId = new YouTubeClone.Domain.ValueObjects.VideoId(videoIdGuid);

            var basics = new VideoBasics(videoIdGuid.ToString(), thumbnailUrl, videoUrl, YouTubeClone.Domain.Enums.Accessibility.PUBLIC);
            
            // Assuming dto.Tags is a comma separated string
            var tagsArray = string.IsNullOrEmpty(dto.Tags) ? Array.Empty<string>() : dto.Tags.Split(',');
            var descriptive = new VideoDescriptive(dto.Title, dto.Description, dto.Category, tagsArray);
            
            // Technical details defaults/estimates
            long fileSize = dto.VideoFile?.Length ?? 0;
            var technical = new VideoTechnicalDetails(dto.DurationSeconds, "1080p", fileSize, "mp4", "h264", "aac", 30f, 5000);
            
            var temporal = new TemporalMetadata(DateTime.UtcNow, DateTime.UtcNow, "Uploaded");
            var stats = new VideoStats(0, 0, 0);

            // 4. Create the aggregate
            var video = new Video(videoId, channelId, basics, descriptive, technical, temporal, stats);

            // 5. Save to the database using UnitOfWork
            // Assuming generic repository AddAsync
            var repo = _unitOfWork.GetRepo<Video, YouTubeClone.Domain.ValueObjects.VideoId>();
            await repo.AddAsync(video);
            await _unitOfWork.SaveChangesAsync();

            return videoIdGuid;
        }


        public async Task<Pagination<HomePageVideoDTO>> GetHomePageVideosAsync(QueryParams queryParams)
        {
            var videoRepo = _unitOfWork.GetRepo<Video, Guid>();
            
            // 1. Get total record count for metadata calculations matching criteria
            var countSpec = new VideoWithDetailsSpecification(queryParams); 
            int totalCount = await videoRepo.CountAsync(countSpec);

            // 2. Fetch the paginated dataset
            var dataSpec = new VideoWithDetailsSpecification(queryParams);
            var videos = await videoRepo.GetAllWithSpecificationAsync(dataSpec);

            // 3. Map Domain Objects to Clean DTOs
            var videoDtos = videos.Select(v => new HomePageVideoDTO
            {
                VideoId = v.video_Basics.VideoId,
                ThumbnailUrl = v.video_Basics.ThumbnailUrl,
                VideoLength = v.video_Technical_details.duration,
                VideoTitle = v.video_Descriptive.Title,
                ViewCount = v.VideoStats.watchCount,
                ChannelAvatar = v.Channel?.ChannelProfile?.avatar ?? string.Empty,
                ChannelName = v.Channel?.ChannelProfile?.name ?? string.Empty
            });

            return new Pagination<HomePageVideoDTO>(queryParams.PageIndex, queryParams.PageSize, totalCount, videoDtos);
        }

        public async Task<WatchVideoDetailDTO?> WatchVideoAsync(Guid videoId, Guid userId)
        {
            var videoRepo = _unitOfWork.GetRepo<Video, Guid>();
            
            // Reusing your VideoWithDetailsSpecification which includes basics, descriptive, stats, and comments
            var spec = new VideoWithDetailsSpecification(videoId);
            var video = await videoRepo.GetByIdWithSpecificationsAsync(spec);

            if (video == null) return null;

            // 1. Dynamic side-effect: Increment watch counter
            video.VideoStats.watchCount++;
            await videoRepo.UpdateAsync(video);

            // 2. Dynamic side-effect: Append to User's Watch History
            var historyRepo = _unitOfWork.GetRepo<WatchHistory, Guid>();
            var historySpec = new WatchHistorySpecification(userId);
            var userHistory = await historyRepo.GetByIdWithSpecificationsAsync(historySpec);

            if (userHistory != null)
            {
                userHistory.videos ??= new List<Video>();
                if (!userHistory.videos.Any(v => v.video_Basics.VideoId == videoId.ToString()))
                {
                    userHistory.videos.Add(video);
                    await historyRepo.UpdateAsync(userHistory);
                }
            }

            // Commit counters and history updates to the database safely
            await _unitOfWork.SaveChangesAsync();

            // 3. Map out ONLY the specific components requested into our clean DTO
            var dto = new WatchVideoDetailDTO
            {
                VideoId = video.video_Basics.VideoId,
                VideoUrl = video.video_Basics.videoUrl,
                Title = video.video_Descriptive.Title,
                Description = video.video_Descriptive.Description,
                Category = video.video_Descriptive.Category,
                Tags = video.video_Descriptive.Tags?.ToList() ?? new List<string>(),
                
                WatchCount = video.VideoStats.watchCount,
                LikesCount = video.VideoStats.likesCount,
                DislikesCount = video.VideoStats.dislikesCount,

                ChannelId = video.channelId,
                ChannelName = video.Channel?.ChannelProfile?.name ?? string.Empty,
                ChannelAvatar = video.Channel?.ChannelProfile?.avatar ?? string.Empty
            };

            // 4. Assemble flat comments array into an organized nested tree structure
            if (video.comments != null && video.comments.Any())
            {
                // First, map all raw domain comments to DTO layouts
                var allCommentDtos = video.comments.Select(c => new VideoCommentDTO
                {
                    CommentId = c.commentId,
                    AuthorId = c.authorId,
                    Content = c.content,
                    ParentCommentId = c.parentComment?.commentId // Reading your self-referential association
                }).ToList();

                // Group them by parent to structure the hierarchy cleanly
                var lookup = allCommentDtos.ToLookup(c => c.ParentCommentId);
                
                // Top-level comments are those without a parent reference
                dto.Comments = lookup[null].ToList();

                // Recursively inject replies into their respective parent nodes
                foreach (var comment in allCommentDtos)
                {
                    comment.Replies = lookup[comment.CommentId].ToList();
                }
            }

            return dto;
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
            // Simulation of pulling authenticated user session claim context
            var mockCurrentUserId = Guid.Parse("00000000-0000-0000-0000-000000000001"); 
            
            var videoDetails = await _videoService.WatchVideoAsync(videoId, mockCurrentUserId);
            
            if (videoDetails == null)
            {
                return NotFound(new ApiResponse<WatchVideoDetailDTO>("Requested video could not be found.", 404));
            }

            return Ok(new ApiResponse<WatchVideoDetailDTO>(videoDetails, "Video metadata package compiled successfully."));
        }
    }
}