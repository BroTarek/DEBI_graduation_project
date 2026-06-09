public class CloudinaryVideoUploadStrategy : IVideoUploadStrategy {
    private readonly Cloudinary _cloudinary;

        public CloudinaryStorageService(IConfiguration configuration)
        {
            var account = new Account(
                configuration["Cloudinary:CloudName"],
                configuration["Cloudinary:ApiKey"],
                configuration["Cloudinary:ApiSecret"]
            );
            _cloudinary = new Cloudinary(account);
        }
   public async Task<string> UploadVideoAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("File is empty or null.");

            var uniqueFileName = Guid.NewGuid().ToString();

            using var stream = file.OpenReadStream();
            var uploadParams = new VideoUploadParams()
            {
                File = new FileDescription(file.FileName, stream),
                PublicId = uniqueFileName,
                Overwrite = true
            };

            var uploadResult = await _cloudinary.UploadAsync(uploadParams);

            if (uploadResult.Error != null)
                throw new Exception($"Cloudinary video upload failed: {uploadResult.Error.Message}");

            return uploadResult.SecureUrl.ToString();
        }

        public async Task DeleteVideoAsync(string fileUrl)
        {
            if (string.IsNullOrEmpty(fileUrl)) return;

            var publicId = ExtractPublicIdFromUrl(fileUrl);
            var deletionParams = new DeletionParams(publicId) 
            { 
                ResourceType = ResourceType.Video 
            };
            
            var deletionResult = await _cloudinary.DestroyAsync(deletionParams);

            if (deletionResult.Error != null)
                throw new Exception($"Cloudinary video deletion failed: {deletionResult.Error.Message}");
        }

        private string ExtractPublicIdFromUrl(string fileUrl)
        {
            try
            {
                var uri = new Uri(fileUrl);
                var segments = uri.Segments;
                var filename = segments.Last();
                var dotIndex = filename.LastIndexOf('.');
                return dotIndex > 0 ? filename.Substring(0, dotIndex) : filename;
            }
            catch (Exception)
            {
                return fileUrl;
            }
        }
}