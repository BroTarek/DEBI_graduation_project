using System;
using System.Collections.Generic;
using YouTubeClone.Domain.Base;
using YouTubeClone.Domain.Entities.Channels;
using YouTubeClone.Domain.Entities.WatchHistories;
using YouTubeClone.Domain.Aggregates.Videos;

namespace YouTubeClone.Domain.Entities.Videos
{
    public class Video : Entity<Guid>
    {
        public Guid channelId { get; set; }
        public virtual Channel? Channel { get; set; }

        public VideoBasics video_Basics { get; set; } = null!;
        public VideoDescriptive video_Descriptive { get; set; } = null!;
        public VideoTechnicalDetails video_Technical_details { get; set; } = null!;
        public TemporalMetadata Temporal_Metadata { get; set; } = null!;
        public VideoStats VideoStats { get; set; } = null!;

        public virtual ICollection<Comment> comments { get; set; } = new List<Comment>();
        public virtual ICollection<WatchHistory> WatchHistories { get; set; } = new List<WatchHistory>();
        public virtual ICollection<Playlists.Playlist> Playlists { get; set; } = new List<Playlists.Playlist>();

        public Video() { }
        public Video(
            Guid id,
            Guid channelId,
            VideoBasics video_Basics,
            VideoDescriptive video_Descriptive,
            VideoTechnicalDetails video_Technical_details,
            TemporalMetadata temporal_Metadata,
            VideoStats videoStats) : base(id)
        {
            this.channelId = channelId;
            this.video_Basics = video_Basics;
            this.video_Descriptive = video_Descriptive;
            this.video_Technical_details = video_Technical_details;
            this.Temporal_Metadata = temporal_Metadata;
            this.VideoStats = videoStats;
        }
    }
}
