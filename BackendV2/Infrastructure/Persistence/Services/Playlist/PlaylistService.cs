using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YouTubeClone.Domain.Contracts.UOW;
using YouTubeClone.Domain.Entities.Playlists;
using YouTubeClone.Domain.Entities.Videos;
using YouTubeClone.Domain.Enums;
using YouTubeClone.Services.Specifications;
using YouTubeClone.Shared.Common;
using YouTubeClone.Shared.Common.Params;
using YouTubeClone.Shared.Dto_s;

namespace YouTubeClone.Services
{
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
                    Id = Guid.NewGuid(),
                    channelId = targetId,
                    description = description,
                    accessibility = Accessibility.PUBLIC,
                    videos = new List<Video>()
                };
                await playlistRepo.AddAsync(channelPlaylist);
            }
            else if (label.ToLower() == "custom")
            {
                var customPlaylist = new CustomPlaylist
                {
                    Id = Guid.NewGuid(),
                    name = name,
                    ownerId = targetId.ToString(),
                    accessibility = Accessibility.PRIVATE,
                    videos = new List<Video>()
                };
                await playlistRepo.AddAsync(customPlaylist);
            }
            else
            {
                return false;
            }

            return await _unitOfWork.SaveChangesAsync() > 0;
        }

        public async Task<bool> AddVideoToPlaylistAsync(Guid playlistId, string videoId)
        {
            var playlistRepo = _unitOfWork.GetRepo<Playlist, Guid>();
            var videoRepo = _unitOfWork.GetRepo<Video, Guid>();

            var spec = new PlaylistWithVideosSpecification(playlistId, new QueryParams());
            var playlist = await playlistRepo.GetByIdWithSpecificationsAsync(spec);
            var video = (await videoRepo.GetAllAsync()).FirstOrDefault(v => v.video_Basics.VideoId == Guid.Parse(videoId));

            if (playlist == null || video == null) return false;

            playlist.videos ??= new List<Video>();
            if (!playlist.videos.Any(v => v.video_Basics.VideoId == Guid.Parse(videoId)))
            {
                playlist.videos.Add(video);
                await playlistRepo.UpdateAsync(playlist);
                return await _unitOfWork.SaveChangesAsync() > 0;
            }

            return true;
        }

        public async Task<bool> RemoveVideoFromPlaylistAsync(Guid playlistId, string videoId)
        {
            var playlistRepo = _unitOfWork.GetRepo<Playlist, Guid>();
            var spec = new PlaylistWithVideosSpecification(playlistId, new QueryParams());
            var playlist = await playlistRepo.GetByIdWithSpecificationsAsync(spec);

            if (playlist == null || playlist.videos == null) return false;

            var targetVideo = playlist.videos.FirstOrDefault(v => v.video_Basics.VideoId == Guid.Parse(videoId));
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

            string playlistName = playlist is CustomPlaylist cp ? cp.name : "Channel Playlist Collection";
            string playlistDesc = playlist is ChannelPlaylist chp ? chp.description : "User Custom Curated Video Vault";
            
            string defaultThumb = playlist.videos?.FirstOrDefault()?.video_Basics?.ThumbnailUrl ?? "https://api.dicebear.com/7.x/identicon/svg?seed=playlist";

            var totalCount = playlist.videos?.Count ?? 0;
            var processedItems = (playlist.videos ?? new List<Video>())
                .Skip((queryParams.PageIndex - 1) * queryParams.PageSize)
                .Take(queryParams.PageSize)
                .Select(v => new PlaylistVideoItemDTO
                {
                    VideoId = v.video_Basics.VideoId.ToString(),
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
