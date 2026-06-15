using System;

namespace YouTubeClone.Domain.Entities.Videos
{
    public class TemporalMetadata
    {
        public DateTime UploadDate { get; set; }
        public DateTime UpdateDate { get; set; }
        public string UploadStatus { get; set; } = string.Empty;

        public TemporalMetadata() { }
        public TemporalMetadata(DateTime uploadDate, DateTime updateDate, string uploadStatus)
        {
            UploadDate = uploadDate;
            UpdateDate = updateDate;
            UploadStatus = uploadStatus;
        }
    }
}
