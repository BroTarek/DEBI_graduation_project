using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using YouTubeClone.Core.Services;
using YouTubeClone.Domain.Services;
using YouTubeClone.Infrastructure.Persistence.Services.PlaylistService;
using YouTubeClone.Infrastructure.Persistence.Services.CommentService;
using YouTubeClone.Infrastructure.Persistence.Services.Subscribtion;
using YouTubeClone.Infrastructure.Persistence.Services.Post;

namespace YouTubeClone.Web.Extensions
{
    public static class ApplicationServicesExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration config)
        {
            services.AddScoped<IVideoService, VideoService>();
            services.AddScoped<IPlaylistService, PlaylistService>();
            services.AddScoped<ICommentService, CommentService>();
            services.AddScoped<IChannelService, ChannelService>();
            services.AddScoped<ISubscribtionService, SubscribtionService>();
            services.AddScoped<IWatchHistoryService, WatchHistoryService>();
            services.AddScoped<IViewCountService, ViewCountService>();
            services.AddScoped<IPostService, PostService>();
            
            return services;
        }
    }
}
