using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace Makanak.Persistance.Services.Storage2
{
    public interface IVideoUploadStrategy
    {
        Task<string> UploadVideoAsync(IFormFile file);
        Task DeleteVideoAsync(string fileUrl);
    }
}