using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YouTubeClone.Domain.Contracts.UOW;
using YouTubeClone.Domain.Entities.Channels;
using YouTubeClone.Domain.Entities.Videos;
using YouTubeClone.Domain.Entities.Playlists;
using YouTubeClone.Domain.Models.Identity;
using YouTubeClone.Services.Specifications;
using YouTubeClone.Shared.Common;
using YouTubeClone.Shared.Common.Params;
using YouTubeClone.Shared.Dto_s;

namespace YouTubeClone.Services
{
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
            var userRepo = _unitOfWork.GetRepo<ApplicationUser, string>();

            var checkSpec = new ChannelProfileSpecification(userId.ToString());
            var existingChannel = await channelRepo.GetByIdWithSpecificationsAsync(checkSpec);
            if (existingChannel != null) return null;

            var ownerUser = await userRepo.GetByIdAsync(userId.ToString());
            if (ownerUser == null) return null;

            var newChannel = new Channel
            {
                OwnerId = userId.ToString(),
                Owner = ownerUser,
                ChannelProfile = new ChannelProfile
                {
                    name = dto.Name,
                    channelsDescription = dto.ChannelsDescription,
                    links = dto.Links,
                    subscribersCount = 0,
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
                VideoId = v.video_Basics.VideoId.ToString(),
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

            await channelRepo.DeleteAsync(channel);
            return await _unitOfWork.SaveChangesAsync() > 0;
        }
    }
}
