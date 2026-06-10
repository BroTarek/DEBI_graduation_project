using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YouTubeClone.Core.Services;
using YouTubeClone.Domain.Contracts.UOW;
using YouTubeClone.Domain.Aggregates.Playlists;
using YouTubeClone.Domain.Aggregates.Videos;
using YouTubeClone.Domain.Aggregates.Channels;
using YouTubeClone.Domain.ValueObjects;
using YouTubeClone.Core.Services.Specifications.PlaylistSpec;
using YouTubeClone.Shared.DTOs.Playlist; // Assuming this namespace based on folder structure

namespace YouTubeClone.Infrastructure.Persistence.Services.PlaylistService
{
    public class PlaylistService : IPlaylistService
    {
        private readonly IUnitOfWork _unitOfWork;

        public PlaylistService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Guid> CreateChannelPlaylist(Guid channelId, CreatePlaylistDto dto, Guid currentUserId)
        {
            // Verify channel ownership
            var channelRepo = _unitOfWork.GetRepo<Channel, ChannelId>();
            var channel = await channelRepo.GetByIdAsync(new ChannelId(channelId));
            if (channel == null || channel.OwnerId.Value != currentUserId)
            {
                throw new UnauthorizedAccessException("You are not authorized to create a playlist for this channel.");
            }

            var playlistId = new PlaylistId(Guid.NewGuid());
            var accessibility = dto.IsPublic ? Accessibility.Public : Accessibility.Private;
            
            var playlist = new ChannelPlaylist(playlistId, channelId.ToString(), dto.Name, dto.Description, dto.ThumbnailUrl ?? "", accessibility);
            
            var playlistRepo = _unitOfWork.GetRepo<Playlist, PlaylistId>();
            await playlistRepo.AddAsync(playlist);
            await _unitOfWork.SaveChangesAsync();

            return playlistId.Value;
        }

        public async Task<Guid> CreateCustomPlaylist(Guid userId, CreatePlaylistDto dto, Guid currentUserId)
        {
            if (userId != currentUserId)
            {
                throw new UnauthorizedAccessException("You can only create custom playlists for yourself.");
            }

            var playlistId = new PlaylistId(Guid.NewGuid());
            var accessibility = dto.IsPublic ? Accessibility.Public : Accessibility.Private;

            var playlist = new CustomPlaylist(playlistId, userId.ToString(), dto.Name, dto.Description, dto.ThumbnailUrl ?? "", accessibility);

            var playlistRepo = _unitOfWork.GetRepo<Playlist, PlaylistId>();
            await playlistRepo.AddAsync(playlist);
            await _unitOfWork.SaveChangesAsync();

            return playlistId.Value;
        }

        public async Task DeletePlaylist(Guid playlistId, Guid currentUserId)
        {
            var playlistRepo = _unitOfWork.GetRepo<Playlist, PlaylistId>();
            var playlist = await playlistRepo.GetByIdAsync(new PlaylistId(playlistId));
            
            if (playlist == null) throw new KeyNotFoundException("Playlist not found");

            await VerifyPlaylistOwnership(playlist, currentUserId);

            await playlistRepo.DeleteAsync(playlist);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task AddVideoToPlaylist(Guid videoId, Guid playlistId, Guid currentUserId)
        {
            var playlistRepo = _unitOfWork.GetRepo<Playlist, PlaylistId>();
            var spec = new PlaylistWithVideosSpecification(new PlaylistId(playlistId));
            var playlist = await playlistRepo.GetByIdWithSpecificationsAsync(spec);

            if (playlist == null) throw new KeyNotFoundException("Playlist not found");

            await VerifyPlaylistOwnership(playlist, currentUserId);

            var videoRepo = _unitOfWork.GetRepo<Video, VideoId>();
            var video = await videoRepo.GetByIdAsync(new VideoId(videoId));
            if (video == null) throw new KeyNotFoundException("Video not found");

            if (!playlist.Videos.Any(v => v.Id.Value == videoId))
            {
                playlist.AddVideo(video);
                await playlistRepo.UpdateAsync(playlist);
                await _unitOfWork.SaveChangesAsync();
            }
        }

        public async Task RemoveVideoFromPlaylist(Guid videoId, Guid playlistId, Guid currentUserId)
        {
            var playlistRepo = _unitOfWork.GetRepo<Playlist, PlaylistId>();
            var spec = new PlaylistWithVideosSpecification(new PlaylistId(playlistId));
            var playlist = await playlistRepo.GetByIdWithSpecificationsAsync(spec);

            if (playlist == null) throw new KeyNotFoundException("Playlist not found");

            await VerifyPlaylistOwnership(playlist, currentUserId);

            var videoToRemove = playlist.Videos.FirstOrDefault(v => v.Id.Value == videoId);
            if (videoToRemove != null)
            {
                playlist.RemoveVideo(videoToRemove);
                await playlistRepo.UpdateAsync(playlist);
                await _unitOfWork.SaveChangesAsync();
            }
        }

        public async Task UpdatePlaylist(Guid playlistId, CreatePlaylistDto dto, Guid currentUserId)
        {
            var playlistRepo = _unitOfWork.GetRepo<Playlist, PlaylistId>();
            var playlist = await playlistRepo.GetByIdAsync(new PlaylistId(playlistId));

            if (playlist == null) throw new KeyNotFoundException("Playlist not found");

            await VerifyPlaylistOwnership(playlist, currentUserId);

            var accessibility = dto.IsPublic ? Accessibility.Public : Accessibility.Private;
            playlist.UpdateDetails(dto.Name, dto.Description, accessibility);

            await playlistRepo.UpdateAsync(playlist);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<List<PlaylistDto>> GetAllPlaylistsOfChannel(Guid channelId)
        {
            var playlistRepo = _unitOfWork.GetRepo<ChannelPlaylist, PlaylistId>();
            var spec = new PlaylistByChannelIdSpecification(channelId.ToString());
            var playlists = await playlistRepo.GetAllWithSpecificationAsync(spec);

            return playlists.Select(p => MapToDto(p, Guid.Parse(p.ChannelId))).ToList();
        }

        public async Task<List<PlaylistDto>> GetAllPlaylistsCreatedByUser(Guid userId)
        {
            var playlistRepo = _unitOfWork.GetRepo<CustomPlaylist, PlaylistId>();
            var spec = new PlaylistByUserIdSpecification(userId.ToString());
            var playlists = await playlistRepo.GetAllWithSpecificationAsync(spec);

            return playlists.Select(p => MapToDto(p, Guid.Empty)).ToList(); // Custom playlists don't have ChannelId
        }

        private async Task VerifyPlaylistOwnership(Playlist playlist, Guid currentUserId)
        {
            if (playlist is ChannelPlaylist cp)
            {
                var channelRepo = _unitOfWork.GetRepo<Channel, ChannelId>();
                var channel = await channelRepo.GetByIdAsync(new ChannelId(Guid.Parse(cp.ChannelId)));
                if (channel == null || channel.OwnerId.Value != currentUserId)
                    throw new UnauthorizedAccessException("You don't own this channel playlist.");
            }
            else if (playlist is CustomPlaylist up)
            {
                if (up.OwnerId != currentUserId.ToString())
                    throw new UnauthorizedAccessException("You don't own this custom playlist.");
            }
            else
            {
                throw new UnauthorizedAccessException("Unknown playlist type ownership.");
            }
        }

        private PlaylistDto MapToDto(Playlist playlist, Guid channelId)
        {
            return new PlaylistDto
            {
                Id = playlist.Id.Value,
                ChannelId = channelId,
                Name = playlist.Name,
                Description = playlist.Description,
                ThumbnailUrl = playlist.ThumbnailUrl,
                IsPublic = playlist.Accessibility == Accessibility.Public,
                VideoIds = playlist.Videos.Select(v => v.Id.Value).ToList()
            };
        }
    }
}
