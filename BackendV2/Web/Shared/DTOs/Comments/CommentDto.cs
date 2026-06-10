
    public class CommentDto
    {
        public Guid Id { get; set; }
        public Guid AuthorId { get; set; }
        public string Content { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public List<CommentDto> Replies { get; set; } = new();
    }