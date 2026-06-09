using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Makanak.Persistance.Services.Storage2
{
    public class VideoUploadContext
    {
        private readonly IUploadStrategyFactory _factory;

        public VideoUploadContext(IUploadStrategyFactory factory)
        {
            _factory = factory;
        }

        public Task<string> ExecuteUploadAsync(IFormFile video, string uploadService)
        {
            return _factory.CreateStrategy(uploadService).UploadVideoAsync(video);
        }

        public Task ExecuteDeleteAsync(string uploadUrl, string uploadService)
        {
            return _factory.CreateStrategy(uploadService).DeleteVideoAsync(uploadUrl);
        }
    }
}