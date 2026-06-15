using Microsoft.AspNetCore.Mvc;
using System;
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
    public class PostController : ControllerBase
    {
        private readonly ICommunityInteractionService _interactionService;
        public PostController(ICommunityInteractionService interactionService) => _interactionService = interactionService;

        [HttpPost("createPost")]
        public async Task<ActionResult<ApiResponse<ChannelPostDTO>>> CreatePost([FromQuery] string channelId, [FromBody] string content, [FromQuery] string accessibility = "Public")
        {
            var res = await _interactionService.CreatePostAsync(channelId, content, accessibility);
            return Ok(new ApiResponse<ChannelPostDTO>(res, "Community post published successfully."));
        }

        [HttpPut("updatePost/{id}")]
        public async Task<ActionResult<ApiResponse<object>>> UpdatePost(Guid id, [FromBody] string content, [FromQuery] string accessibility = "Public")
        {
            var ok = await _interactionService.UpdatePostAsync(id, content, accessibility);
            if (!ok) return BadRequest(new ApiResponse<object>("Target post could not be updated.", 400));
            return Ok(new ApiResponse<object>(new { UpdatedPostId = id }, "Post updated."));
        }

        [HttpDelete("deletePost/{id}")]
        public async Task<ActionResult<ApiResponse<object>>> DeletePost(Guid id)
        {
            var ok = await _interactionService.DeletePostAsync(id);
            if (!ok) return NotFound(new ApiResponse<object>("Target post not found.", 404));
            return Ok(new ApiResponse<object>(new { DeletedPostId = id }, "Post purged."));
        }

        [HttpGet("getAllChannelsPosts/{channelId}")]
        public async Task<ActionResult<ApiResponse<Pagination<ChannelPostDTO>>>> GetAllChannelsPosts(string channelId, [FromQuery] QueryParams queryParams)
        {
            var res = await _interactionService.GetChannelPostsAsync(channelId, queryParams);
            return Ok(new ApiResponse<Pagination<ChannelPostDTO>>(res, "Channel posts feed extracted."));
        }

        [HttpGet("getPost/{id}")]
        public async Task<ActionResult<ApiResponse<ChannelPostDTO>>> GetPost(Guid id)
        {
            var res = await _interactionService.GetPostByIdAsync(id);
            if (res == null) return NotFound(new ApiResponse<ChannelPostDTO>("Post could not be found.", 404));
            return Ok(new ApiResponse<ChannelPostDTO>(res, "Post single view loaded."));
        }
    }
}
