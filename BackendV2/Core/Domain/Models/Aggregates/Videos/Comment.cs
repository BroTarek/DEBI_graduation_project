using System;
using YouTubeClone.Domain.Base;
using YouTubeClone.Domain.ValueObjects;

namespace YouTubeClone.Domain.Aggregates.Videos
{
    public class Comment : Entity<CommentId>
    {
        public string CommentId { get; private set; }
        public string AuthorId { get; private set; }
        public string VideoId { get; private set; }
        public string Content { get; private set; }
        public CommentId? ParentCommentId { get; private set; }
        public Comment? ParentComment { get; private set; }

        // EF Core requires a parameterless constructor
        private Comment() { }

        public Comment(CommentId id, string commentId, string authorId, string videoId, string content, CommentId? parentCommentId = null, Comment? parentComment = null) : base(id)
        {
            CommentId = commentId;
            AuthorId = authorId;
            VideoId = videoId;
            Content = content;
            ParentCommentId = parentCommentId;
            ParentComment = parentComment;
        }

        public void UpdateContent(string content)
        {
            Content = content;
        }
    }
}
