using Makanak.Domain.Contracts.UOW;
using Makanak.Presentation.Controllers;
using Makanak.Shared.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;
using YouTubeClone.Domain.Aggregates.Channels;
using YouTubeClone.Domain.Aggregates.Subscriptions;
using YouTubeClone.Domain.ValueObjects;

namespace YouTubeClone.Presentation.Controllers
{
    [Authorize]
    public class ChannelController : BaseController
    {
        private readonly IUnitOfWork _unitOfWork;

        public ChannelController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpPost]
        public async Task<IActionResult> CreateChannel([FromBody] CreateChannelDto dto)
        {
            var userIdStr = GetUserId();
            if (string.IsNullOrEmpty(userIdStr) || !Guid.TryParse(userIdStr, out var userIdGuid))
            {
                return Unauthorized(new ApiResponse<string>("Unauthorized user.", 401));
            }

            var ownerId = new UserId(userIdGuid);
            var channelId = new ChannelId(Guid.NewGuid());
            var name = new ChannelName(dto.Name);
            var description = new ChannelDescription(dto.Description);

            var channel = new Channel(channelId, ownerId, name, description);
            
            var channelRepo = _unitOfWork.GetRepo<Channel, ChannelId>();
            await channelRepo.AddAsync(channel);
            await _unitOfWork.SaveChangesAsync();

            return Ok(new ApiResponse<Guid>(channel.Id.Value, "Channel created successfully."));
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetChannel(Guid id)
        {
            var channelId = new ChannelId(id);
            var channelRepo = _unitOfWork.GetRepo<Channel, ChannelId>();
            var channel = await channelRepo.GetByIdAsync(channelId);
            if (channel == null)
            {
                return NotFound(new ApiResponse<string>("Channel not found.", 404));
            }

            return Ok(new ApiResponse<ChannelDetailsDto>(new ChannelDetailsDto
            {
                Id = channel.Id.Value,
                OwnerId = channel.OwnerId.Value,
                Name = channel.Name.Value,
                Description = channel.Description.Value,
                CreatedAt = channel.CreatedAt
            }, "Channel retrieved successfully."));
        }

        [HttpPost("{id}/subscribe")]
        public async Task<IActionResult> ToggleSubscribe(Guid id)
        {
            var userIdStr = GetUserId();
            if (string.IsNullOrEmpty(userIdStr) || !Guid.TryParse(userIdStr, out var userIdGuid))
            {
                return Unauthorized(new ApiResponse<string>("Unauthorized user.", 401));
            }

            var subscriberId = new UserId(userIdGuid);
            var channelId = new ChannelId(id);

            var channelRepo = _unitOfWork.GetRepo<Channel, ChannelId>();
            var channel = await channelRepo.GetByIdAsync(channelId);
            if (channel == null)
            {
                return NotFound(new ApiResponse<string>("Channel not found.", 404));
            }

            if (channel.OwnerId.Value == subscriberId.Value)
            {
                return BadRequest(new ApiResponse<string>("You cannot subscribe to your own channel.", 400));
            }

            var subRepo = _unitOfWork.GetRepo<Subscription, SubscriptionId>();
            var allSubs = await subRepo.GetAllAsync();
            var existingSub = allSubs.FirstOrDefault(s => s.SubscriberId.Value == subscriberId.Value && s.ChannelId.Value == channelId.Value);

            if (existingSub != null)
            {
                await subRepo.DeleteAsync(existingSub);
                await _unitOfWork.SaveChangesAsync();
                return Ok(new ApiResponse<string>("Unsubscribed successfully."));
            }
            else
            {
                var sub = new Subscription(new SubscriptionId(Guid.NewGuid()), subscriberId, channelId);
                await subRepo.AddAsync(sub);
                await _unitOfWork.SaveChangesAsync();
                return Ok(new ApiResponse<string>("Subscribed successfully."));
            }
        }
    }

    public class CreateChannelDto
    {
        public string Name { get; set; } = null!;
        public string Description { get; set; } = string.Empty;
    }

    public class ChannelDetailsDto
    {
        public Guid Id { get; set; }
        public Guid OwnerId { get; set; }
        public string Name { get; set; } = null!;
        public string Description { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}
