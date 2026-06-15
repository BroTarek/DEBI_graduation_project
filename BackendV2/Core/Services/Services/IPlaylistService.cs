using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using YouTubeClone.Shared.Common.Params;
using YouTubeClone.Shared.Dto_s;

namespace YouTubeClone.Services
{
    public interface IPlaylistService
    {
        Task<bool> CreatePlaylistAsync(Guid targetId, string label, string name, string description);
        Task<bool> AddVideoToPlaylistAsync(Guid playlistId, string videoId);
        Task<bool> RemoveVideoFromPlaylistAsync(Guid playlistId, string videoId);
        Task<bool> ClearPlaylistAsync(Guid playlistId);
        Task<PlaylistVideosResultDTO?> GetVideosInPlaylistAsync(Guid playlistId, QueryParams queryParams);
        Task<IEnumerable<CompactPlaylistLookupDTO>> GetAllPlaylistsAsync(string ownerId, string label);
    }
}
