namespace Makanak.Persistance.Services.Storage2
{
    public interface IUploadStrategyFactory
    {
        IVideoUploadStrategy CreateStrategy(string uploadServiceProvider);
    }
}