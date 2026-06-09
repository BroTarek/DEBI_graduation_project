using System;

namespace YouTubeClone.Domain.Aggregates.Videos
{
    public class video_Technical_details
    {
        public int Duration { get; private set; }
        public string Resolution { get; private set; }
        public long FileSize { get; private set; }
        public string ContainerFormat { get; private set; }
        public string VideoCodec { get; private set; }
        public string AudioCodec { get; private set; }
        public float FrameRate { get; private set; }
        public int BitRate { get; private set; }

        public video_Technical_details(
            int duration,
            string resolution,
            long fileSize,
            string containerFormat,
            string videoCodec,
            string audioCodec,
            float frameRate,
            int bitRate)
        {
            Duration = duration;
            Resolution = resolution;
            FileSize = fileSize;
            ContainerFormat = containerFormat;
            VideoCodec = videoCodec;
            AudioCodec = audioCodec;
            FrameRate = frameRate;
            BitRate = bitRate;
        }
    }

    public class video_Basics
    {
        public string VideoId { get; private set; }
        public string ThumbnailUrl { get; private set; }
        public string VideoUrl { get; private set; }
        public Accessibility PrivacyStatus { get; private set; }

        public video_Basics(string videoId, string thumbnailUrl, string videoUrl, Accessibility privacyStatus)
        {
            VideoId = videoId;
            ThumbnailUrl = thumbnailUrl;
            VideoUrl = videoUrl;
            PrivacyStatus = privacyStatus;
        }
    }

    public class video_Descriptive
    {
        public string Title { get; private set; }
        public string Description { get; private set; }
        public string Category { get; private set; }
        public string[] Tags { get; private set; }

        public video_Descriptive(string title, string description, string category, string[] tags)
        {
            Title = title;
            Description = description;
            Category = category;
            Tags = tags;
        }
    }

    public class Temporal_Metadata
    {
        public DateTime UploadDate { get; private set; }
        public DateTime UpateDate { get; private set; } // Matches the exact property name in the UML (upateDate)
        public string UploadStatus { get; private set; }

        public Temporal_Metadata(DateTime uploadDate, DateTime upateDate, string uploadStatus)
        {
            UploadDate = uploadDate;
            UpateDate = upateDate;
            UploadStatus = uploadStatus;
        }
    }

    public class VideoStats
    {
        public int WatchCount { get; private set; }
        public int LikesCount { get; private set; }
        public int DislikesCount { get; private set; }

        public VideoStats(int watchCount, int likesCount, int dislikesCount)
        {
            WatchCount = watchCount;
            LikesCount = likesCount;
            DislikesCount = dislikesCount;
        }
    }
}
