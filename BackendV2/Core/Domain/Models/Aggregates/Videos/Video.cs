using System;
using System.Collections.Generic;
using YouTubeClone.Domain.Base;
using YouTubeClone.Domain.ValueObjects;

namespace YouTubeClone.Domain.Aggregates.Videos
{
    public class Video : AggregateRoot<VideoId>
    {
        public string ChannelId { get; private set; }
        public video_Basics Basics { get; private set; }
        public video_Descriptive Descriptive { get; private set; }
        public video_Technical_details TechnicalDetails { get; private set; }
        public Temporal_Metadata TemporalMetadata { get; private set; }
        public VideoStats Stats { get; private set; }

        private readonly List<Comment> _comments = new();
        public IReadOnlyList<Comment> Comments => _comments.AsReadOnly();

        public Video(
            VideoId id,
            string channelId,
            video_Basics basics,
            video_Descriptive descriptive,
            video_Technical_details technicalDetails,
            Temporal_Metadata temporalMetadata,
            VideoStats stats) : base(id)
        {
            ChannelId = channelId;
            Basics = basics;
            Descriptive = descriptive;
            TechnicalDetails = technicalDetails;
            TemporalMetadata = temporalMetadata;
            Stats = stats;
        }

        public void AddComment(Comment comment)
        {
            _comments.Add(comment);
        }
    }
}
