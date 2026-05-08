
using .Services.AutoMapper.Admin;
using .Services.AutoMapper.AmenityMapper;
using .Services.AutoMapper.BookingMapper;
using .Services.AutoMapper.DisputeMapper;
using .Services.AutoMapper.GovernorateMapper;
using .Services.AutoMapper.NotificationMapper;
using .Services.AutoMapper.PropertyMapper;
using .Services.AutoMapper.ReviewMapper;
using .Services.AutoMapper.User;
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