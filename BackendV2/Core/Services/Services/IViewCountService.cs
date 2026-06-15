using System;
using System.Threading.Tasks;

namespace YouTubeClone.Core.Services
{
    public interface IViewCountService
    {
        Task IncrementViewCountAsync(Guid videoId);
    }
}
