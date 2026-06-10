using YouTubeClone.Core.Services;
using YouTubeClone.Presentation.Controllers;
using YouTubeClone.Shared.Responses;
using YouTubeClone.Shared.DTOs.Playlist;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace YouTubeClone.Presentation.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class PlaylistController : BaseController
    {
        private readonly IPlaylistService _playlistService;

        public PlaylistController(IPlaylistService playlistService)
        {
            _playlistService = playlistService;
        }

        [HttpPost("channel/{channelId}")]
        public async Task<IActionResult> CreateChannelPlaylist(Guid channelId, [FromBody] CreatePlaylistDto dto)
        {
            var userIdStr = GetUserId();
            if (string.IsNullOrEmpty(userIdStr) || !Guid.TryParse(userIdStr, out var userIdGuid))
                return Unauthorized(new ApiResponse<string>("Unauthorized user.", 401));

            var playlistId = await _playlistService.CreateChannelPlaylist(channelId, dto, userIdGuid);
            return Ok(new ApiResponse<Guid>(playlistId, "Channel Playlist created successfully."));
        }

        [HttpPost("custom")]
        public async Task<IActionResult> CreateCustomPlaylist([FromBody] CreatePlaylistDto dto)
        {
            var userIdStr = GetUserId();
            if (string.IsNullOrEmpty(userIdStr) || !Guid.TryParse(userIdStr, out var userIdGuid))
                return Unauthorized(new ApiResponse<string>("Unauthorized user.", 401));

            var playlistId = await _playlistService.CreateCustomPlaylist(userIdGuid, dto, userIdGuid);
            return Ok(new ApiResponse<Guid>(playlistId, "Custom Playlist created successfully."));
        }

        [HttpDelete("{playlistId}")]
        public async Task<IActionResult> DeletePlaylist(Guid playlistId)
        {
            var userIdStr = GetUserId();
            if (string.IsNullOrEmpty(userIdStr) || !Guid.TryParse(userIdStr, out var userIdGuid))
                return Unauthorized(new ApiResponse<string>("Unauthorized user.", 401));

            await _playlistService.DeletePlaylist(playlistId, userIdGuid);
            return Ok(new ApiResponse<string>("Playlist deleted successfully."));
        }

        [HttpPost("{playlistId}/videos/{videoId}")]
        public async Task<IActionResult> AddVideoToPlaylist(Guid playlistId, Guid videoId)
        {
            var userIdStr = GetUserId();
            if (string.IsNullOrEmpty(userIdStr) || !Guid.TryParse(userIdStr, out var userIdGuid))
                return Unauthorized(new ApiResponse<string>("Unauthorized user.", 401));

            await _playlistService.AddVideoToPlaylist(videoId, playlistId, userIdGuid);
            return Ok(new ApiResponse<string>("Video added to playlist successfully."));
        }

        [HttpDelete("{playlistId}/videos/{videoId}")]
        public async Task<IActionResult> RemoveVideoFromPlaylist(Guid playlistId, Guid videoId)
        {
            var userIdStr = GetUserId();
            if (string.IsNullOrEmpty(userIdStr) || !Guid.TryParse(userIdStr, out var userIdGuid))
                return Unauthorized(new ApiResponse<string>("Unauthorized user.", 401));

            await _playlistService.RemoveVideoFromPlaylist(videoId, playlistId, userIdGuid);
            return Ok(new ApiResponse<string>("Video removed from playlist successfully."));
        }

        [HttpPut("{playlistId}")]
        public async Task<IActionResult> UpdatePlaylist(Guid playlistId, [FromBody] CreatePlaylistDto dto)
        {
            var userIdStr = GetUserId();
            if (string.IsNullOrEmpty(userIdStr) || !Guid.TryParse(userIdStr, out var userIdGuid))
                return Unauthorized(new ApiResponse<string>("Unauthorized user.", 401));

            await _playlistService.UpdatePlaylist(playlistId, dto, userIdGuid);
            return Ok(new ApiResponse<string>("Playlist updated successfully."));
        }

        [HttpGet("channel/{channelId}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllPlaylistsOfChannel(Guid channelId)
        {
            var playlists = await _playlistService.GetAllPlaylistsOfChannel(channelId);
            return Ok(new ApiResponse<List<PlaylistDto>>(playlists, "Channel playlists retrieved."));
        }

        [HttpGet("user/{userId}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllPlaylistsCreatedByUser(Guid userId)
        {
            var playlists = await _playlistService.GetAllPlaylistsCreatedByUser(userId);
            return Ok(new ApiResponse<List<PlaylistDto>>(playlists, "User custom playlists retrieved."));
        }
    }
}
