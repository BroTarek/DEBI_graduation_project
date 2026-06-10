using System;
using YouTubeClone.Domain.Aggregates;

namespace YouTubeClone.Shared.DTOs.Posts
{
    public class UpdatePostDto
    {
        public string PostContent { get; set; } = null!;
        public Accessibility Accessibility { get; set; }
    }
}
