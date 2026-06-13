using SoftBridge.Services.Specification;
using System;
using YouTubeClone.Domain.Aggregates.Videos;

using YouTubeClone.Domain.Aggregates;

namespace YouTubeClone.Services.Specifications
{
    public class HomePageVideosSpecification : BaseSpecification<Video, VideoId>
    {
        public HomePageVideosSpecification(int skip, int take)
        {
            // 1. Filter out private videos if required by defaults
            Criteria = v => v.Basics.PrivacyStatus == Accessibility.PUBLIC;

            // 2. Eagerly load the Channel info to read the Name 
            // Assuming your Video entity maps the Channel navigation property
            AddInclude("Channel.ChannelProfile");

            // 3. Sort by most recent uploads first
            AddOrderByDescending(v => v.TemporalMetadata.UploadDate);

            // 4. Apply Pagination parameters
            ApplyPaging(skip, take);
        }
    }
}