using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YouTubeClone.Domain.Contracts.UOW;
using YouTubeClone.Domain.Aggregates.Channels;
using YouTubeClone.Domain.Aggregates.Users;
using YouTubeClone.Domain.Aggregates.Videos;
using YouTubeClone.Domain.ValueObjects;

namespace YouTubeClone.Domain.Services
{
    public class ChannelService : IChannelService
    {
        private readonly IUnitOfWork _unitOfWork;
        
        public ChannelService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Guid> CreateChannelAsync(CreateChannelDto dto, Guid ownerId)
        {
            return Guid.NewGuid();
        }

        public async Task<ChannelAboutDto> GetChannelAboutAsync(Guid id)
        {
            return new ChannelAboutDto();
        }

        public async Task<IReadOnlyList<ChannelVideoDto>> GetChannelVideosAsync(Guid id)
        {
            return new List<ChannelVideoDto>();
        }
    }
}