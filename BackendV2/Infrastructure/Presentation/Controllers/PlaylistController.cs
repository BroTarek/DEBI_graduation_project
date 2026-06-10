using YouTubeClone.Domain.Contracts.UOW;
using YouTubeClone.Presentation.Controllers;
using YouTubeClone.Shared.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YouTubeClone.Domain.Aggregates.Channels;
using YouTubeClone.Domain.Aggregates.Playlists;
using YouTubeClone.Domain.Aggregates.Videos;
using YouTubeClone.Domain.ValueObjects;

namespace YouTubeClone.Presentation.Controllers
{
    [Authorize]
    public class PlaylistController : BaseController
    {
        private readonly IUnitOfWork _unitOfWork;

        public PlaylistController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpPost]
        public async Task<IActionResult> CreatePlaylist([FromBody] CreatePlaylistDto dto)
        {
            var userIdStr = GetUserId();
            if (string.IsNullOrEmpty(userIdStr) || !Guid.TryParse(userIdStr, out var userIdGuid))
            {
                return Unauthorized(new ApiResponse<string>("Unauthorized user.", 401));
            }

            var userId = new UserId(userIdGuid);

            // Find user's channel to associate the playlist with
            var channelRepo = _unitOfWork.GetRepo<Channel, ChannelId>();
            var allChannels = await channelRepo.GetAllAsync();
            var channel = allChannels.FirstOrDefault(c => c.OwnerId.Value == userId.Value);
            if (channel == null)
            {
                return BadRequest(new ApiResponse<string>("You must create a channel before creating playlists.", 400));
            }

            var playlistId = new PlaylistId(Guid.NewGuid());
            var name = new ChannelName(dto.Name); // Name is typed as ChannelName in Domain
            var description = new Description(dto.Description);
            var thumbnailUrl = new ThumbnailUrl(dto.ThumbnailUrl ?? "");

            var playlist = new Playlist(playlistId, channel.Id, name, description, thumbnailUrl, dto.IsPublic);

            var playlistRepo = _unitOfWork.GetRepo<Playlist, PlaylistId>();
            await playlistRepo.AddAsync(playlist);
            await _unitOfWork.SaveChangesAsync();

            return Ok(new ApiResponse<Guid>(playlist.Id.Value, "Playlist created successfully."));
        }

        [HttpPost("{playlistId}/videos/{videoId}")]
        public async Task<IActionResult> AddVideoToPlaylist(Guid playlistId, Guid videoId)
        {
            var playlistRepo = _unitOfWork.GetRepo<Playlist, PlaylistId>();
            
            // Load playlist with video items included
            var spec = new PlaylistWithItemsSpecification(new PlaylistId(playlistId));
            var playlist = await playlistRepo.GetByIdWithSpecificationsAsync(spec);
            if (playlist == null)
            {
                return NotFound(new ApiResponse<string>("Playlist not found.", 404));
            }

            // Verify owner
            var userIdStr = GetUserId();
            if (string.IsNullOrEmpty(userIdStr) || !Guid.TryParse(userIdStr, out var userIdGuid))
            {
                return Unauthorized(new ApiResponse<string>("Unauthorized user.", 401));
            }
            var userId = new UserId(userIdGuid);

            var channelRepo = _unitOfWork.GetRepo<Channel, ChannelId>();
            var channel = await channelRepo.GetByIdAsync(playlist.ChannelId);
            if (channel == null || channel.OwnerId.Value != userId.Value)
            {
                return Forbid();
            }

            // Verify video exists
            var videoRepo = _unitOfWork.GetRepo<Video, VideoId>();
            var video = await videoRepo.GetByIdAsync(new VideoId(videoId));
            if (video == null)
            {
                return NotFound(new ApiResponse<string>("Video not found.", 404));
            }

            int nextPosition = playlist.VideoItems.Count + 1;
            playlist.AddVideo(new VideoId(videoId), nextPosition);

            await playlistRepo.UpdateAsync(playlist);
            await _unitOfWork.SaveChangesAsync();

            return Ok(new ApiResponse<string>("Video added to playlist successfully."));
        }

        [HttpDelete("{playlistId}/videos/{videoId}")]
        public async Task<IActionResult> RemoveVideoFromPlaylist(Guid playlistId, Guid videoId)
        {
            var playlistRepo = _unitOfWork.GetRepo<Playlist, PlaylistId>();
            var spec = new PlaylistWithItemsSpecification(new PlaylistId(playlistId));
            var playlist = await playlistRepo.GetByIdWithSpecificationsAsync(spec);
            if (playlist == null)
            {
                return NotFound(new ApiResponse<string>("Playlist not found.", 404));
            }

            // Verify owner
            var userIdStr = GetUserId();
            if (string.IsNullOrEmpty(userIdStr) || !Guid.TryParse(userIdStr, out var userIdGuid))
            {
                return Unauthorized(new ApiResponse<string>("Unauthorized user.", 401));
            }
            var userId = new UserId(userIdGuid);

            var channelRepo = _unitOfWork.GetRepo<Channel, ChannelId>();
            var channel = await channelRepo.GetByIdAsync(playlist.ChannelId);
            if (channel == null || channel.OwnerId.Value != userId.Value)
            {
                return Forbid();
            }

            playlist.RemoveVideo(new VideoId(videoId));
            await playlistRepo.UpdateAsync(playlist);
            await _unitOfWork.SaveChangesAsync();

            return Ok(new ApiResponse<string>("Video removed from playlist successfully."));
        }

        [HttpGet("{playlistId}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetPlaylist(Guid playlistId)
        {
            var playlistRepo = _unitOfWork.GetRepo<Playlist, PlaylistId>();
            var spec = new PlaylistWithItemsSpecification(new PlaylistId(playlistId));
            var playlist = await playlistRepo.GetByIdWithSpecificationsAsync(spec);
            if (playlist == null)
            {
                return NotFound(new ApiResponse<string>("Playlist not found.", 404));
            }

            if (!playlist.IsPublic)
            {
                var userIdStr = GetUserId();
                if (string.IsNullOrEmpty(userIdStr) || !Guid.TryParse(userIdStr, out var userIdGuid))
                {
                    return Forbid();
                }
                var userId = new UserId(userIdGuid);
                var channelRepo = _unitOfWork.GetRepo<Channel, ChannelId>();
                var channel = await channelRepo.GetByIdAsync(playlist.ChannelId);
                if (channel == null || channel.OwnerId.Value != userId.Value)
                {
                    return Forbid();
                }
            }

            return Ok(new ApiResponse<PlaylistDto>(new PlaylistDto
            {
                Id = playlist.Id.Value,
                ChannelId = playlist.ChannelId.Value,
                Name = playlist.Name.Value,
                Description = playlist.Description.Value,
                ThumbnailUrl = playlist.ThumbnailUrl.Value,
                IsPublic = playlist.IsPublic,
                VideoIds = playlist.VideoItems.Select(vi => vi.VideoId.Value).ToList()
            }, "Playlist retrieved successfully."));
        }
    }

    public class PlaylistWithItemsSpecification : SoftBridge.Services.Specification.BaseSpecification<Playlist, PlaylistId>
    {
        public PlaylistWithItemsSpecification(PlaylistId playlistId) : base(p => p.Id.Value == playlistId.Value)
        {
            AddInclude("VideoItems");
        }
    }

    public class CreatePlaylistDto
    {
        public string Name { get; set; } = null!;
        public string Description { get; set; } = string.Empty;
        public string? ThumbnailUrl { get; set; }
        public bool IsPublic { get; set; }
    }

    public class PlaylistDto
    {
        public Guid Id { get; set; }
        public Guid ChannelId { get; set; }
        public string Name { get; set; } = null!;
        public string Description { get; set; } = string.Empty;
        public string ThumbnailUrl { get; set; } = string.Empty;
        public bool IsPublic { get; set; }
        public List<Guid> VideoIds { get; set; } = new();
    }
}
