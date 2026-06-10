using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using YouTubeClone.Shared.DTOs.Posts;

namespace YouTubeClone.Core.Services
{
    public interface IPostService
    {
        Task<Guid> CreatePostAsync(CreatePostDto dto);
        Task UpdatePostAsync(Guid channelId, Guid postId, UpdatePostDto dto);
        Task DeletePostAsync(Guid channelId, Guid postId);
    }
}
