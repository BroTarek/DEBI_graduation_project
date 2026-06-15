using YouTubeClone.Domain.Enums;
using System;
using YouTubeClone.Domain.Entities;

namespace YouTubeClone.Shared.DTOs.Posts
{
    public class UpdatePostDto
    {
        public string PostContent { get; set; } = null!;
        public Accessibility Accessibility { get; set; }
    }
}
