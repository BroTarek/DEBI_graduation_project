using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YouTubeClone.Domain.Contracts.UOW;
using YouTubeClone.Domain.Entities.Videos;
using YouTubeClone.Domain.Entities.WatchHistories;
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
            var videoUrl = await _uploadContext.UploadVideoAsync(dto.VideoFile, preferredProvider);
            var thumbnailUrl = await _uploadContext.UploadImageAsync(dto.ThumbnailFile, "thumbnails");

            var videoIdGuid = Guid.NewGuid();
            var basics = new VideoBasics(videoIdGuid, thumbnailUrl, videoUrl, YouTubeClone.Domain.Enums.Accessibility.PUBLIC);
            
            var tagsArray = string.IsNullOrEmpty(dto.Tags) ? Array.Empty<string>() : dto.Tags.Split(',');
            var descriptive = new VideoDescriptive(dto.Title, dto.Description, dto.Category, tagsArray);
            
            long fileSize = dto.VideoFile?.Length ?? 0;
            var technical = new VideoTechnicalDetails(dto.DurationSeconds, "1080p", fileSize, "mp4", "h264", "aac", 30f, 5000);
            
            var temporal = new TemporalMetadata(DateTime.UtcNow, DateTime.UtcNow, "Uploaded");
            var stats = new VideoStats(0, 0, 0);

            var video = new Video(videoIdGuid, Guid.Parse(channelId), basics, descriptive, technical, temporal, stats);

            var repo = _unitOfWork.GetRepo<Video, Guid>();
            await repo.AddAsync(video);
            await _unitOfWork.SaveChangesAsync();

            return videoIdGuid;
        }

        public async Task<Pagination<HomePageVideoDTO>> GetHomePageVideosAsync(QueryParams queryParams)
        {
            var videoRepo = _unitOfWork.GetRepo<Video, Guid>();
            
            var countSpec = new VideoWithDetailsSpecification(queryParams); 
            int totalCount = await videoRepo.CountAsync(countSpec);

            var dataSpec = new VideoWithDetailsSpecification(queryParams);
            var videos = await videoRepo.GetAllWithSpecificationAsync(dataSpec);

            var videoDtos = videos.Select(v => new HomePageVideoDTO
            {
                VideoId = v.video_Basics.VideoId.ToString(),
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
            
            var spec = new VideoWithDetailsSpecification(videoId);
            var video = await videoRepo.GetByIdWithSpecificationsAsync(spec);

            if (video == null) return null;

            video.VideoStats.watchCount++;
            await videoRepo.UpdateAsync(video);

            var historyRepo = _unitOfWork.GetRepo<WatchHistory, Guid>();
            var historySpec = new WatchHistorySpecification(userId);
            var userHistory = await historyRepo.GetByIdWithSpecificationsAsync(historySpec);

            if (userHistory != null)
            {
                userHistory.videos ??= new List<Video>();
                if (!userHistory.videos.Any(v => v.video_Basics.VideoId == videoId))
                {
                    userHistory.videos.Add(video);
                    await historyRepo.UpdateAsync(userHistory);
                }
            }

            await _unitOfWork.SaveChangesAsync();

            var dto = new WatchVideoDetailDTO
            {
                VideoId = video.video_Basics.VideoId.ToString(),
                VideoUrl = video.video_Basics.videoUrl,
                Title = video.video_Descriptive.Title,
                Description = video.video_Descriptive.Description,
                Category = video.video_Descriptive.Category,
                Tags = video.video_Descriptive.Tags?.ToList() ?? new List<string>(),
                
                WatchCount = video.VideoStats.watchCount,
                LikesCount = video.VideoStats.likesCount,
                DislikesCount = video.VideoStats.dislikesCount,

                ChannelId = video.channelId.ToString(),
                ChannelName = video.Channel?.ChannelProfile?.name ?? string.Empty,
                ChannelAvatar = video.Channel?.ChannelProfile?.avatar ?? string.Empty
            };

            if (video.comments != null && video.comments.Any())
            {
                var allCommentDtos = video.comments.Select(c => new VideoCommentDTO
                {
                    CommentId = c.Id.ToString(),
                    AuthorId = c.AuthorId,
                    Content = c.Content,
                    ParentCommentId = c.ParentCommentId?.ToString()
                }).ToList();

                var lookup = allCommentDtos.ToLookup(c => c.ParentCommentId);
                
                dto.Comments = lookup[null].ToList();

                foreach (var comment in allCommentDtos)
                {
                    comment.Replies = lookup[comment.CommentId].ToList();
                }
            }

            return dto;
        }
    }
}
