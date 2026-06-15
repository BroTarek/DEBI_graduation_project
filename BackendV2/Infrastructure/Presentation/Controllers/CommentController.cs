using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using YouTubeClone.Services;
using YouTubeClone.Shared.Common;
using YouTubeClone.Shared.Common.Params;
using YouTubeClone.Shared.Dto_s;
using YouTubeClone.Shared.Responses;

namespace YouTubeClone.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CommentController : ControllerBase
    {
        private readonly ICommunityInteractionService _interactionService;
        public CommentController(ICommunityInteractionService interactionService) => _interactionService = interactionService;

        [HttpPost("createCommentOnVideo/{videoId}")]
        public async Task<ActionResult<ApiResponse<CommentFeedDTO>>> CreateCommentOnVideo(string videoId, [FromBody] string content)
        {
            var mockUser = "11111111-1111-1111-1111-111111111111";
            var res = await _interactionService.CreateCommentAsync(mockUser, videoId, "video", content);
            return Ok(new ApiResponse<CommentFeedDTO>(res, "Comment dropped on publication successfully."));
        }

        [HttpPost("createCommentOnPost/{postId}")]
        public async Task<ActionResult<ApiResponse<CommentFeedDTO>>> CreateCommentOnPost(string postId, [FromBody] string content)
        {
            var mockUser = "11111111-1111-1111-1111-111111111111";
            var res = await _interactionService.CreateCommentAsync(mockUser, postId, "post", content);
            return Ok(new ApiResponse<CommentFeedDTO>(res, "Comment dropped on community message thread."));
        }

        [HttpPost("createReplyOnComment/{commentId}")]
        public async Task<ActionResult<ApiResponse<CommentFeedDTO>>> CreateReplyOnComment(string commentId, [FromQuery] string targetId, [FromQuery] string targetType, [FromBody] string content)
        {
            var mockUser = "11111111-1111-1111-1111-111111111111";
            var res = await _interactionService.CreateCommentAsync(mockUser, targetId, targetType, content, parentCommentId: commentId);
            return Ok(new ApiResponse<CommentFeedDTO>(res, "Threaded reply logged."));
        }

        [HttpPut("updateComment/{id}")]
        public async Task<ActionResult<ApiResponse<object>>> UpdateComment(string id, [FromBody] string content)
        {
            var ok = await _interactionService.UpdateCommentAsync(id, content);
            if (!ok) return BadRequest(new ApiResponse<object>("Comment update routine failed.", 400));
            return Ok(new ApiResponse<object>(new { TargetCommentId = id }, "Comment message updated."));
        }

        [HttpDelete("deleteComment/{id}")]
        public async Task<ActionResult<ApiResponse<object>>> DeleteComment(string id)
        {
            var ok = await _interactionService.DeleteCommentAsync(id);
            if (!ok) return NotFound(new ApiResponse<object>("Target comment non-existent.", 404));
            return Ok(new ApiResponse<object>(new { PurgedCommentId = id }, "Comment thread node dropped."));
        }

        [HttpGet("getAllCommentsOnPosts/{postId}")]
        public async Task<ActionResult<ApiResponse<Pagination<CommentFeedDTO>>>> GetAllCommentsOnPosts(string postId, [FromQuery] QueryParams queryParams)
        {
            var res = await _interactionService.GetContentCommentsAsync(postId, "post", queryParams);
            return Ok(new ApiResponse<Pagination<CommentFeedDTO>>(res, "Post comments gathered."));
        }

        [HttpGet("getAllCommentsOnVideos/{videoId}")]
        public async Task<ActionResult<ApiResponse<Pagination<CommentFeedDTO>>>> GetAllCommentsOnVideos(string videoId, [FromQuery] QueryParams queryParams)
        {
            var res = await _interactionService.GetContentCommentsAsync(videoId, "video", queryParams);
            return Ok(new ApiResponse<Pagination<CommentFeedDTO>>(res, "Video publication workspace comments feed loaded."));
        }
    }
}
