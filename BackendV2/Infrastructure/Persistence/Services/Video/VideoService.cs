using YouTubeClone.Domain.Contracts.UOW;
using YouTubeClone.Core.Services;
using YouTubeClone.Persistance.Evaluator;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YouTubeClone.Domain.Aggregates.Videos;
using YouTubeClone.Services.Specifications;

using YouTubeClone.Persistance.Services.Storage2;

namespace YouTubeClone.Domain.Services
{
    public class VideoService : IVideoService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IViewCountService _viewCountService;
        private readonly UploadContext _uploadContext;

        public VideoService(IUnitOfWork unitOfWork, IViewCountService viewCountService, UploadContext uploadContext)
        {
            _unitOfWork = unitOfWork;
            _viewCountService = viewCountService;
            _uploadContext = uploadContext;
        }

        public async Task<List<HomePageVideo>> GetHomePageVideosAsync(int skip, int take)
        {
            var repo = _unitOfWork.GetRepo<Video, YouTubeClone.Domain.ValueObjects.VideoId>();
            var videos = await repo.GetAllAsync();
            return videos
                .OrderByDescending(v => v.TemporalMetadata.UploadDate)
                .Skip(skip)
                .Take(take)
                .Select(v => new HomePageVideo
                {
                    VideoId = v.Id.Value.ToString(),
                    ThumbnailUrl = v.Basics.ThumbnailUrl,
                    Title = v.Descriptive.Title,
                    ChannelName = v.ChannelId,
                    Views = v.Stats.WatchCount,
                    UploadDate = v.TemporalMetadata.UploadDate,
                    Duration = TimeSpan.FromSeconds(v.TechnicalDetails.Duration),
                    Accessibility = v.Basics.PrivacyStatus.ToString(),
                    Category = v.Descriptive.Category,
                    VideoUrl = v.Basics.VideoUrl
                })
                .ToList();
        }

        public async Task<VideoWatchDto> GetWatchPageVideoAsync(Guid videoId)
        {
            return new VideoWatchDto {
                Title = "", Description = "", VideoUrl = "", ChannelName = "", ChannelAvatarUrl = ""
            };
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

            var basics = new video_Basics(videoIdGuid.ToString(), thumbnailUrl, videoUrl, YouTubeClone.Domain.Aggregates.Accessibility.PUBLIC);
            
            // Assuming dto.Tags is a comma separated string
            var tagsArray = string.IsNullOrEmpty(dto.Tags) ? Array.Empty<string>() : dto.Tags.Split(',');
            var descriptive = new video_Descriptive(dto.Title, dto.Description, dto.Category, tagsArray);
            
            // Technical details defaults/estimates
            long fileSize = dto.VideoFile?.Length ?? 0;
            var technical = new video_Technical_details(dto.DurationSeconds, "1080p", fileSize, "mp4", "h264", "aac", 30f, 5000);
            
            var temporal = new Temporal_Metadata(DateTime.UtcNow, DateTime.UtcNow, "Uploaded");
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
    }
}