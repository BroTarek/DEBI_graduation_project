public class ChannelVideoDto{
    public Guid Id { get; private set; }
    public string Title { get; private set; }
    public int viewCount { get; private set; }
    public string ThumbnailURL { get; private set; }
    public Date PublishDate{get;private set;}
}