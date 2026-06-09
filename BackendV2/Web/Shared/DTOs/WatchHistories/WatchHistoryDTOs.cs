using System;

namespace YouTubeClone.Shared.DTOs.WatchHistories
{
    public class WatchHistoryVideoDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public int ViewCount { get; set; }
        public string ThumbnailURL { get; set; } = string.Empty;
        public DateTime PublishDate { get; set; }
    }

    public class WatchHistoryRequestDto
    {
        public Guid UserId { get; set; }
    }
}
