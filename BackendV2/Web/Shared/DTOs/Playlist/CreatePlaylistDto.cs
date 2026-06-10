 public class CreatePlaylistDto
    {
        public string Name { get; set; } = null!;
        public string Description { get; set; } = string.Empty;
        public string? ThumbnailUrl { get; set; }
        public bool IsPublic { get; set; }
    }

   