using System;

public class ChannelVideoDto{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public int viewCount { get; set; }
    public string ThumbnailURL { get; set; }
    public DateTime PublishDate{get;set;}
}