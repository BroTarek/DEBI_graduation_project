namespace YouTubeClone.Domain.Entities.Videos
{
    public class VideoTechnicalDetails
    {
        public int duration { get; set; }
        public string Resolution { get; set; } = string.Empty;
        public long FileSize { get; set; }
        public string ContainerFormat { get; set; } = string.Empty;
        public string VideoCodec { get; set; } = string.Empty;
        public string AudioCodec { get; set; } = string.Empty;
        public float FrameRate { get; set; }
        public int BitRate { get; set; }

        public VideoTechnicalDetails() { }
        public VideoTechnicalDetails(int duration, string resolution, long fileSize, string containerFormat, string videoCodec, string audioCodec, float frameRate, int bitRate)
        {
            this.duration = duration;
            Resolution = resolution;
            FileSize = fileSize;
            ContainerFormat = containerFormat;
            VideoCodec = videoCodec;
            AudioCodec = audioCodec;
            FrameRate = frameRate;
            BitRate = bitRate;
        }
    }
}
