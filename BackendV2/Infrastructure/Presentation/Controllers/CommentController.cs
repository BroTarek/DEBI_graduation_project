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
        public async Task<IActionResult> CreateComment([FromBody] PostCommentDto dto)
        {
            //is this comment a direct one or a reply to another comment
            //is this comment to a video or a post
        }

        [HttpGet("video/{videoId}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetComments(Guid videoId)
        {
            //get the comments of a post or a video
            //get nested replies

        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteComment(Guid id)
        {
            
            //is this comment a direct one or a reply to another comment,

            //is this comment to a video or a post

            // deleteing this comment should delete depending comments

        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateComment(Guid id)
        {

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

    public class CreateVideoCommentDto
    {
        public Guid VideoId { get; set; }
        public string Content { get; set; } = null!;
        public Guid? ParentCommentId { get; set; }
    }
    public class CreatePostCommentDto
    {
        public Guid PostId { get; set; }
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
    public class UpdateCommentDto
    {
        public Guid Id { get; set; }
        public string Content { get; set; } = null!;
    }

    public class DeleteCommentDto
    {
        public Guid Id { get; set; }
    }
}
