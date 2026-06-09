namespace YouTubeClone.Domain.Aggregates.Channels
{
    public class ChannelProfile
    {
        public int SubscribersCount { get; private set; }
        public string ChannelsDescription { get; private set; }
        public string Links { get; private set; }
        public string Name { get; private set; }
        public string Avatar { get; private set; }
        public string GreaterImg { get; private set; }

        public ChannelProfile(int subscribersCount, string channelsDescription, string links, string name, string avatar, string greaterImg)
        {
            SubscribersCount = subscribersCount;
            ChannelsDescription = channelsDescription;
            Links = links;
            Name = name;
            Avatar = avatar;
            GreaterImg = greaterImg;
        }
    }
}
