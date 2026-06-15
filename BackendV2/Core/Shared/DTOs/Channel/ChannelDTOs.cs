using System;

namespace YouTubeClone.Shared.Dto_s
{
    public class CreateChannelDTO
    {
        public string Name { get; set; } = string.Empty;
        public string ChannelsDescription { get; set; } = string.Empty;
        public string? Avatar { get; set; }
        public string? GreaterImg { get; set; }
        public string Links { get; set; } = string.Empty;
    }

    public class ChannelProfileDTO
    {
        public string ChannelId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string ChannelsDescription { get; set; } = string.Empty;
        public string Avatar { get; set; } = string.Empty;
        public string GreaterImg { get; set; } = string.Empty;
        public string Links { get; set; } = string.Empty;
        public int SubscribersCount { get; set; }
    }

    public class ChannelVideoItemDTO
    {
        public string VideoId { get; set; } = string.Empty;
        public string VideoTitle { get; set; } = string.Empty;
        public string ThumbnailUrl { get; set; } = string.Empty;
        public string VideoUrl { get; set; } = string.Empty;
        public int ViewCount { get; set; }
        public DateTime UploadDate { get; set; }
    }
}
