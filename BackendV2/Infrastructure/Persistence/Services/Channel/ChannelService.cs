using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Makanak.Domain.Contracts.UOW;
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
            var userOwnerId = new UserId(ownerId);
            var channelIdGuid = Guid.NewGuid();
            var channelId = new ChannelId(channelIdGuid);

            // Channel creation only sets Owner and Profile
            var profile = new ChannelProfile(0, dto.Description, string.Empty, dto.Name, string.Empty, string.Empty);
            
            // Fetch owner user to satisfy the Channel aggregate
            var userRepo = _unitOfWork.GetRepo<User, UserId>();
            var owner = await userRepo.GetByIdAsync(userOwnerId);

            var channel = new Channel(channelId, owner, profile);
            
            var channelRepo = _unitOfWork.GetRepo<Channel, ChannelId>();
            await channelRepo.AddAsync(channel);
            await _unitOfWork.SaveChangesAsync();

            return channelIdGuid;
        }

        public async Task<ChannelAboutDto> GetChannelAboutAsync(Guid id)
        {
            var channelRepo = _unitOfWork.GetRepo<Channel, ChannelId>();
            var channel = await channelRepo.GetByIdAsync(new ChannelId(id));
            
            if (channel == null) return null;

            return new ChannelAboutDto
            {
                SubscribersCount = channel.Profile.SubscribersCount,
                ChannelsDescription = channel.Profile.ChannelsDescription,
                Links = channel.Profile.Links,
                Name = channel.Profile.Name,
                Avatar = channel.Profile.Avatar,
                GreaterImg = channel.Profile.GreaterImg
            };
        }

        public async Task<IReadOnlyList<ChannelVideoDto>> GetChannelVideosAsync(Guid id)
        {
            var videoRepo = _unitOfWork.GetRepo<Video, VideoId>();
            
            // Crucial fix: Video aggregate uses string for ChannelId
            var channelIdString = id.ToString();
            var videos = await videoRepo.FindAsync(v => v.ChannelId == channelIdString);
            
            return videos.Select(v => new ChannelVideoDto
            {
                Id = v.Id.Value,
                Title = v.Descriptive.Title,
                viewCount = v.Stats.WatchCount,
                ThumbnailURL = v.Basics.ThumbnailUrl,
                PublishDate = v.TemporalMetadata.UploadDate
            }).ToList();
        }
    }
}