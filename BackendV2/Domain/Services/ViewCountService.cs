using YouTubeClone.Domain.Aggregates.Videos;

namespace YouTubeClone.Domain.Services
{
    public interface IViewCountService
    {
        void IncrementViewCount(Video video);
    }

    public class ViewCountService : IViewCountService
    {
        public void IncrementViewCount(Video video)
        {
            video.IncrementViewCount();
        }
    }
}
