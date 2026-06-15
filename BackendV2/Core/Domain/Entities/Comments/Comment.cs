using System;
using System.Collections.Generic;
using YouTubeClone.Domain.Base;

namespace YouTubeClone.Domain.Aggregates.Videos
{
    public class Comment : Entity<Guid>
    {
        public string AuthorId { get; set; } = string.Empty;
        public Guid? VideoId { get; set; }
        public Guid? PostId { get; set; } // Clean support for community posts side
        public string Content { get; set; } = string.Empty;
        public DateTime PublishTime { get; set; } = DateTime.UtcNow;

        // Self-referential navigation parameters for threaded replies
        public Guid? ParentCommentId { get; set; }
        public virtual Comment? ParentComment { get; set; }
        public virtual ICollection<Comment> Replies { get; set; } = new List<Comment>();

        public Comment() { }
        public Comment(Guid id) : base(id) { }
    }
}