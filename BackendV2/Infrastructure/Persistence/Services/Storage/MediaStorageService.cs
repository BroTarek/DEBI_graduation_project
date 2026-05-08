using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Makanak.Abstraction.Storage;

namespace Makanak.Persistance.Services.Storage
{
    public class MediaStorageService : IMediaStorageService
    {
        private readonly ILocalStorageService _localService;
        private readonly IS3StorageService _s3Service;
        private readonly ICloudinaryStorageService _cloudinaryService;

        public MediaStorageService(
            ILocalStorageService localService,
            IS3StorageService s3Service,
            ICloudinaryStorageService cloudinaryService)
        {
            _localService = localService;
            _s3Service = s3Service;
            _cloudinaryService = cloudinaryService;
        }

        public async Task<string> UploadImageAsync(IFormFile file)
        {
            // Images and thumbnails go to local storage
            return await _localService.UploadFileAsync(file, "images");
        }

        public async Task DeleteImageAsync(string fileUrl)
        {
            await _localService.DeleteFileAsync(fileUrl);
        }

        public async Task<string> UploadVideoAsync(IFormFile file, VideoStorageProvider provider)
        {
            // Videos go to the specified cloud provider
            if (provider == VideoStorageProvider.S3)
            {
                return await _s3Service.UploadVideoAsync(file);
            }
            else
            {
                return await _cloudinaryService.UploadVideoAsync(file);
            }
        }

        public async Task DeleteVideoAsync(string fileUrl, VideoStorageProvider provider)
        {
            if (provider == VideoStorageProvider.S3)
            {
                await _s3Service.DeleteVideoAsync(fileUrl);
            }
            else
            {
                await _cloudinaryService.DeleteVideoAsync(fileUrl);
            }
        }
    }
}
