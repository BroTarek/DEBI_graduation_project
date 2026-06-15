using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YouTubeClone.Domain.Contracts.UOW;
using YouTubeClone.Domain.Entities.Channels;
using YouTubeClone.Domain.Aggregates.Videos;
using YouTubeClone.Domain.Enums;
using YouTubeClone.Services.Specifications;
using YouTubeClone.Shared.Common;
using YouTubeClone.Shared.Common.Params;
using YouTubeClone.Shared.Dto_s;

namespace YouTubeClone.Services
{
    public class CommunityInteractionService : ICommunityInteractionService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CommunityInteractionService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ChannelPostDTO> CreatePostAsync(string channelId, string content, string accessibility)
        {
            var repo = _unitOfWork.GetRepo<Post, Guid>();
            Enum.TryParse(accessibility, true, out Accessibility accessLevel);

            var post = new Post(Guid.NewGuid(), Guid.Parse(channelId), content, accessLevel);
            await repo.AddAsync(post);
            await _unitOfWork.SaveChangesAsync();

            return new ChannelPostDTO 
            { 
                PostId = post.Id, 
                PostContent = post.PostContent, 
                Accessibility = post.Accessibility.ToString() 
            };
        }

        public async Task<bool> UpdatePostAsync(Guid postId, string content, string accessibility)
        {
            var repo = _unitOfWork.GetRepo<Post, Guid>();
            var post = await repo.GetByIdAsync(postId);
            if (post == null) return false;

            Enum.TryParse(accessibility, true, out Accessibility accessLevel);
            post.Update(content, accessLevel);
            
            await repo.UpdateAsync(post);
            return await _unitOfWork.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeletePostAsync(Guid postId)
        {
            var repo = _unitOfWork.GetRepo<Post, Guid>();
            var post = await repo.GetByIdAsync(postId);
            if (post == null) return false;

            await repo.DeleteAsync(post);
            return await _unitOfWork.SaveChangesAsync() > 0;
        }

        public async Task<Pagination<ChannelPostDTO>> GetChannelPostsAsync(string channelId, QueryParams queryParams)
        {
            var repo = _unitOfWork.GetRepo<Post, Guid>();
            var countSpec = new ChannelPostsSpecification(channelId, queryParams);
            int total = await repo.CountAsync(countSpec);

            var dataSpec = new ChannelPostsSpecification(channelId, queryParams);
            var posts = await repo.GetAllWithSpecificationAsync(dataSpec);

            var dtos = posts.Select(p => new ChannelPostDTO
            {
                PostId = p.Id,
                PostContent = p.PostContent,
                Accessibility = p.Accessibility.ToString(),
                ChannelName = p.Channel?.ChannelProfile?.name ?? "Creator Space",
                ChannelAvatar = p.Channel?.ChannelProfile?.avatar ?? "https://api.dicebear.com/7.x/identicon/svg"
            });

            return new Pagination<ChannelPostDTO>(queryParams.PageIndex, queryParams.PageSize, total, dtos);
        }

        public async Task<ChannelPostDTO?> GetPostByIdAsync(Guid postId)
        {
            var repo = _unitOfWork.GetRepo<Post, Guid>();
            var spec = new ChannelPostsSpecification(postId);
            var post = await repo.GetByIdWithSpecificationsAsync(spec);
            if (post == null) return null;

            return new ChannelPostDTO 
            { 
                PostId = post.Id, 
                PostContent = post.PostContent, 
                Accessibility = post.Accessibility.ToString(),
                ChannelName = post.Channel?.ChannelProfile?.name ?? "Creator Space",
                ChannelAvatar = post.Channel?.ChannelProfile?.avatar ?? "https://api.dicebear.com/7.x/identicon/svg"
            };
        }

        public async Task<CommentFeedDTO> CreateCommentAsync(string authorId, string targetId, string targetType, string content, string? parentCommentId = null)
        {
            var repo = _unitOfWork.GetRepo<Comment, Guid>();

            Guid? parentGuid = null;
            if (!string.IsNullOrEmpty(parentCommentId))
            {
                parentGuid = Guid.Parse(parentCommentId);
            }

            Guid? videoId = targetType.ToLower() == "video" ? Guid.Parse(targetId) : null;
            Guid? postId = targetType.ToLower() == "post" ? Guid.Parse(targetId) : null;

            var comment = new Comment
            {
                Id = Guid.NewGuid(),
                AuthorId = authorId,
                VideoId = videoId,
                PostId = postId,
                Content = content,
                PublishTime = DateTime.UtcNow,
                ParentCommentId = parentGuid
            };

            await repo.AddAsync(comment);
            await _unitOfWork.SaveChangesAsync();

            return new CommentFeedDTO 
            { 
                CommentId = comment.Id.ToString(), 
                Content = comment.Content, 
                PublishTime = comment.PublishTime 
            };
        }

        public async Task<bool> UpdateCommentAsync(string commentId, string content)
        {
            var repo = _unitOfWork.GetRepo<Comment, Guid>();
            var comment = await repo.GetByIdAsync(Guid.Parse(commentId));
            if (comment == null) return false;

            comment.Content = content;
            
            await repo.UpdateAsync(comment);
            return await _unitOfWork.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteCommentAsync(string commentId)
        {
            var repo = _unitOfWork.GetRepo<Comment, Guid>();
            var comment = await repo.GetByIdAsync(Guid.Parse(commentId));
            if (comment == null) return false;

            await repo.DeleteAsync(comment);
            return await _unitOfWork.SaveChangesAsync() > 0;
        }

        public async Task<Pagination<CommentFeedDTO>> GetContentCommentsAsync(string targetId, string targetType, QueryParams queryParams)
        {
            var repo = _unitOfWork.GetRepo<Comment, Guid>();
            
            var countSpec = new ContentCommentsSpecification(targetId, targetType, queryParams);
            int total = await repo.CountAsync(countSpec);

            var dataSpec = new ContentCommentsSpecification(targetId, targetType, queryParams);
            var items = await repo.GetAllWithSpecificationAsync(dataSpec);

            var dtos = items.Select(c => MapCommentToFeedDTO(c));
            return new Pagination<CommentFeedDTO>(queryParams.PageIndex, queryParams.PageSize, total, dtos);
        }

        private CommentFeedDTO MapCommentToFeedDTO(Comment comment)
        {
            return new CommentFeedDTO
            {
                CommentId = comment.Id.ToString(),
                Content = comment.Content,
                PublishTime = comment.PublishTime,
                CommenterName = "User_" + (comment.AuthorId.Length >= 4 ? comment.AuthorId.Substring(0, 4) : comment.AuthorId),
                CommenterAvatar = $"https://api.dicebear.com/7.x/pixel-art/svg?seed={comment.AuthorId}",
                Replies = comment.Replies?.Select(r => MapCommentToFeedDTO(r)).ToList() ?? new List<CommentFeedDTO>()
            };
        }
    }
}
