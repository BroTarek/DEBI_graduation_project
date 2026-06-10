using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using YouTubeClone.Shared.Responses;

namespace YouTubeClone.Core.Services
{
    public interface ISubscribtionService 
    {
        Task AddToSubscribtions(Guid ChannelID, Guid UserID);
        Task RemoveFromSubscribtions(Guid ChannelID, Guid UserID);
        Task<List<SubscribtionBadgeDTO>> GetAllSubscribedChannels(Guid UserID);
        Task<List<SubscribedChannelsVideosDTO>> GetAllSubscribedChannelsVideos(Guid UserID);
        Task<List<SubscribedChannelsPostsDTO>> GetAllSubscribedChannelsPosts(Guid UserID); 
    }
}