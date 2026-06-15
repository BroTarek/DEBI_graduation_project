using System;
using System.Threading.Tasks;
using YouTubeClone.Shared.Common;
using YouTubeClone.Shared.Common.Params;
using YouTubeClone.Shared.Dto_s;

namespace YouTubeClone.Services
{
    public interface ICommunityInteractionService
    {
        // Post Lifecycle Mechanics
        Task<ChannelPostDTO> CreatePostAsync(string channelId, string content, string accessibility);
        Task<bool> UpdatePostAsync(Guid postId, string content, string accessibility);
        Task<bool> DeletePostAsync(Guid postId);
        Task<Pagination<ChannelPostDTO>> GetChannelPostsAsync(string channelId, QueryParams queryParams);
        Task<ChannelPostDTO?> GetPostByIdAsync(Guid postId);

        // Comment Lifecycle Mechanics
        Task<CommentFeedDTO> CreateCommentAsync(string authorId, string targetId, string targetType, string content, string? parentCommentId = null);
        Task<bool> UpdateCommentAsync(string commentId, string content);
        Task<bool> DeleteCommentAsync(string commentId);
        Task<Pagination<CommentFeedDTO>> GetContentCommentsAsync(string targetId, string targetType, QueryParams queryParams);
    }
}
