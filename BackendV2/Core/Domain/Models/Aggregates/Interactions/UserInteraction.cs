using System;
using YouTubeClone.Domain.Base;
using YouTubeClone.Domain.ValueObjects;

namespace YouTubeClone.Domain.Aggregates.Interactions
{
    public class UserInteraction : Entity<UserInteractionId>
    {
        public UserInteractionId Id { get; set; }
    }

    public record UserInteractionId(Guid Value);
}
