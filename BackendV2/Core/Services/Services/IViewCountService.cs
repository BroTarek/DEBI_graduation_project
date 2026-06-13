using System.Threading.Tasks;

namespace YouTubeClone.Core.Services
{
    public interface IViewCountService
    {
        Task IncrementViewCountAsync(System.Guid videoId);
    }
}
