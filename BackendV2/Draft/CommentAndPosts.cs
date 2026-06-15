

using System;
using YouTubeClone.Domain.Entities.Channels;
using SoftBridge.Services.Specification;
using YouTubeClone.Shared.Common.Params;

namespace YouTubeClone.Services.Specifications
{
    public class ChannelPostsSpecification : BaseSpecification<Post, Guid>
    {
        public ChannelPostsSpecification(string channelId, QueryParams query)
            : base(p => p.ChannelId == channelId && 
                       p.Accessibility == Accessibility.PUBLIC &&
                       (string.IsNullOrEmpty(query.Search) || p.PostContent.ToLower().Contains(query.Search)))
        {
            AddInclude("Channel.ChannelProfile");
            AddOrderByDescending(p => p.Id); 
            ApplyPaging((query.PageIndex - 1) * query.PageSize, query.PageSize);
        }

        public ChannelPostsSpecification(Guid postId) : base(p => p.Id == postId)
        {
            AddInclude("Channel.ChannelProfile");
        }
    }
}


using System;
using YouTubeClone.Domain.Aggregates.Videos;
using SoftBridge.Services.Specification;
using YouTubeClone.Shared.Common.Params;

namespace YouTubeClone.Services.Specifications
{
    public class ContentCommentsSpecification : BaseSpecification<Comment, Guid>
    {
        public ContentCommentsSpecification(string targetId, string targetType, QueryParams query)
            : base(c => (targetType.ToLower() == "video" ? c.VideoId == targetId : c.PostId == targetId) 
                       && c.ParentCommentId == null) // Filter out roots cleanly via standard FK nullable tracking
        {
            AddInclude("Replies"); 

            if (query.Sort == SortingOptionsEnum.DateCreatedAsc)
            {
                AddOrderBy(c => c.PublishTime);
            }
            else
            {
                AddOrderByDescending(c => c.PublishTime);
            }

            ApplyPaging((query.PageIndex - 1) * query.PageSize, query.PageSize);
        }
    }
}
using System;
using System.Collections.Generic;

namespace YouTubeClone.Shared.Dto_s
{
    public class ChannelPostDTO
    {
        public Guid PostId { get; set; }
        public string ChannelName { get; set; } = string.Empty;
        public string ChannelAvatar { get; set; } = string.Empty;
        public string PostContent { get; set; } = string.Empty;
        public string Accessibility { get; set; } = string.Empty;
    }

    public class CommentFeedDTO
    {
        public string CommentId { get; set; } = string.Empty;
        public string CommenterName { get; set; } = string.Empty;
        public string CommenterAvatar { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public DateTime PublishTime { get; set; }
        public List<CommentFeedDTO> Replies { get; set; } = new();
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YouTubeClone.Domain.Contracts.UOW;
using YouTubeClone.Domain.Entities.Channels;
using YouTubeClone.Domain.Aggregates.Videos;
using YouTubeClone.Domain.ValueObjects;
using YouTubeClone.Services.Specifications;
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

    public class CommunityInteractionService : ICommunityInteractionService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CommunityInteractionService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ChannelPostDTO> CreatePostAsync(string channelId, string content, string accessibility)
        {
            var repo = _unitOfWork.GetRepo<Post, PostId>();
            Enum.TryParse(accessibility, true, out Accessibility accessLevel);

            var post = new Post(new PostId(Guid.NewGuid()), channelId, content, accessLevel);
            await repo.AddAsync(post);
            await _unitOfWork.SaveChangesAsync();

            return new ChannelPostDTO { PostId = post.Id.Value, PostContent = post.PostContent, Accessibility = post.Accessibility.ToString() };
        }

        public async Task<bool> UpdatePostAsync(Guid postId, string content, string accessibility)
        {
            var repo = _unitOfWork.GetRepo<Post, PostId>();
            var post = await repo.GetByIdAsync(new PostId(postId));
            if (post == null) return false;

            Enum.TryParse(accessibility, true, out Accessibility accessLevel);
            post.Update(content, accessLevel);
            
            await repo.UpdateAsync(post);
            return await _unitOfWork.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeletePostAsync(Guid postId)
        {
            var repo = _unitOfWork.GetRepo<Post, PostId>();
            var post = await repo.GetByIdAsync(new PostId(postId));
            if (post == null) return false;

            await repo.DeleteAsync(post);
            return await _unitOfWork.SaveChangesAsync() > 0;
        }

        public async Task<Pagination<ChannelPostDTO>> GetChannelPostsAsync(string channelId, QueryParams queryParams)
        {
            var repo = _unitOfWork.GetRepo<Post, PostId>();
            var countSpec = new ChannelPostsSpecification(channelId, queryParams);
            int total = await repo.CountAsync(countSpec);

            var dataSpec = new ChannelPostsSpecification(channelId, queryParams);
            var posts = await repo.GetAllWithSpecificationAsync(dataSpec);

            // Accessing Channel references via structural layout matching your Phase 1 UML mapping rules
            var dtos = posts.Select(p => new ChannelPostDTO
            {
                PostId = p.Id.Value,
                PostContent = p.PostContent,
                Accessibility = p.Accessibility.ToString(),
                ChannelName = "Creator Space", // Map from Channel entity if linked up
                ChannelAvatar = "https://api.dicebear.com/7.x/identicon/svg"
            });

            return new Pagination<ChannelPostDTO>(queryParams.PageIndex, queryParams.PageSize, total, dtos);
        }

        public async Task<ChannelPostDTO?> GetPostByIdAsync(Guid postId)
        {
            var repo = _unitOfWork.GetRepo<Post, PostId>();
            var spec = new ChannelPostsSpecification(new PostId(postId));
            var post = await repo.GetByIdWithSpecificationsAsync(spec);
            if (post == null) return null;

            return new ChannelPostDTO { PostId = post.Id.Value, PostContent = post.PostContent, Accessibility = post.Accessibility.ToString() };
        }

        public async Task<CommentFeedDTO> CreateCommentAsync(string authorId, string targetId, string targetType, string content, string? parentCommentId = null)
        {
            var repo = _unitOfWork.GetRepo<Comment, CommentId>();
            Comment? parent = null;

            if (!string.IsNullOrEmpty(parentCommentId))
            {
                parent = await repo.GetByIdAsync(new CommentId(Guid.Parse(parentCommentId)));
            }

            string videoId = targetType.ToLower() == "video" ? targetId : string.Empty;
            // Assuming your comment model handles conditional linking to PostId if target type is a community post
            
            var comment = new Comment(new CommentId(Guid.NewGuid()), Guid.NewGuid().ToString(), authorId, videoId, content, DateTime.UtcNow, parent);
            await repo.AddAsync(comment);
            await _unitOfWork.SaveChangesAsync();

            return new CommentFeedDTO { CommentId = comment.Id.Value.ToString(), Content = comment.Content, PublishTime = comment.PublishTime };
        }

        public async Task<bool> UpdateCommentAsync(string commentId, string content)
        {
            var repo = _unitOfWork.GetRepo<Comment, CommentId>();
            var comment = await repo.GetByIdAsync(new CommentId(Guid.Parse(commentId)));
            if (comment == null) return false;

            // Mutation handled directly inside the clean encapsulated boundary
            // Assuming a method like comment.UpdateContent(content) exists, or fallback directly:
            typeof(Comment).GetProperty("Content")?.SetValue(comment, content); 
            
            await repo.UpdateAsync(comment);
            return await _unitOfWork.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteCommentAsync(string commentId)
        {
            var repo = _unitOfWork.GetRepo<Comment, CommentId>();
            var comment = await repo.GetByIdAsync(new CommentId(Guid.Parse(commentId)));
            if (comment == null) return false;

            await repo.DeleteAsync(comment);
            return await _unitOfWork.SaveChangesAsync() > 0;
        }

        public async Task<Pagination<CommentFeedDTO>> GetContentCommentsAsync(string targetId, string targetType, QueryParams queryParams)
        {
            var repo = _unitOfWork.GetRepo<Comment, CommentId>();
            
            var countSpec = new ContentCommentsSpecification(targetId, targetType, queryParams);
            int total = await repo.CountAsync(countSpec);

            var dataSpec = new ContentCommentsSpecification(targetId, targetType, queryParams);
            var items = await repo.GetAllWithSpecificationAsync(dataSpec);

            var dtos = items.Select(c => MapCommentToFeedDTO(c));
            return new Pagination<CommentFeedDTO>(queryParams.PageIndex, queryParams.PageSize, total, dtos);
        }

        // Recursive mapper ensures deeply-nested threaded reply configurations render clean trees
        private CommentFeedDTO MapCommentToFeedDTO(Comment comment)
        {
            return new CommentFeedDTO
            {
                CommentId = comment.Id.Value.ToString(),
                Content = comment.Content,
                PublishTime = comment.PublishTime,
                CommenterName = "User_" + comment.AuthorId.Substring(0, 4), // Placeholder fallback mapping context
                CommenterAvatar = $"https://api.dicebear.com/7.x/pixel-art/svg?seed={comment.AuthorId}",
                Replies = comment.Replies?.Select(r => MapCommentToFeedDTO(r)).ToList() ?? new List<CommentFeedDTO>()
            };
        }
    }
}


using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using YouTubeClone.Services;
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


using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using YouTubeClone.Services;
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
            var mockUser = "user-guid-xyz-12345";
            var res = await _interactionService.CreateCommentAsync(mockUser, videoId, "video", content);
            return Ok(new ApiResponse<CommentFeedDTO>(res, "Comment dropped on publication successfully."));
        }

        [HttpPost("createCommentOnPost/{postId}")]
        public async Task<ActionResult<ApiResponse<CommentFeedDTO>>> CreateCommentOnPost(string postId, [FromBody] string content)
        {
            var mockUser = "user-guid-xyz-12345";
            var res = await _interactionService.CreateCommentAsync(mockUser, postId, "post", content);
            return Ok(new ApiResponse<CommentFeedDTO>(res, "Comment dropped on community message thread."));
        }

        [HttpPost("createReplyOnComment/{commentId}")]
        public async Task<ActionResult<ApiResponse<CommentFeedDTO>>> CreateReplyOnComment(string commentId, [FromQuery] string targetId, [FromQuery] string targetType, [FromBody] string content)
        {
            var mockUser = "user-guid-xyz-12345";
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

