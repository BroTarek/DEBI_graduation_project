using Makanak.Domain.Contracts.UOW;
using Makanak.Presentation.Controllers;
using Makanak.Shared.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;
using YouTubeClone.Domain.Aggregates.Subscribtion;
using YouTubeClone.Domain.Aggregates.Subscriptions;
using YouTubeClone.Domain.ValueObjects;

namespace YouTubeClone.Presentation.Controllers
{
    [Authorize]
    public class Subscribtionontroller : BaseController
    {
        private readonly IUnitOfWork _unitOfWork;

        public Subscribtionontroller(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }


        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetSubscribtion (Guid id)
        {
            var Subscribtiond = new Subscribtiond(id);
            var Subscribtionepo = _unitOfWork.GetRepo<Subscribtion Subscribtiond>();
            var Subscribtion= await Subscribtionepo.GetByIdAsync(Subscribtiond);
            if (Subscribtion== null)
            {
                return NotFound(new ApiResponse<string>("Subscribtionnot found.", 404));
            }

            return Ok(new ApiResponse<SubscribtionetailsDto>(new SubscribtionetailsDto
            {
                Id = SubscribtionId.Value,
                OwnerId = SubscribtionOwnerId.Value,
                Name = SubscribtionName.Value,
                Description = SubscribtionDescription.Value,
                CreatedAt = SubscribtionCreatedAt
            }, "Subscribtionretrieved successfully."));
        }  
        public async Task<IActionResult> GetSubscribedChannelsVideos (Guid id)
        { 
            //////////////////////////////////////////////////////////////////////
            /// 
            /// 
            /// SubscribedChannelsVideosSpecification
            /// 
            /// 
            /// 
            /// 
            /// 
            /// 
            /// 
            /// 
            /// 
            /// 
            /// 
            /// 
            /// 
            /// ///////////////////////////////////////////////////////////////////
            var Subscribtiond = new Subscribtiond(id);
            var Subscribtionepo = _unitOfWork.GetRepo<Subscribtion Subscribtiond>();
            var Subscribtion= await Subscribtionepo.GetByIdAsync(Subscribtiond);
            if (Subscribtion== null)
            {
                return NotFound(new ApiResponse<string>("Subscribtionnot found.", 404));
            }

            return Ok(new ApiResponse<SubscribtionetailsDto>(new SubscribtionetailsDto
            {
                Id = SubscribtionId.Value,
                OwnerId = SubscribtionOwnerId.Value,
                Name = SubscribtionName.Value,
                Description = SubscribtionDescription.Value,
                CreatedAt = SubscribtionCreatedAt
            }, "Subscribtionretrieved successfully."));
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
            var Subscribtiond = new Subscribtiond(id);

            var Subscribtionepo = _unitOfWork.GetRepo<Subscribtion Subscribtiond>();
            var Subscribtion= await Subscribtionepo.GetByIdAsync(Subscribtiond);
            if (Subscribtion== null)
            {
                return NotFound(new ApiResponse<string>("Subscribtionnot found.", 404));
            }

            if (SubscribtionOwnerId.Value == subscriberId.Value)
            {
                return BadRequest(new ApiResponse<string>("You cannot subscribe to your own Subscribtion", 400));
            }

            var subRepo = _unitOfWork.GetRepo<Subscription, SubscriptionId>();
            var allSubs = await subRepo.GetAllAsync();
            var existingSub = allSubs.FirstOrDefault(s => s.SubscriberId.Value == subscriberId.Value && s.Subscribtiond.Value == Subscribtiond.Value);

            if (existingSub != null)
            {
                await subRepo.DeleteAsync(existingSub);
                await _unitOfWork.SaveChangesAsync();
                return Ok(new ApiResponse<string>("Unsubscribed successfully."));
            }
            else
            {
                var sub = new Subscription(new SubscriptionId(Guid.NewGuid()), subscriberId, Subscribtiond);
                await subRepo.AddAsync(sub);
                await _unitOfWork.SaveChangesAsync();
                return Ok(new ApiResponse<string>("Subscribed successfully."));
            }
        }
    }

    public class CreateSubscribtionDTO
    {
        public string Name { get; set; } = null!;
        public string Description { get; set; } = string.Empty;
    }

    public class SubscribtionDetailsDto
    {
        public Guid Id { get; set; }
        public Guid OwnerId { get; set; }
        public string Name { get; set; } = null!;
        public string Description { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}
