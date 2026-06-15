using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using YouTubeClone.Services;
using YouTubeClone.Core.Services;
using YouTubeClone.Domain.Services;

namespace YouTubeClone.Web.Extensions
{
    public static class ApplicationServicesExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration config)
        {
            services.AddScoped<IVideoService, VideoService>();
            services.AddScoped<IPlaylistService, PlaylistService>();
            services.AddScoped<IChannelService, ChannelService>();
            services.AddScoped<ICommunityInteractionService, CommunityInteractionService>();
            services.AddScoped<ISubscriptionService, SubscriptionService>();
            services.AddScoped<IWatchHistoryService, WatchHistoryService>();
            services.AddScoped<IViewCountService, ViewCountService>();
            
            return services;
        }
    }
}
