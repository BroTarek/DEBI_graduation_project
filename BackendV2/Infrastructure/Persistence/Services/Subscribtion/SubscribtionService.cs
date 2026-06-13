using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YouTubeClone.Core.Services;
using YouTubeClone.Core.Services.Specifications;
using YouTubeClone.Domain.Contracts.UOW;
using YouTubeClone.Domain.ValueObjects;
using YouTubeClone.Domain.Aggregates.Subscriptions;
using YouTubeClone.Domain.Aggregates.Channels;
using YouTubeClone.Shared.Responses;

namespace YouTubeClone.Infrastructure.Persistence.Services.Subscribtion
{
    public class SubscribtionService : ISubscribtionService
    {
        private readonly IUnitOfWork _unitOfWork;

        public SubscribtionService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task AddToSubscribtions(Guid ChannelID, Guid UserID)
        {
            var channelRepo = _unitOfWork.GetRepo<Channel, ChannelId>();
            var channel = await channelRepo.GetByIdAsync(new ChannelId(ChannelID));
            if (channel == null) throw new Exception("Channel not found");

            var subRepo = _unitOfWork.GetRepo<Subscriptions, SubscriptionId>();
            var spec = new SubscribtionByOwnerIdSpecification(UserID.ToString());
            var userSub = await subRepo.GetByIdWithSpecificationsAsync(spec);

            if (userSub == null)
            {
                userSub = new Subscriptions(new SubscriptionId(Guid.NewGuid()), UserID.ToString());
                userSub.TrackChannel(channel);
                await subRepo.AddAsync(userSub);
            }
            else
            {
                if (!userSub.Channels.Any(c => c.Id.Value == ChannelID))
                {
                    userSub.TrackChannel(channel);
                    await subRepo.UpdateAsync(userSub);
                }
            }
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task RemoveFromSubscribtions(Guid ChannelID, Guid UserID)
        {
            var subRepo = _unitOfWork.GetRepo<Subscriptions, SubscriptionId>();
            var spec = new SubscribtionByOwnerIdSpecification(UserID.ToString());
            var userSub = await subRepo.GetByIdWithSpecificationsAsync(spec);

            if (userSub != null)
            {
                var channelToRemove = userSub.Channels.FirstOrDefault(c => c.Id.Value == ChannelID);
                if (channelToRemove != null)
                {
                    userSub.UntrackChannel(channelToRemove);
                    await subRepo.UpdateAsync(userSub);
                    await _unitOfWork.SaveChangesAsync();
                }
            }
        }

        public async Task<List<SubscribtionBadgeDTO>> GetAllSubscribedChannels(Guid UserID)
        {
            var subRepo = _unitOfWork.GetRepo<Subscriptions, SubscriptionId>();
            var spec = new SubscribtionByOwnerIdSpecification(UserID.ToString());
            var userSub = await subRepo.GetByIdWithSpecificationsAsync(spec);

            if (userSub == null) return new List<SubscribtionBadgeDTO>();

            return userSub.Channels.Select(c => new SubscribtionBadgeDTO
            {
                ChannelName = c.Profile?.Name ?? "",
                ChannelAvatarURL = c.Profile?.Avatar ?? ""
            }).ToList();
        }

        public async Task<List<SubscribedChannelsVideosDTO>> GetAllSubscribedChannelsVideos(Guid UserID)
        {
            var subRepo = _unitOfWork.GetRepo<Subscriptions, SubscriptionId>();
            var spec = new SubscribtionByOwnerIdSpecification(UserID.ToString());
            var userSub = await subRepo.GetByIdWithSpecificationsAsync(spec);

            if (userSub == null) return new List<SubscribedChannelsVideosDTO>();

            var result = new List<SubscribedChannelsVideosDTO>();
            foreach (var c in userSub.Channels)
            {
                foreach (var v in c.Videos)
                {
                    result.Add(new SubscribedChannelsVideosDTO
                    {
                        VideoThumbnail = v.Basics?.ThumbnailUrl ?? "",
                        VideoURL = v.Basics?.VideoUrl ?? "",
                        ChannelAvatarURL = c.Profile?.Avatar ?? "",
                        ChannelName = c.Profile?.Name ?? "",
                        PublishDate = v.TemporalMetadata?.UploadDate.ToString("yyyy-MM-dd") ?? "",
                        views = v.Stats?.WatchCount ?? 0
                    });
                }
            }
            return result.OrderByDescending(x => x.PublishDate).ToList();
        }

        public async Task<List<SubscribedChannelsPostsDTO>> GetAllSubscribedChannelsPosts(Guid UserID)
        {
            var subRepo = _unitOfWork.GetRepo<Subscriptions, YouTubeClone.Domain.ValueObjects.SubscriptionId>();
            var spec = new SubscribtionByOwnerIdSpecification(UserID.ToString());
            var userSub = await subRepo.GetByIdWithSpecificationsAsync(spec);

            if (userSub == null) return new List<SubscribedChannelsPostsDTO>();

            var result = new List<SubscribedChannelsPostsDTO>();
            foreach (var c in userSub.Channels)
            {
                foreach (var p in c.Posts)
                {
                    result.Add(new SubscribedChannelsPostsDTO
                    {
                        Content = p.PostContent ?? "",
                        ChannelAvatarURL = c.Profile?.Avatar ?? "",
                        ChannelName = c.Profile?.Name ?? "",
                        PublishDate = "" // Post does not have a date property
                    });
                }
            }
            return result;
        }
    }
}
