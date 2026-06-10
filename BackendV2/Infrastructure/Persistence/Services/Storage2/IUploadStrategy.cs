using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace YouTubeClone.Persistance.Services.Storage2
{
    public interface IUploadStrategy
    {
        Task<string> UploadAsync(IFormFile file, string folderName);
        Task DeleteAsync(string fileUrl);
    }
}
