using System;
using YouTubeClone.Domain.Aggregates.Videos;

namespace YouTubeClone.Core.Services.Specifications.CommentsSpec
{
    public class VideoWithCommentsSpecification : SoftBridge.Services.Specification.BaseSpecification<Video, VideoId>{
        public VideoWithCommentsSpecification(VideoId videoId) : base(v => v.Id.Value == videoId.Value)
        {
            AddInclude("Comments");
            AddInclude("Comments.Replies");
        }
    }
}