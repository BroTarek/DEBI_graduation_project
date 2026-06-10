namespace YouTubeClone.Persistance.Services.Storage2
{
    public interface IUploadStrategyFactory
    {
        IUploadStrategy CreateStrategy(string uploadServiceProvider);
    }
}