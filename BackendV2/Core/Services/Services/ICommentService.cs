using System;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace YouTubeClone.Core.Services
{
    public interface ICommentService
    {
        Task<Guid> CreateCommentOnVideo(CreateVideoCommentDto dto);
        Task<Guid> CreateCommentOnPost(CreatePostCommentDto dto);
        Task<IEnumerable<CommentDto>> GetCommentsOnVideo(Guid videoId);
        Task<IEnumerable<CommentDto>> GetCommentsOnPost(Guid postId);
        Task DeleteComment(Guid commentId);
        Task UpdateComment(UpdateCommentDto dto);
    }
}
