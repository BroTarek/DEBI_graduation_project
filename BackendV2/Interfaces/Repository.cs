public interface IVideoRepository
{
    Task<List<Video>> FindByOwnerIdAsync(string ownerId);
    Task<List<Video>> FindByStatusAsync(VideoStatus status);
    Task<List<Video>> SearchVideosAsync(string? type, string? breed, int? maxAge, string? location);
    Task UpdateAsync(Video Video);
}
