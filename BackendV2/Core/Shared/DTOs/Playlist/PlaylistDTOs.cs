using System;
using YouTubeClone.Shared.Common;

namespace YouTubeClone.Shared.Dto_s
{
    public class PlaylistVideosResultDTO
    {
        public string PlaylistName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string PlaylistThumbnailUrl { get; set; } = string.Empty;
        public Pagination<PlaylistVideoItemDTO> Videos { get; set; } = null!;
    }

    public class PlaylistVideoItemDTO
    {
        public string VideoId { get; set; } = string.Empty;
        public string VideoName { get; set; } = string.Empty;
        public string VideoUrl { get; set; } = string.Empty;
        public string ThumbnailUrl { get; set; } = string.Empty;
        public DateTime UploadDate { get; set; }
    }

    public class CompactPlaylistLookupDTO
    {
        public string PlaylistId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string ThumbnailUrl { get; set; } = string.Empty;
        public int VideosCount { get; set; }
    }
}
