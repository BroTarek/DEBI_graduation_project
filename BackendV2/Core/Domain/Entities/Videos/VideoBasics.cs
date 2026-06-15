using YouTubeClone.Domain.Enums;

namespace YouTubeClone.Domain.Entities.Videos
{
    public class VideoBasics
    {
        public Guid VideoId { get; set; }
        public string ThumbnailUrl { get; set; } = string.Empty;
        public string videoUrl { get; set; } = string.Empty;
        public Accessibility PrivacyStatus { get; set; }

        public VideoBasics() { }
        public VideoBasics(Guid videoId, string thumbnailUrl, string videoUrl, Accessibility privacyStatus)
        {
            VideoId = videoId;
            ThumbnailUrl = thumbnailUrl;
            this.videoUrl = videoUrl;
            PrivacyStatus = privacyStatus;
        }
    }
}
