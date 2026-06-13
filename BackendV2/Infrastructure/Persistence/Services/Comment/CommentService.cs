using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YouTubeClone.Core.Services;
using YouTubeClone.Core.Services.Specifications.CommentsSpec;
using YouTubeClone.Domain.Aggregates.Videos;
using YouTubeClone.Domain.Aggregates.Channels;
using YouTubeClone.Domain.Contracts.UOW;
using YouTubeClone.Domain.ValueObjects;


namespace YouTubeClone.Infrastructure.Persistence.Services.Comment
{
    public class CommentService : ICommentService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CommentService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Guid> CreateCommentOnVideo(CreateVideoCommentDto dto)
        {
            return Guid.NewGuid();
        }

        public async Task<Guid> CreateCommentOnPost(CreatePostCommentDto dto)
        {
            return Guid.NewGuid();
        }

        public async Task<IEnumerable<CommentDto>> GetCommentsOnVideo(Guid videoId)
        {
            return new List<CommentDto>();
        }

        public async Task<IEnumerable<CommentDto>> GetCommentsOnPost(Guid postId)
        {
            return new List<CommentDto>();
        }

        public async Task DeleteComment(Guid commentId)
        {
        }

        public async Task UpdateComment(UpdateCommentDto dto)
        {
        }
    }
}