using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YouTubeClone.Domain.Contracts.UOW;
using YouTubeClone.Domain.Aggregates.Channels;
using YouTubeClone.Domain.Aggregates.Users;
using YouTubeClone.Domain.Aggregates.Videos;
using YouTubeClone.Domain.ValueObjects;

namespace YouTubeClone.Domain.Services
{
    public class ChannelService : IChannelService
    {
        private readonly IUnitOfWork _unitOfWork;
        
        public ChannelService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Guid> CreateChannelAsync(CreateChannelDto dto, Guid ownerId)
        {
            return Guid.NewGuid();
        }

        public async Task<ChannelAboutDto> GetChannelAboutAsync(Guid id)
        {
            var repo = _unitOfWork.GetRepo<Channel, ChannelId>();
            var channel = await repo.GetByIdAsync(new ChannelId(id));
            if (channel == null) return null;

            return new ChannelAboutDto
            {
                SubscribersCount = channel.Profile?.SubscribersCount ?? 0,
                ChannelsDescription = channel.Profile?.ChannelsDescription ?? "",
                Links = channel.Profile?.Links ?? "",
                Name = channel.Profile?.Name ?? "",
                Avatar = channel.Profile?.Avatar ?? "",
                GreaterImg = channel.Profile?.GreaterImg ?? ""
            };
        }

        public async Task<IReadOnlyList<ChannelVideoDto>> GetChannelVideosAsync(Guid id)
        {
            var videoRepo = _unitOfWork.GetRepo<Video, YouTubeClone.Domain.ValueObjects.VideoId>();
            var videos = await videoRepo.GetAllAsync();
            return videos
                .Where(v => v.ChannelId == id.ToString())
                .Select(v => new ChannelVideoDto
                {
                    Id = v.Id.Value,
                    Title = v.Descriptive?.Title ?? "",
                    viewCount = v.Stats?.WatchCount ?? 0,
                    ThumbnailURL = v.Basics?.ThumbnailUrl ?? "",
                    PublishDate = v.TemporalMetadata?.UploadDate ?? DateTime.UtcNow
                })
                .ToList();
        }
    }
}