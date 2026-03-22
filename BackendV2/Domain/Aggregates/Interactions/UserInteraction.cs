using System;
using YouTubeClone.Domain.Base;
using YouTubeClone.Domain.ValueObjects;

namespace YouTubeClone.Domain.Aggregates.Interactions
{
    public class UserInteraction : AggregateRoot<UserInteractionId>
    {
        public UserId UserId { get; private set; }
        public InteractionTarget Target { get; private set; }
        public InteractionType Type { get; private set; }
        public DateTime CreatedAt { get; private set; }

        public UserInteraction(UserInteractionId id, UserId userId, InteractionTarget target, InteractionType type) : base(id)
        {
            UserId = userId;
            Target = target;
            Type = type;
            CreatedAt = DateTime.UtcNow;
        }
    }
}
