using System;

namespace YouTubeClone.Domain.ValueObjects
{
    public record PlaylistId(Guid Value);
    public record SubscriptionId(Guid Value);
    public record UserId(Guid Value);
    public record CommentId(Guid Value);
    public record VideoId(Guid Value);
    public record WatchHistoryId(Guid Value);
    public record ChannelId(Guid Value);
}
