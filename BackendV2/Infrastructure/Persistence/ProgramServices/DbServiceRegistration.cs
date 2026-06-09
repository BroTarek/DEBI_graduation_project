
using Makanak.Persistance.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Makanak.Abstraction.Storage;
using Makanak.Persistance.Services.Storage;
using Makanak.Persistance.Services.Storage2;
using Amazon.S3;
using Makanak.Domain.Contracts.InitializerDB;
using Makanak.Persistance.Implements.InitializerImplement;
using Makanak.Domain.Contracts.UOW;
using Core.Domain.Contracts.Repos;
using Makanak.Persistance.Implements;
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

            // Add IAmazonS3 Client (for S3 storage strategy)
            services.AddScoped<IAmazonS3>(sp => {
                var config = sp.GetRequiredService<IConfiguration>();
                var accessKey = config["AWS:AccessKey"];
                var secretKey = config["AWS:SecretKey"];
                var region = config["AWS:Region"];
                
                if (!string.IsNullOrEmpty(accessKey) && !string.IsNullOrEmpty(secretKey))
                {
                    var regionVal = Amazon.RegionEndpoint.GetBySystemName(region ?? "us-east-1");
                    return new AmazonS3Client(accessKey, secretKey, regionVal);
                }
                return new AmazonS3Client();
            });

            // Register Strategy-based Storage2 Services
            services.AddHttpContextAccessor();
            services.AddScoped<AwsS3VideoUploadStrategy>();
            services.AddScoped<CloudinaryVideoUploadStrategy>();
            services.AddScoped<LocalUploadStrategy>();
            services.AddScoped<IUploadStrategyFactory, UploadStrategyFactory>();
            services.AddScoped<UploadContext>();

            // Add DB Initializer
            services.AddScoped<IDbInitializer, DbInitialized>();

            // Add Unit of Work & Repositories
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped(typeof(IGenericRepo<,>), typeof(GenericRepoImp<,>));

            return services;
        }
    }
}