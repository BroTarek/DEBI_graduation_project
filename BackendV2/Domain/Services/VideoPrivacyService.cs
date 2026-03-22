using YouTubeClone.Domain.ValueObjects;
using YouTubeClone.Domain.Aggregates.Videos;
using YouTubeClone.Domain.Aggregates.Playlists;
using System.Collections.Generic;

namespace YouTubeClone.Domain.Services
{
    public interface IVideoPrivacyService
    {
        void HandleVideoPrivacyChange(VideoId videoId, PrivacyStatus newStatus, List<Playlist> playlistsAffected);
    }

    public class VideoPrivacyService : IVideoPrivacyService
    {
        public void HandleVideoPrivacyChange(VideoId videoId, PrivacyStatus newStatus, List<Playlist> playlistsAffected)
        {
            if (newStatus == PrivacyStatus.Private)
            {
                // Logic to remove video from public playlists if necessary
                // This would typically involve coordination across playlist aggregates
            }
        }
    }
}
