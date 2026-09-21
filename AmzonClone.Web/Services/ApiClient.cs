using AmzonClone.Web.Services.Interfaces;

namespace AmzonClone.Web.Services
{
    public class ApiClient : IApiClient
    {
        private readonly HttpClient _httpClient;
        public ApiClient(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("AmazonApi");
        }
        public async Task<T?> GetAsync<T>(string endpoint, CancellationToken cancellationToken = default)
        {
            var response = await _httpClient.GetAsync(endpoint, cancellationToken);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<T>(cancellationToken);
        }
        public async Task<TResponse?> PostAsync<TRequest, TResponse>(string endpoint, TRequest request, CancellationToken cancellationToken = default)
        {
            var response = await _httpClient.PostAsJsonAsync(endpoint, request, cancellationToken);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<TResponse>(cancellationToken);
        }
        public async Task<TResponse?> PutAsync<TRequest, TResponse>(string endpoint, TRequest request, CancellationToken cancellationToken = default)
        {
            var response = await _httpClient.PutAsJsonAsync(endpoint, request, cancellationToken);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<TResponse>(cancellationToken);
        }
        public async Task<bool> DeleteAsync(string endpoint, CancellationToken cancellationToken = default)
        {
            var response = await _httpClient.DeleteAsync(endpoint, cancellationToken);
            return response.IsSuccessStatusCode;
        }
    }
}
