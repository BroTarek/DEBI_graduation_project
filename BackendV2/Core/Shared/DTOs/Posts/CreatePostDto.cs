using System;
using YouTubeClone.Domain.Aggregates;

namespace YouTubeClone.Shared.DTOs.Posts
{
    public class CreatePostDto
    {
        public string ChannelId { get; set; } = null!;
        public string PostContent { get; set; } = null!;
        public Accessibility Accessibility { get; set; }
    }
}
