using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Makanak.Abstraction.Storage;
using Microsoft.Extensions.Configuration;

// To fully implement this, you will need the AWSSDK.S3 nuget package.
// For now, this is the structural implementation.

namespace Makanak.Persistance.Services.Storage
{
    public class S3StorageService : IS3StorageService
    {
        private readonly string _bucketName;
        // private readonly IAmazonS3 _s3Client;

        public S3StorageService(IConfiguration configuration)
        {
            _bucketName = configuration["AWS:BucketName"] ?? "default-bucket-name";
            // _s3Client = s3Client;
        }

        public async Task<string> UploadVideoAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("File is empty or null.");

            var uniqueFileName = Guid.NewGuid().ToString() + "_" + file.FileName;

            /*
            var putRequest = new PutObjectRequest
            {
                BucketName = _bucketName,
                Key = uniqueFileName,
                InputStream = file.OpenReadStream(),
                ContentType = file.ContentType
            };

            await _s3Client.PutObjectAsync(putRequest);
            return $"https://{_bucketName}.s3.amazonaws.com/{uniqueFileName}";
            */

            // Dummy return until AWS SDK is installed
            await Task.Delay(100); 
            return $"https://{_bucketName}.s3.amazonaws.com/{uniqueFileName}";
        }

        public async Task DeleteVideoAsync(string fileUrl)
        {
            if (string.IsNullOrEmpty(fileUrl)) return;

            /*
            var key = ExtractKeyFromUrl(fileUrl);
            var deleteRequest = new DeleteObjectRequest
            {
                BucketName = _bucketName,
                Key = key
            };
            await _s3Client.DeleteObjectAsync(deleteRequest);
            */

            await Task.Delay(100); 
        }
    }
}
