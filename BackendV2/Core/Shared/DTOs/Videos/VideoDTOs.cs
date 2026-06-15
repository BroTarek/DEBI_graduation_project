using System;
using System.Collections.Generic;

namespace YouTubeClone.Shared.Dto_s
{
    public class WatchVideoDetailDTO
    {
        public string VideoId { get; set; } = string.Empty;
        public string VideoUrl { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public List<string> Tags { get; set; } = new();
        public int WatchCount { get; set; }
        public int LikesCount { get; set; }
        public int DislikesCount { get; set; }
        public string ChannelId { get; set; } = string.Empty;
        public string ChannelName { get; set; } = string.Empty;
        public string ChannelAvatar { get; set; } = string.Empty;
        public List<VideoCommentDTO> Comments { get; set; } = new();
    }

    public class VideoCommentDTO
    {
        public string CommentId { get; set; } = string.Empty;
        public string AuthorId { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string? ParentCommentId { get; set; }
        public List<VideoCommentDTO> Replies { get; set; } = new();
    }

    public class HomePageVideoDTO
    {
        public string VideoId { get; set; } = string.Empty;
        public string ThumbnailUrl { get; set; } = string.Empty;
        public int VideoLength { get; set; }
        public string VideoTitle { get; set; } = string.Empty;
        public int ViewCount { get; set; }
        public string ChannelAvatar { get; set; } = string.Empty;
        public string ChannelName { get; set; } = string.Empty;
    }

    public class WatchHistoryVideoDTO
    {
        public string VideoId { get; set; } = string.Empty;
        public string VideoUrl { get; set; } = string.Empty;
        public string VideoTitle { get; set; } = string.Empty;
        public int VideoLength { get; set; }
    }
}
