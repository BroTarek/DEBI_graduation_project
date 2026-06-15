using System;
using YouTubeClone.Domain.Base;
using YouTubeClone.Domain.Enums;
using YouTubeClone.Domain.Entities.Channels;

namespace YouTubeClone.Domain.Entities.Channels
{
    public class Post : Entity<Guid>
    {
        public Guid ChannelId { get; set; }
        public string PostContent { get; set; } = string.Empty;
        public Accessibility Accessibility { get; set; }

        public virtual Channel? Channel { get; set; }

        public Post() { }
        public Post(Guid id, Guid channelId, string postContent, Accessibility accessibility) : base(id)
        {
            ChannelId = channelId;
            PostContent = postContent;
            Accessibility = accessibility;
        }

        public void Update(string postContent, Accessibility accessibility)
        {
            PostContent = postContent;
            Accessibility = accessibility;
        }
    }
}