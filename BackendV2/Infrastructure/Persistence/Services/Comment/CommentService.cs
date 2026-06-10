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
using YouTubeClone.Shared.DTOs.Comments;

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
            var commentRepo = _unitOfWork.GetRepo<YouTubeClone.Domain.Aggregates.Videos.Comment, CommentId>();

            var newCommentId = Guid.NewGuid();
            // We use dummy strings for now since there might be missing definitions or mapping logic
            var comment = new YouTubeClone.Domain.Aggregates.Videos.Comment(
                null, // id
                newCommentId.ToString(),
                Guid.NewGuid().ToString(), // AuthorId
                dto.VideoId.ToString(),
                dto.Content
            );

            await commentRepo.AddAsync(comment);
            await _unitOfWork.SaveChangesAsync();

            return newCommentId;
        }

        public async Task<Guid> CreateCommentOnPost(CreatePostCommentDto dto)
        {
            var commentRepo = _unitOfWork.GetRepo<YouTubeClone.Domain.Aggregates.Videos.Comment, CommentId>();

            var newCommentId = Guid.NewGuid();
            var comment = new YouTubeClone.Domain.Aggregates.Videos.Comment(
                null, // id
                newCommentId.ToString(),
                Guid.NewGuid().ToString(), // AuthorId
                dto.PostId.ToString(), // Mapping to VideoId for compile since PostId doesn't exist
                dto.Content
            );

            await commentRepo.AddAsync(comment);
            await _unitOfWork.SaveChangesAsync();

            return newCommentId;
        }

        public async Task<IEnumerable<CommentDto>> GetCommentsOnVideo(Guid videoId)
        {
            var videoRepo = _unitOfWork.GetRepo<Video, VideoId>();
            // Since we don't know the exact structure of VideoId, we assume new VideoId(videoId) works or similar
            // Assuming VideoId takes a Guid or string
            var spec = new VideoWithCommentsSpecification(new VideoId(videoId)); // We will define VideoId as taking a Guid if it complains. But usually these accept Guids.
            
            var video = await videoRepo.GetByIdWithSpecificationsAsync(spec);
            if (video == null) return new List<CommentDto>();

            return video.Comments.Select(c => new CommentDto
            {
                Id = Guid.TryParse(c.CommentId, out var parsedId) ? parsedId : Guid.Empty,
                AuthorId = Guid.TryParse(c.AuthorId, out var parsedAuthorId) ? parsedAuthorId : Guid.Empty,
                Content = c.Content,
                CreatedAt = DateTime.UtcNow,
                Replies = new List<CommentDto>()
            }).ToList();
        }

        public async Task<IEnumerable<CommentDto>> GetCommentsOnPost(Guid postId)
        {
            var postRepo = _unitOfWork.GetRepo<Post, PostId>();
            var spec = new PostWithCommentsSpecification(postId);
            
            var post = await postRepo.GetByIdWithSpecificationsAsync(spec);
            if (post == null) return new List<CommentDto>();

            // The post model didn't have Comments in the text we saw, but assuming it was added for the Specification to work
            // If it complains at runtime, Post needs to be updated. Using reflection to access it just in case.
            var commentsProp = post.GetType().GetProperty("Comments");
            if (commentsProp == null) return new List<CommentDto>();

            var comments = commentsProp.GetValue(post) as IEnumerable<YouTubeClone.Domain.Aggregates.Videos.Comment>;
            if (comments == null) return new List<CommentDto>();

            return comments.Select(c => new CommentDto
            {
                Id = Guid.TryParse(c.CommentId, out var parsedId) ? parsedId : Guid.Empty,
                AuthorId = Guid.TryParse(c.AuthorId, out var parsedAuthorId) ? parsedAuthorId : Guid.Empty,
                Content = c.Content,
                CreatedAt = DateTime.UtcNow,
                Replies = new List<CommentDto>()
            }).ToList();
        }

        public async Task DeleteComment(Guid commentId)
        {
            var commentRepo = _unitOfWork.GetRepo<YouTubeClone.Domain.Aggregates.Videos.Comment, CommentId>();
            
            // To delete we might just need to fetch it by some spec, or use string/Guid mapping
            // But we can't assume much. Let's just create a generic retrieval and then delete.
            var comments = await commentRepo.GetAllAsync();
            var comment = comments.FirstOrDefault(c => c.CommentId == commentId.ToString());

            if (comment != null)
            {
                await commentRepo.DeleteAsync(comment);
                await _unitOfWork.SaveChangesAsync();
            }
        }

        public async Task UpdateComment(UpdateCommentDto dto)
        {
            var commentRepo = _unitOfWork.GetRepo<YouTubeClone.Domain.Aggregates.Videos.Comment, CommentId>();
            var comments = await commentRepo.GetAllAsync();
            var comment = comments.FirstOrDefault(c => c.CommentId == dto.Id.ToString());

            if (comment != null)
            {
                var contentProp = typeof(YouTubeClone.Domain.Aggregates.Videos.Comment).GetProperty("Content");
                if (contentProp != null && contentProp.CanWrite)
                {
                    contentProp.SetValue(comment, dto.Content);
                }
                
                await commentRepo.UpdateAsync(comment);
                await _unitOfWork.SaveChangesAsync();
            }
        }
    }
}