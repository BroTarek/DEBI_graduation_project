using System;
using YouTubeClone.Domain.Entities.Channels;
using YouTubeClone.Domain.Aggregates.Videos;
using YouTubeClone.Domain.Enums;
using SoftBridge.Services.Specification;
using YouTubeClone.Shared.Common.Params;
using YouTubeClone.Shared.Common;

namespace YouTubeClone.Services.Specifications
{
    public class ChannelPostsSpecification : BaseSpecification<Post, Guid>
    {
        public ChannelPostsSpecification(string channelId, QueryParams query)
            : base(p => p.ChannelId == Guid.Parse(channelId) && 
                       p.Accessibility == Accessibility.PUBLIC &&
                       (string.IsNullOrEmpty(query.Search) || p.PostContent.ToLower().Contains(query.Search)))
        {
            AddInclude("Channel.ChannelProfile");
            AddOrderByDescending(p => p.Id); 
            ApplyPaging((query.PageIndex - 1) * query.PageSize, query.PageSize);
        }

        public ChannelPostsSpecification(Guid postId) : base(p => p.Id == postId)
        {
            AddInclude("Channel.ChannelProfile");
        }
    }

    public class ContentCommentsSpecification : BaseSpecification<Comment, Guid>
    {
        public ContentCommentsSpecification(string targetId, string targetType, QueryParams query)
            : base(c => (targetType.ToLower() == "video" ? c.VideoId == Guid.Parse(targetId) : c.PostId == Guid.Parse(targetId)) 
                       && c.ParentCommentId == null)
        {
            AddInclude("Replies"); 

            if (query.Sort == SortingOptionsEnum.DateCreatedAsc)
            {
                AddOrderBy(c => c.PublishTime);
            }
            else
            {
                AddOrderByDescending(c => c.PublishTime);
            }

            ApplyPaging((query.PageIndex - 1) * query.PageSize, query.PageSize);
        }
    }
}
