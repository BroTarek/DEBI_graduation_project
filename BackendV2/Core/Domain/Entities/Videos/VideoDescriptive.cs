namespace YouTubeClone.Domain.Entities.Videos
{
    public class VideoDescriptive
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string[] Tags { get; set; } = [];

        public VideoDescriptive() { }
        public VideoDescriptive(string title, string description, string category, string[] tags)
        {
            Title = title;
            Description = description;
            Category = category;
            Tags = tags;
        }
    }
}
