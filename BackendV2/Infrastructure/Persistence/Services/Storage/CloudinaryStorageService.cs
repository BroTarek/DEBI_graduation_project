using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Makanak.Abstraction.Storage;
using Microsoft.Extensions.Configuration;

// To fully implement this, you will need the CloudinaryDotNet nuget package.
// For now, this is the structural implementation.

namespace Makanak.Persistance.Services.Storage
{
    public class CloudinaryStorageService : ICloudinaryStorageService
    {
        private readonly string _cloudName;

        public CloudinaryStorageService(IConfiguration configuration)
        {
            _cloudName = configuration["Cloudinary:CloudName"] ?? "default-cloud";
        }

        public async Task<string> UploadVideoAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("File is empty or null.");

            var uniqueFileName = Guid.NewGuid().ToString();

            /*
            var uploadParams = new VideoUploadParams()
            {
                File = new FileDescription(file.FileName, file.OpenReadStream()),
                PublicId = uniqueFileName
            };

            var uploadResult = await _cloudinary.UploadAsync(uploadParams);
            return uploadResult.SecureUrl.ToString();
            */

            await Task.Delay(100); 
            return $"https://res.cloudinary.com/{_cloudName}/video/upload/v1/{uniqueFileName}.mp4";
        }

        public async Task DeleteVideoAsync(string fileUrl)
        {
            if (string.IsNullOrEmpty(fileUrl)) return;

            /*
            var publicId = ExtractPublicIdFromUrl(fileUrl);
            var deletionParams = new DeletionParams(publicId) { ResourceType = ResourceType.Video };
            await _cloudinary.DestroyAsync(deletionParams);
            */

            await Task.Delay(100); 
        }
    }
}
