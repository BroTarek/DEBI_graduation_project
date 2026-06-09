using Makanak.Abstraction.Storage;
using Makanak.Domain.Contracts.UOW;
using Makanak.Presentation.Controllers;
using Makanak.Shared.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YouTubeClone.Domain.Aggregates.Channels;
using YouTubeClone.Domain.Aggregates.Interactions;
using YouTubeClone.Domain.Aggregates.Videos;
using YouTubeClone.Domain.Aggregates.WatchHistories;
using YouTubeClone.Domain.Services;
using YouTubeClone.Domain.ValueObjects;

namespace YouTubeClone.Presentation.Controllers
{
    public class VideoController : BaseController
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMediaStorageService _mediaStorage;
        private readonly IViewCountService _viewCountService;

        public VideoController(
            IUnitOfWork unitOfWork,
            IMediaStorageService mediaStorage,
            IViewCountService viewCountService)
        {
            _unitOfWork = unitOfWork;
            _mediaStorage = mediaStorage;
            _viewCountService = viewCountService;
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> UploadVideo([FromForm] UploadVideoDto dto){
            /*
            get all the data for the video (the prefered cloudStorage) and create a new video
            upload the Video to the cloud stroage provider 
            if successful upload the thumnail to the Local Storage of the app wwwroot
            then upload the other video entity to the database 

            use the unit of work to save to the database to ensure atomic transaction (if upload to the cloud provider failed do not save anything in the database)



            */
        }

        [HttpGet]
        public async Task<IActionResult> GetVideos([FromQuery] int skip = 0, [FromQuery] int take = 10)
        {
            var videoRepo = _unitOfWork.GetRepo<Video, VideoId>();
            var allVideos = await videoRepo.GetAllAsync();

            var result = allVideos
                .Where(v => v.PrivacyStatus == PrivacyStatus.Public)
                .OrderByDescending(v => v.CreatedAt)
                .Skip(skip)
                .Take(take)
                .Select(v => new VideoDto
                {
                    Id = v.Id.Value,
                    ChannelId = v.ChannelId.Value,
                    Title = v.Title.Value,
                    Description = v.Description.Value,
                    DurationSeconds = v.Duration.Seconds,
                    ThumbnailUrl = v.ThumbnailUrl.Value,
                    Category = v.Category.Value,
                    Tags = v.Tags.Select(t => t.Value).ToList(),
                    ViewCount = v.ViewCount,
                    CreatedAt = v.CreatedAt
                })
                .ToList();

            return Ok(new ApiResponse<List<VideoDto>>(result, "Videos retrieved successfully."));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetVideo(Guid id)
        {
            var videoId = new VideoId(id);
            var videoRepo = _unitOfWork.GetRepo<Video, VideoId>();
            var video = await videoRepo.GetByIdAsync(videoId);
            if (video == null)
            {
                return NotFound(new ApiResponse<string>("Video not found.", 404));
            }

            // Increment View Count
            _viewCountService.IncrementViewCount(video);
            await videoRepo.UpdateAsync(video);

            // Record Watch History if Authenticated
            var userIdStr = GetUserId();
            if (!string.IsNullOrEmpty(userIdStr) && Guid.TryParse(userIdStr, out var userIdGuid))
            {
                var userId = new UserId(userIdGuid);
                var historyRepo = _unitOfWork.GetRepo<WatchHistory, WatchHistoryId>();
                var allHistory = await historyRepo.GetAllAsync();
                var existingHistory = allHistory.FirstOrDefault(h => h.UserId.Value == userId.Value && h.VideoId.Value == videoId.Value);

                if (existingHistory != null)
                {
                    existingHistory.UpdateWatchDuration(existingHistory.WatchDurationSeconds + 10); // simulate watch duration increment
                    await historyRepo.UpdateAsync(existingHistory);
                }
                else
                {
                    var watchHistory = new WatchHistory(new WatchHistoryId(Guid.NewGuid()), userId, videoId, 10);
                    await historyRepo.AddAsync(watchHistory);
                }
            }

            await _unitOfWork.SaveChangesAsync();

            return Ok(new ApiResponse<VideoDto>(new VideoDto
            {
                Id = video.Id.Value,
                ChannelId = video.ChannelId.Value,
                Title = video.Title.Value,
                Description = video.Description.Value,
                DurationSeconds = video.Duration.Seconds,
                ThumbnailUrl = video.ThumbnailUrl.Value,
                Category = video.Category.Value,
                Tags = video.Tags.Select(t => t.Value).ToList(),
                ViewCount = video.ViewCount,
                CreatedAt = video.CreatedAt
            }, "Video details retrieved successfully."));
        }

        [HttpPost("{id}/react")]
        [Authorize]
        public async Task<IActionResult> React(Guid id, [FromQuery] bool isLike)
        {
            var userIdStr = GetUserId();
            if (string.IsNullOrEmpty(userIdStr) || !Guid.TryParse(userIdStr, out var userIdGuid))
            {
                return Unauthorized(new ApiResponse<string>("Unauthorized user.", 401));
            }

            var userId = new UserId(userIdGuid);
            var videoId = new VideoId(id);

            var videoRepo = _unitOfWork.GetRepo<Video, VideoId>();
            var video = await videoRepo.GetByIdAsync(videoId);
            if (video == null)
            {
                return NotFound(new ApiResponse<string>("Video not found.", 404));
            }

            var type = isLike ? InteractionType.Like : InteractionType.Dislike;
            var target = new InteractionTarget(InteractionTarget.TargetType.Video, id);

            var interactionRepo = _unitOfWork.GetRepo<UserInteraction, UserInteractionId>();
            var allInteractions = await interactionRepo.GetAllAsync();
            var existingInteraction = allInteractions.FirstOrDefault(i => 
                i.UserId.Value == userId.Value && 
                i.Target.Id == id && 
                i.Target.Type == InteractionTarget.TargetType.Video);

            if (existingInteraction != null)
            {
                await interactionRepo.DeleteAsync(existingInteraction);
                
                if (existingInteraction.Type != type)
                {
                    var newInteraction = new UserInteraction(new UserInteractionId(Guid.NewGuid()), userId, target, type);
                    await interactionRepo.AddAsync(newInteraction);
                    await _unitOfWork.SaveChangesAsync();
                    return Ok(new ApiResponse<string>($"Changed reaction to {(isLike ? "Like" : "Dislike")}."));
                }

                await _unitOfWork.SaveChangesAsync();
                return Ok(new ApiResponse<string>("Reaction removed."));
            }
            else
            {
                var newInteraction = new UserInteraction(new UserInteractionId(Guid.NewGuid()), userId, target, type);
                await interactionRepo.AddAsync(newInteraction);
                await _unitOfWork.SaveChangesAsync();
                return Ok(new ApiResponse<string>($"Added reaction {(isLike ? "Like" : "Dislike")}."));
            }
        }
    }

    

   
}
