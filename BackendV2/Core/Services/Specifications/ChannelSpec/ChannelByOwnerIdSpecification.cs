using System;
using SoftBridge.Services.Specification;
using YouTubeClone.Domain.Aggregates.Channels;
using YouTubeClone.Domain.ValueObjects;

namespace YouTubeClone.Core.Services.Specifications.ChannelSpec
{
    public class ChannelByOwnerIdSpecification : BaseSpecification<Channel, ChannelId>
    {
        public ChannelByOwnerIdSpecification(Guid ownerId) 
            : base(c => c.Owner.Id == new UserId(ownerId))
        {
        }
    }
}
