using System;
using System.Collections.Generic;

namespace YouTubeClone.Shared.Dto_s
{
    public class ChannelPostDTO
    {
        public Guid PostId { get; set; }
        public string ChannelName { get; set; } = string.Empty;
        public string ChannelAvatar { get; set; } = string.Empty;
        public string PostContent { get; set; } = string.Empty;
        public string Accessibility { get; set; } = string.Empty;
    }

    public class CommentFeedDTO
    {
        public string CommentId { get; set; } = string.Empty;
        public string CommenterName { get; set; } = string.Empty;
        public string CommenterAvatar { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public DateTime PublishTime { get; set; }
        public List<CommentFeedDTO> Replies { get; set; } = new();
    }
}
