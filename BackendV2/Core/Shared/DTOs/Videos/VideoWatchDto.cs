using System;

public class VideoWatchDto
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string VideoUrl { get; set; }
    public string ChannelName { get; set; }
    public string ChannelAvatarUrl { get; set; }
    public int Views { get; set; }
    public int Likes { get; set; }
    public int Dislikes { get; set; }
    public DateTime UploadDate { get; set; }
    public string ThumbnailUrl { get; set; } = string.Empty;
    public int Duration { get; set; }
}
