 public class VideoDto
    {
        public Guid Id { get; set; }
        public Guid ChannelId { get; set; }
        public string Title { get; set; } = null!;
        public string Description { get; set; } = string.Empty;
        public int DurationSeconds { get; set; }
        public string ThumbnailUrl { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public List<string> Tags { get; set; } = new();
        public int ViewCount { get; set; }
        public DateTime CreatedAt { get; set; }
    }