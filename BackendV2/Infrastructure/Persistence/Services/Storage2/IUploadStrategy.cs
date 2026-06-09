using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace Makanak.Persistance.Services.Storage2
{
    public interface IUploadStrategy
    {
        Task<string> UploadAsync(IFormFile file, string folderName);
        Task DeleteAsync(string fileUrl);
    }
}
