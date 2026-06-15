namespace YouTubeClone.Shared.Dto_s
{
    public class SubscribedChannelsDTO
    {
        public string ChannelId { get; set; } = string.Empty;
        public string ChannelName { get; set; } = string.Empty;
        public string ChannelAvatar { get; set; } = string.Empty;
        public int SubscribersCount { get; set; }
    }

    public class SubscribedChannelsVideosDTO
    {
        public string VideoId { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string ThumbnailUrl { get; set; } = string.Empty;
        public string VideoUrl { get; set; } = string.Empty;
        public int Duration { get; set; }
        public int WatchCount { get; set; }
        public string ChannelName { get; set; } = string.Empty;
        public string ChannelAvatar { get; set; } = string.Empty;
    }
}
