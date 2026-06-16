using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using YouTubeClone.Domain.Contracts.UOW;
using YouTubeClone.Domain.Aggregates.Channels;
using YouTubeClone.Domain.Aggregates.Users;
using YouTubeClone.Domain.Aggregates.Videos;
using YouTubeClone.Domain.ValueObjects;
using YouTubeClone.Core.Services.Specifications.ChannelSpec;

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
            var userRepo = _unitOfWork.GetRepo<User, UserId>();
            var owner = await userRepo.GetByIdAsync(new UserId(ownerId));
            if (owner == null)
            {
                throw new KeyNotFoundException("User not found.");
            }

            var channelRepo = _unitOfWork.GetRepo<Channel, ChannelId>();

            // Check if the user already has a channel to avoid unique index violation
            var existingChannel = owner.Channel;
            if (existingChannel == null)
            {
                var spec = new ChannelByOwnerIdSpecification(ownerId);
                var channels = await channelRepo.GetAllWithSpecificationAsync(spec);
                existingChannel = channels.FirstOrDefault();
            }

            if (existingChannel != null)
            {
                var profile = new ChannelProfile(
                    dto.Description,
                    existingChannel.Profile?.Links ?? "",
                    dto.Name,
                    $"https://ui-avatars.com/api/?name={Uri.EscapeDataString(dto.Name)}&background=random",
                    existingChannel.Profile?.GreaterImg ?? "https://images.unsplash.com/photo-1618005182384-a83a8bd57fbe?w=1500&auto=format&fit=crop",
                    existingChannel.Profile?.SubscribersCount ?? 0
                );
                existingChannel.UpdateProfile(profile);

                await channelRepo.UpdateAsync(existingChannel);
                await _unitOfWork.SaveChangesAsync();

                return existingChannel.Id.Value;
            }

            var channelIdGuid = Guid.NewGuid();
            var channelId = new ChannelId(channelIdGuid);

            var newProfile = new ChannelProfile(
                dto.Description,
                "", // links
                dto.Name,
                $"https://ui-avatars.com/api/?name={Uri.EscapeDataString(dto.Name)}&background=random",
                "https://images.unsplash.com/photo-1618005182384-a83a8bd57fbe?w=1500&auto=format&fit=crop",
                0
            );

            var channel = new Channel(channelId, owner, newProfile);
            owner.AssignChannel(channel);

            await channelRepo.AddAsync(channel);
            await _unitOfWork.SaveChangesAsync();

            return channelIdGuid;
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