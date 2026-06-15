namespace YouTubeClone.Domain.Entities.Channels
{
    public class ChannelProfile
    {
        public string name { get; set; } = string.Empty;
        public string channelsDescription { get; set; } = string.Empty;
        public string links { get; set; } = string.Empty;
        public int subscribersCount { get; set; }
        public string avatar { get; set; } = string.Empty;
        public string greaterImg { get; set; } = string.Empty;

        public ChannelProfile() { }
        public ChannelProfile(string name, string channelsDescription, string links, int subscribersCount, string avatar, string greaterImg)
        {
            this.name = name;
            this.channelsDescription = channelsDescription;
            this.links = links;
            this.subscribersCount = subscribersCount;
            this.avatar = avatar;
            this.greaterImg = greaterImg;
        }
    }
}
