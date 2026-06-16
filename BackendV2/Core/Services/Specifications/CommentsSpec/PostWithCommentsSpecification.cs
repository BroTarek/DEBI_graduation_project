using System;
using YouTubeClone.Domain.Aggregates.Channels;

namespace YouTubeClone.Core.Services.Specifications.CommentsSpec
{
    public class PostWithCommentsSpecification : SoftBridge.Services.Specification.BaseSpecification<Post, PostId>{
        public PostWithCommentsSpecification(Guid postId) : base(p => p.Id == new PostId(postId))
        {
            AddInclude("Comments");
            AddInclude("Comments.Replies");
        }
    }
}