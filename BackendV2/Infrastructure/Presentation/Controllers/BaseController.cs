using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace YouTubeClone.Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public abstract class BaseController : ControllerBase
    {
        protected string GetUserId()
        {
            if (Request.Query.TryGetValue("userId", out var queryUserId))
            {
                return queryUserId.ToString();
            }
            if (Request.Headers.TryGetValue("X-User-Id", out var headerUserId))
            {
                return headerUserId.ToString();
            }
            return User.FindFirstValue(ClaimTypes.NameIdentifier);
        }
    }
}
