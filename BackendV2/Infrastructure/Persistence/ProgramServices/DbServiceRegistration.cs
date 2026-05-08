
using Makanak.Persistance.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Makanak.Abstraction.Storage;
using Makanak.Persistance.Services.Storage;
using System;
using System.Collections.Generic;
using System.Text;

namespace Makanak.Persistance.ProgramServices
{
    public static class AddDbServices
    {
        public static IServiceCollection InjectDatabaseService(this IServiceCollection services, IConfiguration configuration)
        {
            // Add DbContext
            services.AddDbContext<MakanakDbContext>(options =>
               options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));
               
            // Add Storage Services
            services.AddScoped<ILocalStorageService, LocalStorageService>();
            services.AddScoped<IS3StorageService, S3StorageService>();
            services.AddScoped<ICloudinaryStorageService, CloudinaryStorageService>();
            services.AddScoped<IMediaStorageService, MediaStorageService>();

            return services;
        }
    }
}