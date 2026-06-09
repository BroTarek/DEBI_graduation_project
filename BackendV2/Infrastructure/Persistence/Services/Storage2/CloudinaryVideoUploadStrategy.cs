using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;

namespace Makanak.Persistance.Services.Storage2
{
    public class CloudinaryVideoUploadStrategy : IUploadStrategy
    {
        private readonly Cloudinary _cloudinary;

        public CloudinaryVideoUploadStrategy(IConfiguration configuration)
        {
            var account = new Account(
                configuration["Cloudinary:CloudName"],
                configuration["Cloudinary:ApiKey"],
                configuration["Cloudinary:ApiSecret"]
            );
            _cloudinary = new Cloudinary(account);
        }

        public async Task<string> UploadAsync(IFormFile file, string folderName)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("File is empty or null.");

            var uniqueFileName = Guid.NewGuid().ToString();

            using var stream = file.OpenReadStream();
            var uploadParams = new VideoUploadParams()
            {
                File = new FileDescription(file.FileName, stream),
                PublicId = uniqueFileName,
                Overwrite = true,
                Folder = folderName
            };

            var uploadResult = await _cloudinary.UploadAsync(uploadParams);

            if (uploadResult.Error != null)
                throw new Exception($"Cloudinary upload failed: {uploadResult.Error.Message}");

            return uploadResult.SecureUrl.ToString();
        }

        public async Task DeleteAsync(string fileUrl)
        {
            if (string.IsNullOrEmpty(fileUrl)) return;

            var publicId = ExtractPublicIdFromUrl(fileUrl);
            var deletionParams = new DeletionParams(publicId) 
            { 
                ResourceType = ResourceType.Video 
            };
            
            var deletionResult = await _cloudinary.DestroyAsync(deletionParams);

            if (deletionResult.Error != null)
                throw new Exception($"Cloudinary deletion failed: {deletionResult.Error.Message}");
        }

        private string ExtractPublicIdFromUrl(string fileUrl)
        {
            try
            {
                var uri = new Uri(fileUrl);
                var segments = uri.Segments;
                var filename = segments.Last();
                var dotIndex = filename.LastIndexOf('.');
                return dotIndex > 0 ? filename.Substring(0, dotIndex) : filename;
            }
            catch (Exception)
            {
                return fileUrl;
            }
        }
    }
}