using System;
using System.Collections.Generic;

namespace YouTubeClone.Shared.DTOs.Playlist
{
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