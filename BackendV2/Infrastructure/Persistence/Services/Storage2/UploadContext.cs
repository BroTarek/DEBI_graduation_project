using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Makanak.Persistance.Services.Storage2
{
    public class UploadContext
    {
        private readonly IUploadStrategyFactory _factory;

        public UploadContext(IUploadStrategyFactory factory)
        {
            _factory = factory;
        }

        // Upload a video using a specific cloud provider (S3 or Cloudinary)
        public Task<string> UploadVideoAsync(IFormFile video, string preferredProvider)
        {
            return _factory.CreateStrategy(preferredProvider).UploadAsync(video, "videos");
        }

        // Upload an image locally under a specific folder (e.g. avatars, thumbnails, banners)
        public Task<string> UploadImageAsync(IFormFile image, string folderName)
        {
            return _factory.CreateStrategy("LOCAL").UploadAsync(image, folderName);
        }

        // Delete a video using a specific provider
        public Task DeleteVideoAsync(string videoUrl, string provider)
        {
            return _factory.CreateStrategy(provider).DeleteAsync(videoUrl);
        }

        // Delete a locally stored image
        public Task DeleteImageAsync(string imageUrl)
        {
            return _factory.CreateStrategy("LOCAL").DeleteAsync(imageUrl);
        }
    }
}
