using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YouTubeClone.Core.Services;
using YouTubeClone.Core.Services.Specifications.PostSpec;
using YouTubeClone.Domain.Aggregates.Channels;
using YouTubeClone.Domain.Contracts.UOW;
using YouTubeClone.Shared.DTOs.Posts;

namespace YouTubeClone.Infrastructure.Persistence.Services.Post
{
    public class PostService : IPostService
    {
        private readonly IUnitOfWork _unitOfWork;

        public PostService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Guid> CreatePostAsync(CreatePostDto dto)
        {
            var postRepo = _unitOfWork.GetRepo<YouTubeClone.Domain.Aggregates.Channels.Post, PostId>();

            var newPostId = Guid.NewGuid();
            var post = new YouTubeClone.Domain.Aggregates.Channels.Post(
                new PostId(newPostId),
                dto.ChannelId,
                dto.PostContent,
                dto.Accessibility
            );

            await postRepo.AddAsync(post);
            await _unitOfWork.SaveChangesAsync();

            return newPostId;
        }

        public async Task UpdatePostAsync(Guid channelId, Guid postId, UpdatePostDto dto)
        {
            var postRepo = _unitOfWork.GetRepo<YouTubeClone.Domain.Aggregates.Channels.Post, PostId>();
            var spec = new PostByChannelAndIdSpecification(channelId, postId);

            var post = await postRepo.GetByIdWithSpecificationsAsync(spec);

            if (post != null)
            {
                // Note: The Post model properties are private set. 
                // In a real application, you'd call a method like post.UpdateContent(...)
                // Using reflection here for the mock setup, assuming the aggregate root will eventually have proper methods
                var contentProp = typeof(YouTubeClone.Domain.Aggregates.Channels.Post).GetProperty("PostContent");
                if (contentProp != null && contentProp.CanWrite)
                {
                    contentProp.SetValue(post, dto.PostContent);
                }

                var accessibilityProp = typeof(YouTubeClone.Domain.Aggregates.Channels.Post).GetProperty("Accessibility");
                if (accessibilityProp != null && accessibilityProp.CanWrite)
                {
                    accessibilityProp.SetValue(post, dto.Accessibility);
                }

                await postRepo.UpdateAsync(post);
                await _unitOfWork.SaveChangesAsync();
            }
        }

        public async Task DeletePostAsync(Guid channelId, Guid postId)
        {
            var postRepo = _unitOfWork.GetRepo<YouTubeClone.Domain.Aggregates.Channels.Post, PostId>();
            var spec = new PostByChannelAndIdSpecification(channelId, postId);

            var post = await postRepo.GetByIdWithSpecificationsAsync(spec);

            if (post != null)
            {
                await postRepo.DeleteAsync(post);
                await _unitOfWork.SaveChangesAsync();
            }
        }
    }
}
