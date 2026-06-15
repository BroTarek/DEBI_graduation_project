using System;
using YouTubeClone.Domain.Models;
using SoftBridge.Services.Specification;
using YouTubeClone.Shared.Common.Params;

namespace YouTubeClone.Services.Specifications
{
    public class PlaylistWithVideosSpecification : BaseSpecification<Playlist, Guid>
    {
        public PlaylistWithVideosSpecification(Guid playlistId, QueryParams queryParams)
            : base(p => p.Id == playlistId)
        {
            // Pull down videos array along with their nested technical components
            AddInclude("videos.video_Basics");
            AddInclude("videos.video_Descriptive");
            AddInclude("videos.Temporal_Metadata");

            // Dynamic sort parsing explicitly on UploadDate
            if (queryParams.Sort == SortingOptionsEnum.DateCreatedAsc)
            {
                AddOrderBy(p => p.videos.Select(v => v.Temporal_Metadata.UploadDate));
            }
            else
            {
                // Default: Newest Uploads First
                AddOrderByDescending(p => p.videos.Select(v => v.Temporal_Metadata.UploadDate));
            }
        }
    }
}


using System;
using YouTubeClone.Domain.Models;
using SoftBridge.Services.Specification;

namespace YouTubeClone.Services.Specifications
{
    public class OwnerPlaylistsSpecification : BaseSpecification<Playlist, Guid>
    {
        public OwnerPlaylistsSpecification(string ownerId, string label)
            : base(p => (label.ToLower() == "channel" && p is ChannelPlaylist && ((ChannelPlaylist)p).channelId == ownerId) ||
                       (label.ToLower() == "custom" && p is CustomPlaylist && ((CustomPlaylist)p).ownerId == ownerId))
        {
            AddInclude("videos.video_Basics");
        }
    }
}

using System;
using System.Collections.Generic;
using YouTubeClone.Shared.Common;

namespace YouTubeClone.Shared.Dto_s
{
    public class PlaylistVideosResultDTO
    {
        // Header Info
        public string PlaylistName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string PlaylistThumbnailUrl { get; set; } = string.Empty;

        // Paginated Collection wrapper matching your Shared contract pattern
        public Pagination<PlaylistVideoItemDTO> Videos { get; set; } = null!;
    }

    public class PlaylistVideoItemDTO
    {
        public string VideoId { get; set; } = string.Empty;
        public string VideoName { get; set; } = string.Empty;
        public string VideoUrl { get; set; } = string.Empty;
        public string ThumbnailUrl { get; set; } = string.Empty;
        public DateTime UploadDate { get; set; }
    }

    public class CompactPlaylistLookupDTO
    {
        public string PlaylistId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string ThumbnailUrl { get; set; } = string.Empty;
        public int VideosCount { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YouTubeClone.Domain.Contracts.UOW;
using YouTubeClone.Domain.Models;
using YouTubeClone.Services.Specifications;
using YouTubeClone.Shared.Common;
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

    public class PlaylistService : IPlaylistService
    {
        private readonly IUnitOfWork _unitOfWork;

        public PlaylistService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> CreatePlaylistAsync(Guid targetId, string label, string name, string description)
        {
            var playlistRepo = _unitOfWork.GetRepo<Playlist, Guid>();

            if (label.ToLower() == "channel")
            {
                var channelPlaylist = new ChannelPlaylist
                {
                    channelId = targetId.ToString(),
                    description = description,
                    accessibility = Accessibility.PUBLIC,
                    videos = new List<Video>()
                };
                // Note: Assuming your domain maps property name from base polymorphic class
                // mapping to your ChannelPlaylist fields
                await playlistRepo.AddAsync(channelPlaylist);
            }
            else if (label.ToLower() == "custom")
            {
                var customPlaylist = new CustomPlaylist
                {
                    name = name,
                    ownerId = targetId.ToString(),
                    accessibility = Accessibility.PRIVATE,
                    videos = new List<Video>()
                };
                await playlistRepo.AddAsync(customPlaylist);
            }
            else
            {
                return false; // Invalid classification tag
            }

            return await _unitOfWork.SaveChangesAsync() > 0;
        }

        public async Task<bool> AddVideoToPlaylistAsync(Guid playlistId, string videoId)
        {
            var playlistRepo = _unitOfWork.GetRepo<Playlist, Guid>();
            var videoRepo = _unitOfWork.GetRepo<Video, Guid>();

            var playlist = await playlistRepo.GetByIdAsync(playlistId);
            var video = (await videoRepo.GetAllAsync()).FirstOrDefault(v => v.video_Basics.VideoId == videoId);

            if (playlist == null || video == null) return false;

            playlist.videos ??= new List<Video>();
            if (!playlist.videos.Any(v => v.video_Basics.VideoId == videoId))
            {
                playlist.videos.Add(video);
                await playlistRepo.UpdateAsync(playlist);
                return await _unitOfWork.SaveChangesAsync() > 0;
            }

            return true; // Already exists safely
        }

        public async Task<bool> RemoveVideoFromPlaylistAsync(Guid playlistId, string videoId)
        {
            var playlistRepo = _unitOfWork.GetRepo<Playlist, Guid>();
            var spec = new PlaylistWithVideosSpecification(playlistId, new QueryParams());
            var playlist = await playlistRepo.GetByIdWithSpecificationsAsync(spec);

            if (playlist == null || playlist.videos == null) return false;

            var targetVideo = playlist.videos.FirstOrDefault(v => v.video_Basics.VideoId == videoId);
            if (targetVideo == null) return false;

            playlist.videos.Remove(targetVideo);
            await playlistRepo.UpdateAsync(playlist);
            return await _unitOfWork.SaveChangesAsync() > 0;
        }

        public async Task<bool> ClearPlaylistAsync(Guid playlistId)
        {
            var playlistRepo = _unitOfWork.GetRepo<Playlist, Guid>();
            var spec = new PlaylistWithVideosSpecification(playlistId, new QueryParams());
            var playlist = await playlistRepo.GetByIdWithSpecificationsAsync(spec);

            if (playlist == null) return false;

            playlist.videos?.Clear();
            await playlistRepo.UpdateAsync(playlist);
            return await _unitOfWork.SaveChangesAsync() > 0;
        }

        public async Task<PlaylistVideosResultDTO?> GetVideosInPlaylistAsync(Guid playlistId, QueryParams queryParams)
        {
            var playlistRepo = _unitOfWork.GetRepo<Playlist, Guid>();
            var spec = new PlaylistWithVideosSpecification(playlistId, queryParams);
            var playlist = await playlistRepo.GetByIdWithSpecificationsAsync(spec);

            if (playlist == null) return null;

            // Determine display names based on polymorphic type mapping attributes
            string playlistName = playlist is CustomPlaylist cp ? cp.name : "Channel Playlist Collection";
            string playlistDesc = playlist is ChannelPlaylist chp ? chp.description : "User Custom Curated Video Vault";
            
            // Thumbnail is fallback-mapped to the first video in the selection array
            string defaultThumb = playlist.videos?.FirstOrDefault()?.video_Basics?.ThumbnailUrl ?? "https://api.dicebear.com/7.x/identicon/svg?seed=playlist";

            // Map standard pagination
            var totalCount = playlist.videos?.Count ?? 0;
            var processedItems = (playlist.videos ?? new List<Video>())
                .Skip((queryParams.PageIndex - 1) * queryParams.PageSize)
                .Take(queryParams.PageSize)
                .Select(v => new PlaylistVideoItemDTO
                {
                    VideoId = v.video_Basics.VideoId,
                    VideoName = v.video_Descriptive.Title,
                    VideoUrl = v.video_Basics.videoUrl,
                    ThumbnailUrl = v.video_Basics.ThumbnailUrl,
                    UploadDate = v.Temporal_Metadata.UploadDate
                });

            return new PlaylistVideosResultDTO
            {
                PlaylistName = playlistName,
                Description = playlistDesc,
                PlaylistThumbnailUrl = defaultThumb,
                Videos = new Pagination<PlaylistVideoItemDTO>(queryParams.PageIndex, queryParams.PageSize, totalCount, processedItems)
            };
        }

        public async Task<IEnumerable<CompactPlaylistLookupDTO>> GetAllPlaylistsAsync(string ownerId, string label)
        {
            var playlistRepo = _unitOfWork.GetRepo<Playlist, Guid>();
            var spec = new OwnerPlaylistsSpecification(ownerId, label);
            var playlists = await playlistRepo.GetAllWithSpecificationAsync(spec);

            return playlists.Select(p => new CompactPlaylistLookupDTO
            {
                PlaylistId = p.Id.ToString(),
                Name = p is CustomPlaylist cp ? cp.name : "Channel Content List",
                VideosCount = p.videos?.Count ?? 0,
                ThumbnailUrl = p.videos?.FirstOrDefault()?.video_Basics?.ThumbnailUrl ?? "https://api.dicebear.com/7.x/identicon/svg?seed=playlist"
            });
        }
    }
}
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
            if (!success) return BadRequest(new ApiResponse<object>("Failed to instantiate custom structured payload playlist.", 400));
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
            return Ok(new ApiResponse<object>(new { ClearedPlaylistId = playlistId }, "All video association logs purged from playlist repository contents."));
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


