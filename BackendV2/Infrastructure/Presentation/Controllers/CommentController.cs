using Makanak.Domain.Contracts.UOW;
using Makanak.Presentation.Controllers;
using Makanak.Shared.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YouTubeClone.Domain.Aggregates.Channels;
using YouTubeClone.Domain.Aggregates.Videos;
using YouTubeClone.Domain.ValueObjects;

namespace YouTubeClone.Presentation.Controllers
{
    [Authorize]
    public class CommentController : BaseController
    {
        private readonly IUnitOfWork _unitOfWork;

        public CommentController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpPost]
        public async Task<IActionResult> PostComment([FromBody] PostCommentDto dto)
        {
            var userIdStr = GetUserId();
            if (string.IsNullOrEmpty(userIdStr) || !Guid.TryParse(userIdStr, out var userIdGuid))
            {
                return Unauthorized(new ApiResponse<string>("Unauthorized user.", 401));
            }

            var authorId = new UserId(userIdGuid);
            var videoId = new VideoId(dto.VideoId);

            var videoRepo = _unitOfWork.GetRepo<Video, VideoId>();
            
            // Fetch video using specification to include comments so we can modify the list
            var spec = new VideoWithCommentsSpecification(videoId);
            var video = await videoRepo.GetByIdWithSpecificationsAsync(spec);
            if (video == null)
            {
                return NotFound(new ApiResponse<string>("Video not found.", 404));
            }

            CommentId? parentCommentId = null;
            if (dto.ParentCommentId.HasValue)
            {
                parentCommentId = new CommentId(dto.ParentCommentId.Value);
            }

            var commentId = new CommentId(Guid.NewGuid());
            var comment = new Comment(commentId, authorId, dto.Content, parentCommentId);

            video.AddComment(comment);
            
            await videoRepo.UpdateAsync(video);
            await _unitOfWork.SaveChangesAsync();

            return Ok(new ApiResponse<Guid>(comment.Id.Value, "Comment posted successfully."));
        }

        [HttpGet("video/{videoId}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetComments(Guid videoId)
        {
            var videoRepo = _unitOfWork.GetRepo<Video, VideoId>();
            var spec = new VideoWithCommentsSpecification(new VideoId(videoId));
            var video = await videoRepo.GetByIdWithSpecificationsAsync(spec);
            if (video == null)
            {
                return NotFound(new ApiResponse<string>("Video not found.", 404));
            }

            var videoComments = video.Comments
                .Where(c => c.ParentCommentId == null) // Root comments
                .Select(c => new CommentDto
                {
                    Id = c.Id.Value,
                    AuthorId = c.AuthorId.Value,
                    Content = c.Content,
                    CreatedAt = c.CreatedAt,
                    Replies = c.Replies
                        .Select(r => new CommentDto
                        {
                            Id = r.Id.Value,
                            AuthorId = r.AuthorId.Value,
                            Content = r.Content,
                            CreatedAt = r.CreatedAt
                        })
                        .ToList()
                })
                .ToList();

            return Ok(new ApiResponse<List<CommentDto>>(videoComments, "Comments retrieved successfully."));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteComment(Guid id)
        {
            var userIdStr = GetUserId();
            if (string.IsNullOrEmpty(userIdStr) || !Guid.TryParse(userIdStr, out var userIdGuid))
            {
                return Unauthorized(new ApiResponse<string>("Unauthorized user.", 401));
            }

            var userId = new UserId(userIdGuid);
            var commentId = new CommentId(id);

            var commentRepo = _unitOfWork.GetRepo<Comment, CommentId>();
            var comment = await commentRepo.GetByIdAsync(commentId);
            if (comment == null)
            {
                return NotFound(new ApiResponse<string>("Comment not found.", 404));
            }

            // A comment can be deleted by its author, or the owner of the video it belongs to
            var isAuthor = comment.AuthorId.Value == userId.Value;
            var isVideoOwner = false;

            // Find the video this comment belongs to
            var videoRepo = _unitOfWork.GetRepo<Video, VideoId>();
            var allVideos = await videoRepo.GetAllAsync();
            var video = allVideos.FirstOrDefault(v => v.Comments.Any(c => c.Id.Value == id));

            if (video != null)
            {
                var channelRepo = _unitOfWork.GetRepo<Channel, ChannelId>();
                var channel = await channelRepo.GetByIdAsync(video.ChannelId);
                if (channel != null && channel.OwnerId.Value == userId.Value)
                {
                    isVideoOwner = true;
                }
            }

            if (!isAuthor && !isVideoOwner)
            {
                return Forbid();
            }

            await commentRepo.DeleteAsync(comment);
            await _unitOfWork.SaveChangesAsync();

            return Ok(new ApiResponse<string>("Comment deleted successfully."));
        }
    }

    public class VideoWithCommentsSpecification : SoftBridge.Services.Specification.BaseSpecification<Video, VideoId>
    {
        public VideoWithCommentsSpecification(VideoId videoId) : base(v => v.Id.Value == videoId.Value)
        {
            AddInclude("Comments");
            AddInclude("Comments.Replies");
        }
    }

    public class PostCommentDto
    {
        public Guid VideoId { get; set; }
        public string Content { get; set; } = null!;
        public Guid? ParentCommentId { get; set; }
    }

    public class CommentDto
    {
        public Guid Id { get; set; }
        public Guid AuthorId { get; set; }
        public string Content { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public List<CommentDto> Replies { get; set; } = new();
    }
}
