using System;
using System.Linq;
using YouTubeClone.Domain.Entities.Videos;
using YouTubeClone.Domain.Entities.WatchHistories;
using YouTubeClone.Domain.Enums;
using SoftBridge.Services.Specification;
using YouTubeClone.Shared.Common.Params;
using YouTubeClone.Shared.Common;

namespace YouTubeClone.Services.Specifications
{
    public class VideoWithDetailsSpecification : BaseSpecification<Video, Guid>
    {
        public VideoWithDetailsSpecification(QueryParams queryParams)
            : base(v => v.video_Basics.PrivacyStatus == Accessibility.PUBLIC &&
                       (string.IsNullOrEmpty(queryParams.Search) || 
                        v.video_Descriptive.Title.ToLower().Contains(queryParams.Search)))
        {
            AddInclude(v => v.video_Basics);
            AddInclude(v => v.video_Descriptive);
            AddInclude(v => v.video_Technical_details);
            AddInclude(v => v.VideoStats);
            AddInclude("Channel.ChannelProfile");

            if (queryParams.Sort.HasValue)
            {
                switch (queryParams.Sort.Value)
                {
                    case SortingOptionsEnum.NameAsc:
                        AddOrderBy(v => v.video_Descriptive.Title);
                        break;
                    case SortingOptionsEnum.NameDesc:
                        AddOrderByDescending(v => v.video_Descriptive.Title);
                        break;
                    case SortingOptionsEnum.DateCreatedAsc:
                        AddOrderBy(v => v.Temporal_Metadata.UploadDate);
                        break;
                    case SortingOptionsEnum.DateCreatedDesc:
                    default:
                        AddOrderByDescending(v => v.Temporal_Metadata.UploadDate);
                        break;
                }
            }
            else
            {
                AddOrderByDescending(v => v.Temporal_Metadata.UploadDate);
            }

            ApplyPaging((queryParams.PageIndex - 1) * queryParams.PageSize, queryParams.PageSize);
        }

        public VideoWithDetailsSpecification(Guid videoId) 
            : base(v => v.video_Basics.VideoId == videoId)
        {
            AddInclude(v => v.video_Basics);
            AddInclude(v => v.video_Descriptive);
            AddInclude(v => v.video_Technical_details);
            AddInclude(v => v.VideoStats);
            AddInclude("Channel.ChannelProfile");
            AddInclude("comments");
        }
    }

    public class WatchHistorySpecification : BaseSpecification<WatchHistory, Guid>
    {
        public WatchHistorySpecification(Guid userId) 
            : base(wh => wh.owner.Id == userId.ToString())
        {
            AddInclude("videos.video_Basics");
            AddInclude("videos.video_Descriptive");
            AddInclude("videos.VideoStats");
            AddInclude("videos.Channel");
        }
    }

    public class WatchHistoryVideosSpecification : BaseSpecification<Video, Guid>
    {
        public WatchHistoryVideosSpecification(QueryParams query, Guid userId)
            : base(v => v.WatchHistories.Any(wh => wh.owner.Id == userId.ToString()) &&
                       (string.IsNullOrEmpty(query.Search) || 
                        v.video_Descriptive.Title.ToLower().Contains(query.Search)))
        {
            AddInclude(v => v.video_Basics);
            AddInclude(v => v.video_Descriptive);
            AddInclude(v => v.video_Technical_details);

            AddOrderBy(v => v.video_Descriptive.Title);

            ApplyPaging((query.PageIndex - 1) * query.PageSize, query.PageSize);
        }
    }
}
