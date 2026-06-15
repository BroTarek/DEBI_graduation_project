using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YouTubeClone.Domain.Contracts.UOW;
using YouTubeClone.Domain.Entities.Channels;
using YouTubeClone.Domain.Entities.Videos;
using YouTubeClone.Domain.Entities.Subscriptions;
using YouTubeClone.Services.Specifications;
using YouTubeClone.Shared.Common;
using YouTubeClone.Shared.Common.Params;
using YouTubeClone.Shared.Dto_s;

namespace YouTubeClone.Services
{
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
                ChannelId = s.ChannelId.ToString(),
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

            var cleanSubCheckSpec = new SubscriptionSpecification(new QueryParams { PageSize = 1000 }, userId);
            var userSubs = await subRepo.GetAllWithSpecificationAsync(cleanSubCheckSpec);
            var followedChannelIds = userSubs.Select(s => s.ChannelId).ToList();

            if (!followedChannelIds.Any())
            {
                return new Pagination<SubscribedChannelsVideosDTO>(queryParams.PageIndex, queryParams.PageSize, 0, Enumerable.Empty<SubscribedChannelsVideosDTO>());
            }

            var videoCountSpec = new SubscribedChannelsVideosSpecification(queryParams, followedChannelIds);
            int totalCount = await videoRepo.CountAsync(videoCountSpec);

            var videoDataSpec = new SubscribedChannelsVideosSpecification(queryParams, followedChannelIds);
            var videos = await videoRepo.GetAllWithSpecificationAsync(videoDataSpec);

            var dtos = videos.Select(v => new SubscribedChannelsVideosDTO
            {
                VideoId = v.video_Basics.VideoId.ToString(),
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

            var targetChannel = await channelRepo.GetByIdAsync(Guid.Parse(channelId));
            if (targetChannel == null) return false;

            var checkSpec = new SubscriptionSpecification(userId, Guid.Parse(channelId));
            var existingSub = await subRepo.GetByIdWithSpecificationsAsync(checkSpec);

            bool isSubscribedNow;

            if (existingSub != null)
            {
                await subRepo.DeleteAsync(existingSub);
                targetChannel.ChannelProfile.subscribersCount = Math.Max(0, targetChannel.ChannelProfile.subscribersCount - 1);
                isSubscribedNow = false;
            }
            else
            {
                var newSub = new Subscription
                {
                    Id = Guid.NewGuid(),
                    ownerId = userId.ToString(),
                    ChannelId = Guid.Parse(channelId)
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
