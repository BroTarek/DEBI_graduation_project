using System;
using Microsoft.Extensions.DependencyInjection;

namespace Makanak.Persistance.Services.Storage2
{
    public class UploadStrategyFactory : IUploadStrategyFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public UploadStrategyFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IVideoUploadStrategy CreateStrategy(string preferredUploadServiceProvider)
        {
            if (string.IsNullOrEmpty(preferredUploadServiceProvider))
                throw new ArgumentException("Preferred upload service provider cannot be null or empty.", nameof(preferredUploadServiceProvider));

            return preferredUploadServiceProvider.ToUpperInvariant() switch
            {
                "S3" => _serviceProvider.GetRequiredService<AwsS3VideoUploadStrategy>(),
                "CLOUDINARY" => _serviceProvider.GetRequiredService<CloudinaryVideoUploadStrategy>(),
                _ => throw new NotSupportedException($"Upload provider '{preferredUploadServiceProvider}' is not supported.")
            };
        }
    }
}