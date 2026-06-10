public class CreateVideoCommentDto{
    public Guid VideoId { get; set; }
    public string Content { get; set; } = null!;
    public Guid? ParentCommentId { get; set; }
}