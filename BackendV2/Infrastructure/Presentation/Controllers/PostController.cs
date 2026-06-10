using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using YouTubeClone.Core.Services;
using YouTubeClone.Shared.DTOs.Posts;

namespace YouTubeClone.Presentation.Controllers
{
    [Authorize]
    [Route("api/channels/{channelId}/posts")]
    public class PostController : BaseController
    {
        private readonly IPostService _postService;
        private readonly ICommentService _commentService;

        public PostController(IPostService postService, ICommentService commentService)
        {
            _postService = postService;
            _commentService = commentService;
        }

        [HttpPost]
        public async Task<IActionResult> CreatePost(Guid channelId, [FromBody] CreatePostDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            
            // Override channelId from route if needed, or ensure they match
            dto.ChannelId = channelId.ToString();

            var postId = await _postService.CreatePostAsync(dto);
            return Ok(new { Id = postId });
        }

        [HttpGet("{postId}/comments")]
        [AllowAnonymous]
        public async Task<IActionResult> GetCommentsOnPost(Guid channelId, Guid postId)
        {
            // channelId might not be directly used by GetCommentsOnPost but it's part of the route
            var comments = await _commentService.GetCommentsOnPost(postId);
            return Ok(comments);
        }

        [HttpDelete("{postId}")]
        public async Task<IActionResult> DeletePost(Guid channelId, Guid postId)
        {
            await _postService.DeletePostAsync(channelId, postId);
            return NoContent();
        }

        [HttpPut("{postId}")]
        public async Task<IActionResult> UpdatePost(Guid channelId, Guid postId, [FromBody] UpdatePostDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            await _postService.UpdatePostAsync(channelId, postId, dto);
            return NoContent();
        }
    }
}
