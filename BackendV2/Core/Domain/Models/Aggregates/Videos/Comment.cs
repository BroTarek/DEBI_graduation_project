using System;
using System.Collections.Generic;
using YouTubeClone.Domain.Base;
using YouTubeClone.Domain.ValueObjects;

namespace YouTubeClone.Domain.Aggregates.Videos
{
    public class Comment : Entity<CommentId>
    {
        public UserId AuthorId { get; private set; }
        public string Content { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public CommentId? ParentCommentId { get; private set; }

        private readonly List<Comment> _replies = new();
        public IReadOnlyList<Comment> Replies => _replies.AsReadOnly();

        public Comment(CommentId id, UserId authorId, string content, CommentId? parentCommentId = null) : base(id)
        {
            AuthorId = authorId;
            Content = content;
            ParentCommentId = parentCommentId;
            CreatedAt = DateTime.UtcNow;
        }

        public void AddReply(Comment reply)
        {
            _replies.Add(reply);
        }
    }
}
