using System;
using YouTubeClone.Domain.Entities.Channels;
using YouTubeClone.Domain.Entities.Videos;
using YouTubeClone.Domain.Enums;
using SoftBridge.Services.Specification;
using YouTubeClone.Shared.Common.Params;

namespace YouTubeClone.Services.Specifications
{
    public class ChannelVideosSpecification : BaseSpecification<Video, Guid>
    {
        public ChannelVideosSpecification(string channelId, QueryParams query)
            : base(v => v.channelId == Guid.Parse(channelId) && 
                       v.video_Basics.PrivacyStatus == Accessibility.PUBLIC &&
                       (string.IsNullOrEmpty(query.Search) || 
                        v.video_Descriptive.Title.ToLower().Contains(query.Search)))
        {
            AddInclude(v => v.video_Basics);
            AddInclude(v => v.video_Descriptive);
            AddInclude(v => v.VideoStats);
            AddInclude(v => v.Temporal_Metadata);

            AddOrderByDescending(v => v.Temporal_Metadata.UploadDate);

            ApplyPaging((query.PageIndex - 1) * query.PageSize, query.PageSize);
        }
    }

    public class ChannelProfileSpecification : BaseSpecification<Channel, Guid>
    {
        public ChannelProfileSpecification(Guid channelId)
            : base(c => c.Id == channelId)
        {
            AddInclude(c => c.ChannelProfile);
        }

        public ChannelProfileSpecification(string ownerUserId)
            : base(c => c.Owner.Id == ownerUserId)
        {
            AddInclude(c => c.ChannelProfile);
        }
    }
}
