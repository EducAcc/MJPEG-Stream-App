using VideoTest.Models;

namespace VideoTest.Services
{
    public interface IStreamApiService
    {
        Task<List<Channel>> GetActiveChannelsAsync();

        Task<HttpResponseMessage> OpenStreamAsync(Guid channelId, CancellationToken cancelToken);
    }
}
