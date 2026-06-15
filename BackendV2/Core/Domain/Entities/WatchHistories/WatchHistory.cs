using System;
using System.Collections.Generic;
using YouTubeClone.Domain.Base;
using YouTubeClone.Domain.Models.Identity;
using YouTubeClone.Domain.Entities.Videos;

namespace YouTubeClone.Domain.Entities.WatchHistories
{
    public class WatchHistory : Entity<Guid>
    {
        public string OwnerId { get; set; } = string.Empty;
        public virtual ApplicationUser owner { get; set; } = null!;
        public virtual ICollection<Video> videos { get; set; } = new List<Video>();

        public WatchHistory() { }
        public WatchHistory(Guid id) : base(id) { }
    }
}
