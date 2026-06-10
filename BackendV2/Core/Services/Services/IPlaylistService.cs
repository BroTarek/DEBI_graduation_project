using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using YouTubeClone.Shared.DTOs.Playlist; // Assuming this namespace exists based on DTO metadata

namespace YouTubeClone.Core.Services
{
    public interface IPlaylistService 
    {
        Task<Guid> CreateChannelPlaylist(Guid channelId, CreatePlaylistDto dto, Guid currentUserId);
        Task<Guid> CreateCustomPlaylist(Guid userId, CreatePlaylistDto dto, Guid currentUserId);
        Task DeletePlaylist(Guid playlistId, Guid currentUserId);
        Task AddVideoToPlaylist(Guid videoId, Guid playlistId, Guid currentUserId);
        Task RemoveVideoFromPlaylist(Guid videoId, Guid playlistId, Guid currentUserId);
        Task UpdatePlaylist(Guid playlistId, CreatePlaylistDto dto, Guid currentUserId);
        Task<List<PlaylistDto>> GetAllPlaylistsOfChannel(Guid channelId);
        Task<List<PlaylistDto>> GetAllPlaylistsCreatedByUser(Guid userId);
    }
}
