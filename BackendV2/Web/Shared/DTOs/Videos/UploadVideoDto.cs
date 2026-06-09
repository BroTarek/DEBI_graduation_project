public class UploadVideoDto
    {
        public string Title { get; set; } = null!;
        public string Description { get; set; } = string.Empty;
        public int DurationSeconds { get; set; }
        public string Category { get; set; } = string.Empty;
        public string Tags { get; set; } = string.Empty; // Comma separated list
        public IFormFile? VideoFile { get; set; }
        public IFormFile? ThumbnailFile { get; set; }
    }