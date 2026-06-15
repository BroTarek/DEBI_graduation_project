using System;
using System.Threading.Tasks;
using YouTubeClone.Shared.Common;
using YouTubeClone.Shared.Common.Params;
using YouTubeClone.Shared.Dto_s;

namespace YouTubeClone.Services
{
    public interface IVideoService
    {
        Task<Guid> UploadVideoAsync(UploadVideoDto dto, string channelId, string preferredProvider = "CLOUDINARY");
        Task<Pagination<HomePageVideoDTO>> GetHomePageVideosAsync(QueryParams queryParams);
        Task<WatchVideoDetailDTO?> WatchVideoAsync(Guid videoId, Guid userId);
    }
}