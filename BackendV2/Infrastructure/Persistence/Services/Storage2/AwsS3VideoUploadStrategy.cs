using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Amazon.S3;
using Amazon.S3.Model;

namespace YouTubeClone.Persistance.Services.Storage2
{
    public class AwsS3VideoUploadStrategy : IUploadStrategy
    {
        private readonly string _bucketName;
        private readonly IAmazonS3 _s3Client;

        public AwsS3VideoUploadStrategy(IConfiguration configuration, IAmazonS3 s3Client)
        {
            _bucketName = configuration["AWS:BucketName"] ?? "default-bucket-name";
            _s3Client = s3Client;
        }

        public async Task<string> UploadAsync(IFormFile file, string folderName)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("File is empty or null.");

            var prefix = string.IsNullOrEmpty(folderName) ? "" : folderName.TrimEnd('/') + "/";
            var uniqueFileName = prefix + Guid.NewGuid().ToString() + "_" + file.FileName;

            using var stream = file.OpenReadStream();
            var putRequest = new PutObjectRequest
            {
                BucketName = _bucketName,
                Key = uniqueFileName,
                InputStream = stream,
                ContentType = file.ContentType
            };

            await _s3Client.PutObjectAsync(putRequest);
            return $"https://{_bucketName}.s3.amazonaws.com/{uniqueFileName}";
        }

        public async Task DeleteAsync(string fileUrl)
        {
            if (string.IsNullOrEmpty(fileUrl)) return;

            var key = ExtractKeyFromUrl(fileUrl);
            var deleteRequest = new DeleteObjectRequest
            {
                BucketName = _bucketName,
                Key = key
            };

            await _s3Client.DeleteObjectAsync(deleteRequest);
        }

        private string ExtractKeyFromUrl(string fileUrl)
        {
            try
            {
                var uri = new Uri(fileUrl);
                var absolutePath = uri.AbsolutePath.TrimStart('/');
                
                if (uri.Host.StartsWith(_bucketName, StringComparison.OrdinalIgnoreCase))
                {
                    return absolutePath;
                }

                if (absolutePath.StartsWith(_bucketName + "/", StringComparison.OrdinalIgnoreCase))
                {
                    return absolutePath.Substring(_bucketName.Length + 1);
                }

                return absolutePath;
            }
            catch (Exception)
            {
                return fileUrl;
            }
        }
    }
}