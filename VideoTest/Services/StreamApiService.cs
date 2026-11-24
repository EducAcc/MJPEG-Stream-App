using Microsoft.Extensions.Options;
using VideoTest.Models;

namespace VideoTest.Services
{
    public class StreamApiService : IStreamApiService
    {
        private HttpClient _httpClient;

        public StreamApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<Channel>> GetActiveChannelsAsync()
        {
            string configUrl = "/configex?responsetype=json";

            ConfigexResponse? response = await _httpClient.GetFromJsonAsync<ConfigexResponse>(configUrl);
            
            if (response == null && response.Channels == null)
            {
                return [];
            }

            return response.Channels
                .Where(c => !c.IsDisabled)
                .ToList();
        }

        public async Task<HttpResponseMessage> OpenStreamAsync(Guid camId, CancellationToken cancelToken)
        {
            string videoUrl = $"/mobile?channelId={camId}";

            return await _httpClient.GetAsync(videoUrl, HttpCompletionOption.ResponseHeadersRead, cancelToken);
        }
    }
}
