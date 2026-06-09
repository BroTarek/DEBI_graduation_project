
using SoftBridge.Services.AutoMapper.Admin;
using SoftBridge.Services.AutoMapper.AmenityMapper;
using SoftBridge.Services.AutoMapper.BookingMapper;
using SoftBridge.Services.AutoMapper.DisputeMapper;
using SoftBridge.Services.AutoMapper.GovernorateMapper;
using SoftBridge.Services.AutoMapper.NotificationMapper;
using SoftBridge.Services.AutoMapper.PropertyMapper;
using SoftBridge.Services.AutoMapper.ReviewMapper;
using SoftBridge.Services.AutoMapper.User;
using Microsoft.Extensions.DependencyInjection;
namespace SoftBridge.Services.AutoMapper
{
    public static class AutoMapperService
    {
        public static IServiceCollection InjectAutoMapperService(this IServiceCollection services)
        {
            services.AddAutoMapper(cfg =>
            {
                //cfg.AddProfile(new [Auth]Profile());
                cfg.AddProfile(new UserProfile());
                cfg.AddProfile(new AdminProfile());
                cfg.AddProfile(new PropertyProfile());
                cfg.AddProfile(new BookingProfile());
                cfg.AddProfile(new ReviewProfile());
                cfg.AddProfile(new NotificationProfile());
                cfg.AddProfile(new DisputeProfile());
                cfg.AddProfile(new GovernorateProfile());
                cfg.AddProfile(new AmenityProfile());
            });
            return services;
        }
    }
}