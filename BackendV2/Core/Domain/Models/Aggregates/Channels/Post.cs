using System;
using YouTubeClone.Domain.Base;

namespace YouTubeClone.Domain.Aggregates.Channels
{
    public record PostId(Guid Value);

    public class Post : Entity<PostId>
    {
        public string ChannelId { get; private set; }
        public string PostContent { get; private set; }
        public Accessibility Accessibility { get; private set; }

        public Post(PostId id, string channelId, string postContent, Accessibility accessibility) : base(id)
        {
            ChannelId = channelId;
            PostContent = postContent;
            Accessibility = accessibility;
        }
    }
}
