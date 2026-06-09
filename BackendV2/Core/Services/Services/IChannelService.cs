using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace YouTubeClone.Domain.Services
{
    public interface IChannelService
    {
        Task<Guid> CreateChannelAsync(CreateChannelDto dto, Guid ownerId);
        Task<ChannelAboutDto> GetChannelAboutAsync(Guid channelId);
        Task<IReadOnlyList<ChannelVideoDto>> GetChannelVideosAsync(Guid channelId);
    }
}
