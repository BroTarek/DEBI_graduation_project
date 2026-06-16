using System;
using YouTubeClone.Domain.Aggregates.Channels;

namespace YouTubeClone.Core.Services.Specifications.PostSpec
{
    public class PostByChannelAndIdSpecification : SoftBridge.Services.Specification.BaseSpecification<Post, PostId>
    {
        public PostByChannelAndIdSpecification(Guid channelId, Guid postId) 
            : base(p => p.ChannelId == channelId.ToString() && p.Id == new PostId(postId))
        {
        }
    }
}
