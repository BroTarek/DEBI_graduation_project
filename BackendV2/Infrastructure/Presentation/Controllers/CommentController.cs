using YouTubeClone.Core.Services;
using YouTubeClone.Shared.DTOs.Comments;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace YouTubeClone.Presentation.Controllers
{
    [Authorize]
    public class CommentController : BaseController
    {
        private readonly ICommentService _commentService;

        public CommentController(ICommentService commentService)
        {
            _commentService = commentService;
        }

        [HttpPost("video")]
        public async Task<IActionResult> CreateCommentOnVideo([FromBody] CreateVideoCommentDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var commentId = await _commentService.CreateCommentOnVideo(dto);
            return Ok(new { Id = commentId });
        }

        [HttpPost("post")]
        public async Task<IActionResult> CreateCommentOnPost([FromBody] CreatePostCommentDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var commentId = await _commentService.CreateCommentOnPost(dto);
            return Ok(new { Id = commentId });
        }

        [HttpGet("video/{videoId}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetCommentsOnVideo(Guid videoId)
        {
            var comments = await _commentService.GetCommentsOnVideo(videoId);
            return Ok(comments);
        }

        [HttpGet("post/{postId}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetCommentsOnPost(Guid postId)
        {
            var comments = await _commentService.GetCommentsOnPost(postId);
            return Ok(comments);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteComment(Guid id)
        {
            await _commentService.DeleteComment(id);
            return NoContent();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateComment(Guid id, [FromBody] UpdateCommentDto dto)
        {
            if (id != dto.Id) return BadRequest("Id mismatch.");
            if (!ModelState.IsValid) return BadRequest(ModelState);

            await _commentService.UpdateComment(dto);
            return NoContent();
        }
    }
}
