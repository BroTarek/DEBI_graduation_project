using System;
using System.Threading.Tasks;
using YouTubeClone.Core.Services;

namespace YouTubeClone.Domain.Services
{
    public class ViewCountService : IViewCountService
    {
        public Task IncrementViewCountAsync(Guid videoId)
        {
            return Task.CompletedTask;
        }
    }
}
