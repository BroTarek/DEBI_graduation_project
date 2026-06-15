using System;
using System.Collections.Generic;
using YouTubeClone.Domain.Entities.Videos;
using YouTubeClone.Domain.Entities.Subscriptions;
using YouTubeClone.Domain.Enums;
using SoftBridge.Services.Specification;
using YouTubeClone.Shared.Common.Params;

namespace YouTubeClone.Services.Specifications
{
    public class SubscribedChannelsVideosSpecification : BaseSpecification<Video, Guid>
    {
        public SubscribedChannelsVideosSpecification(QueryParams query, List<Guid> followedChannelIds)
            : base(v => followedChannelIds.Contains(v.channelId) && 
                       v.video_Basics.PrivacyStatus == Accessibility.PUBLIC &&
                       (string.IsNullOrEmpty(query.Search) || 
                        v.video_Descriptive.Title.ToLower().Contains(query.Search)))
        {
            AddInclude(v => v.video_Basics);
            AddInclude(v => v.video_Descriptive);
            AddInclude(v => v.VideoStats);
            AddInclude("Channel.ChannelProfile");

            AddOrderByDescending(v => v.Temporal_Metadata.UploadDate);

            ApplyPaging((query.PageIndex - 1) * query.PageSize, query.PageSize);
        }
    }

    public class SubscriptionSpecification : BaseSpecification<Subscription, Guid>
    {
        public SubscriptionSpecification(QueryParams query, Guid userId)
            : base(s => s.ownerId == userId.ToString() && 
                       (string.IsNullOrEmpty(query.Search) || 
                        s.Channel.ChannelProfile.name.ToLower().Contains(query.Search)))
        {
            AddInclude("Channel.ChannelProfile");
            ApplyPaging((query.PageIndex - 1) * query.PageSize, query.PageSize);
        }
        
        public SubscriptionSpecification(Guid userId, Guid channelId)
            : base(s => s.ownerId == userId.ToString() && s.ChannelId == channelId)
        {
        }
    }
}
