using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using YouTubeClone.Services;
using YouTubeClone.Shared.Common.Params;
using YouTubeClone.Shared.Dto_s;
using YouTubeClone.Shared.Responses;

namespace YouTubeClone.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PlaylistController : ControllerBase
    {
        private readonly IPlaylistService _playlistService;

        public PlaylistController(IPlaylistService playlistService)
        {
            _playlistService = playlistService;
        }

        [HttpPost("createPlaylist")]
        public async Task<ActionResult<ApiResponse<object>>> CreatePlaylist([FromQuery] Guid targetId, [FromQuery] string label, [FromQuery] string name, [FromQuery] string description)
        {
            var success = await _playlistService.CreatePlaylistAsync(targetId, label, name, description);
            if (!success) return BadRequest(new ApiResponse<object>("Failed to instantiate playlist.", 400));
            return Ok(new ApiResponse<object>(new { Status = "Created" }, "Playlist record generated cleanly."));
        }

        [HttpPost("addVideo/{playlistId}/{videoId}")]
        public async Task<ActionResult<ApiResponse<object>>> AddVideo(Guid playlistId, string videoId)
        {
            var success = await _playlistService.AddVideoToPlaylistAsync(playlistId, videoId);
            if (!success) return BadRequest(new ApiResponse<object>("Target playlist or video could not be identified.", 400));
            return Ok(new ApiResponse<object>(new { PlaylistId = playlistId }, "Video attached to compilation successfully."));
        }

        [HttpDelete("removeFromPlaylist/{playlistId}/{videoId}")]
        public async Task<ActionResult<ApiResponse<object>>> RemoveFromPlaylist(Guid playlistId, string videoId)
        {
            var success = await _playlistService.RemoveVideoFromPlaylistAsync(playlistId, videoId);
            if (!success) return BadRequest(new ApiResponse<object>("Video relationship reference target not found inside playlist items.", 400));
            return Ok(new ApiResponse<object>(new { Status = "Removed" }, "Video detached successfully."));
        }

        [HttpPut("clearPlaylist/{playlistId}")]
        public async Task<ActionResult<ApiResponse<object>>> ClearPlaylist(Guid playlistId)
        {
            var success = await _playlistService.ClearPlaylistAsync(playlistId);
            if (!success) return NotFound(new ApiResponse<object>("Failed to isolate compilation targeting index reference.", 404));
            return Ok(new ApiResponse<object>(new { ClearedPlaylistId = playlistId }, "All video association logs purged from playlist repository."));
        }

        [HttpGet("getVideosInPlaylist/{playlistId}")]
        public async Task<ActionResult<ApiResponse<PlaylistVideosResultDTO>>> GetVideosInPlaylist(Guid playlistId, [FromQuery] QueryParams queryParams)
        {
            var result = await _playlistService.GetVideosInPlaylistAsync(playlistId, queryParams);
            if (result == null) return NotFound(new ApiResponse<PlaylistVideosResultDTO>("No active playlist logs match this unique key.", 404));
            return Ok(new ApiResponse<PlaylistVideosResultDTO>(result, "Playlist details packet compiled successfully."));
        }

        [HttpGet("getAllPlaylists")]
        public async Task<ActionResult<ApiResponse<IEnumerable<CompactPlaylistLookupDTO>>>> GetAllPlaylists([FromQuery] string ownerId, [FromQuery] string label)
        {
            var playlists = await _playlistService.GetAllPlaylistsAsync(ownerId, label);
            return Ok(new ApiResponse<IEnumerable<CompactPlaylistLookupDTO>>(playlists, "Ownership tracking lists collected successfully."));
        }
    }
}
