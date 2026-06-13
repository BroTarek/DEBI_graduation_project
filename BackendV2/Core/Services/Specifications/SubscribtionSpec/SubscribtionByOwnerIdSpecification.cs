using SoftBridge.Services.Specification;
using YouTubeClone.Domain.Aggregates.Subscriptions;

namespace YouTubeClone.Core.Services.Specifications
{
    public class SubscribtionByOwnerIdSpecification : BaseSpecification<Subscriptions, YouTubeClone.Domain.ValueObjects.SubscriptionId>
    {
        public SubscribtionByOwnerIdSpecification(string ownerId) 
            : base(s => s.OwnerId == ownerId)
        {
            AddInclude(s => s.Channels);
            // Use string includes for nested properties
            AddInclude("Channels.Videos");
            AddInclude("Channels.Posts");
        }
    }
}
