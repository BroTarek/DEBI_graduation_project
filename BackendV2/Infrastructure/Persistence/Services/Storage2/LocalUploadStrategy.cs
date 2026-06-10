using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace YouTubeClone.Persistance.Services.Storage2
{
    public class LocalUploadStrategy : IUploadStrategy
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public LocalUploadStrategy(IWebHostEnvironment webHostEnvironment, IHttpContextAccessor httpContextAccessor)
        {
            _webHostEnvironment = webHostEnvironment;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<string> UploadAsync(IFormFile file, string folderName)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("File is empty or null.");

            var webRoot = _webHostEnvironment.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            var uploadsFolder = Path.Combine(webRoot, "uploads", folderName);

            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            var uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(file.FileName);
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var request = _httpContextAccessor.HttpContext?.Request;
            if (request != null)
            {
                var scheme = request.Scheme;
                var host = request.Host;
                return $"{scheme}://{host}/uploads/{folderName}/{uniqueFileName}";
            }

            return $"/uploads/{folderName}/{uniqueFileName}";
        }

        public Task DeleteAsync(string fileUrl)
        {
            if (string.IsNullOrEmpty(fileUrl))
                return Task.CompletedTask;

            try
            {
                string relativePath = "";
                if (Uri.TryCreate(fileUrl, UriKind.Absolute, out var uri))
                {
                    relativePath = uri.LocalPath.TrimStart('/');
                }
                else
                {
                    relativePath = fileUrl.TrimStart('/');
                }

                var webRoot = _webHostEnvironment.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
                var filePath = Path.Combine(webRoot, relativePath);

                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
            }
            catch
            {
                // Silently ignore or log delete errors
            }

            return Task.CompletedTask;
        }
    }
}
