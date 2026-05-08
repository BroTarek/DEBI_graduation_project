using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace Makanak.Abstraction.Storage
{
    public enum VideoStorageProvider
    {
        S3,
        Cloudinary
    }

    public interface IMediaStorageService
    {
        /// <summary>
        /// Uploads an image (like a post image or thumbnail) to local storage.
        /// </summary>
        /// <param name="file">The image file</param>
        /// <returns>The local URL/path to the image</returns>
        Task<string> UploadImageAsync(IFormFile file);

        /// <summary>
        /// Deletes an image from local storage.
        /// </summary>
        /// <param name="fileUrl">The local URL/path to the image</param>
        Task DeleteImageAsync(string fileUrl);

        /// <summary>
        /// Uploads a video to the specified cloud provider (S3 or Cloudinary).
        /// </summary>
        /// <param name="file">The video file</param>
        /// <param name="provider">The cloud provider to use</param>
        /// <returns>The URL to the uploaded video</returns>
        Task<string> UploadVideoAsync(IFormFile file, VideoStorageProvider provider);

        /// <summary>
        /// Deletes a video from the specified cloud provider.
        /// </summary>
        /// <param name="fileUrl">The URL of the video</param>
        /// <param name="provider">The cloud provider where the video is stored</param>
        Task DeleteVideoAsync(string fileUrl, VideoStorageProvider provider);
    }

    // Separate interfaces for specific providers (Optional, but good for DI)
    public interface ILocalStorageService
    {
        Task<string> UploadFileAsync(IFormFile file, string folderName);
        Task DeleteFileAsync(string fileUrl);
    }

    public interface IS3StorageService
    {
        Task<string> UploadVideoAsync(IFormFile file);
        Task DeleteVideoAsync(string fileUrl);
    }

    public interface ICloudinaryStorageService
    {
        Task<string> UploadVideoAsync(IFormFile file);
        Task DeleteVideoAsync(string fileUrl);
    }
}
