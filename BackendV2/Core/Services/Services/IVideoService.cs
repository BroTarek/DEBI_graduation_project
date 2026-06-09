using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace YouTubeClone.Domain.Services
{
    public interface IVideoService
    {
        Task<List<HomePageVideo>> GetHomePageVideosAsync(int skip, int take);
        Task<VideoWatchDto> GetWatchPageVideoAsync(Guid videoId);
        Task<Guid> UploadVideoAsync(UploadVideoDto dto, string channelId, string preferredProvider = "CLOUDINARY");
    }
}