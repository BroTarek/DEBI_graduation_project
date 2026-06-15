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


namespace YouTubeClone.Infrastructure.Persistence.Services.CommentService
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
            var commentRepo = _unitOfWork.GetRepo<Comment, CommentId>();
            
            var commentIdGuid = Guid.NewGuid();
            var comment = new Comment(
                new CommentId(commentIdGuid),
                commentIdGuid.ToString(),
                dto.UserId,
                dto.VideoId.ToString(),
                dto.Content,
                dto.ParentCommentId.HasValue ? new CommentId(dto.ParentCommentId.Value) : null
            );

            await commentRepo.AddAsync(comment);
            await _unitOfWork.SaveChangesAsync();
            return commentIdGuid;
        }

        public async Task<Guid> CreateCommentOnPost(CreatePostCommentDto dto)
        {
            var commentRepo = _unitOfWork.GetRepo<Comment, CommentId>();
            
            var commentIdGuid = Guid.NewGuid();
            var comment = new Comment(
                new CommentId(commentIdGuid),
                commentIdGuid.ToString(),
                dto.UserId,
                dto.PostId.ToString(),
                dto.Content,
                dto.ParentCommentId.HasValue ? new CommentId(dto.ParentCommentId.Value) : null
            );

            await commentRepo.AddAsync(comment);
            await _unitOfWork.SaveChangesAsync();
            return commentIdGuid;
        }

        public async Task<IEnumerable<CommentDto>> GetCommentsOnVideo(Guid videoId)
        {
            var commentRepo = _unitOfWork.GetRepo<Comment, CommentId>();
            var allComments = await commentRepo.GetAllAsync();
            
            var videoComments = allComments
                .Where(c => c.VideoId == videoId.ToString())
                .ToList();
                
            return BuildCommentTree(videoComments);
        }

        public async Task<IEnumerable<CommentDto>> GetCommentsOnPost(Guid postId)
        {
            var commentRepo = _unitOfWork.GetRepo<Comment, CommentId>();
            var allComments = await commentRepo.GetAllAsync();
            
            var postComments = allComments
                .Where(c => c.VideoId == postId.ToString())
                .ToList();
                
            return BuildCommentTree(postComments);
        }

        public async Task DeleteComment(Guid commentId)
        {
            var commentRepo = _unitOfWork.GetRepo<Comment, CommentId>();
            var allComments = await commentRepo.GetAllAsync();
            var commentToDelete = allComments.FirstOrDefault(c => c.Id.Value == commentId);
            if (commentToDelete != null)
            {
                var toDelete = new List<Comment>();
                GetRepliesRecursive(commentToDelete, allComments.ToList(), toDelete);
                toDelete.Add(commentToDelete);
                
                await commentRepo.DeleteRangeAsync(toDelete);
                await _unitOfWork.SaveChangesAsync();
            }
        }

        private void GetRepliesRecursive(Comment parent, List<Comment> allComments, List<Comment> toDelete)
        {
            var replies = allComments.Where(c => c.ParentCommentId != null && c.ParentCommentId.Value == parent.Id.Value).ToList();
            foreach (var reply in replies)
            {
                toDelete.Add(reply);
                GetRepliesRecursive(reply, allComments, toDelete);
            }
        }

        public async Task UpdateComment(UpdateCommentDto dto)
        {
            var commentRepo = _unitOfWork.GetRepo<Comment, CommentId>();
            var comment = await commentRepo.GetByIdAsync(new CommentId(dto.Id));
            if (comment != null)
            {
                comment.UpdateContent(dto.Content);
                await commentRepo.UpdateAsync(comment);
                await _unitOfWork.SaveChangesAsync();
            }
        }

        private IEnumerable<CommentDto> BuildCommentTree(List<Comment> comments)
        {
            var dtos = comments.Select(c => new CommentDto
            {
                Id = c.Id.Value,
                AuthorId = Guid.TryParse(c.AuthorId, out var authorGuid) ? authorGuid : Guid.Empty,
                Content = c.Content,
                CreatedAt = DateTime.UtcNow,
                Replies = new List<CommentDto>()
            }).ToList();

            var dtoLookup = dtos.ToDictionary(d => d.Id);
            var rootDtos = new List<CommentDto>();

            foreach (var comment in comments)
            {
                var dto = dtoLookup[comment.Id.Value];
                if (comment.ParentCommentId != null && dtoLookup.ContainsKey(comment.ParentCommentId.Value))
                {
                    var parentDto = dtoLookup[comment.ParentCommentId.Value];
                    parentDto.Replies.Add(dto);
                }
                else
                {
                    rootDtos.Add(dto);
                }
            }

            return rootDtos;
        }
    }
}