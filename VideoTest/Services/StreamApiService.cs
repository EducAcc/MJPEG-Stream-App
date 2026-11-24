using VideoTest.Models;

namespace VideoTest.Services
{
    public class StreamApiService : IStreamApiService
    {
        private readonly HttpClient _httpClient;

        public StreamApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<Channel>> GetActiveChannelsAsync()
        {
            string configUrl = "/configex?responsetype=json";

            ConfigexResponse? response = await _httpClient.GetFromJsonAsync<ConfigexResponse>(configUrl);
            
            if (response == null || response.Channels == null)
            {
                return [];
            }

            return response.Channels
                .Where(c => !c.IsDisabled)
                .ToList();
        }

        public async Task<HttpResponseMessage> OpenStreamAsync(Guid channelId, CancellationToken cancelToken)
        {
            string videoUrl = $"/mobile?channelId={channelId}";

            return await _httpClient.GetAsync(videoUrl, HttpCompletionOption.ResponseHeadersRead, cancelToken);
        }
    }
}
