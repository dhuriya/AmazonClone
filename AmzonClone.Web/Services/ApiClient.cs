using AmzonClone.Web.Services.Interfaces;
using System.Net.Http.Headers;

namespace AmzonClone.Web.Services
{
    public class ApiClient : IApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public ApiClient(IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = httpClientFactory.CreateClient("AmazonApi");
            _httpContextAccessor = httpContextAccessor;
        }
        private HttpRequestMessage CreateRequest(HttpMethod method,string endpoint,HttpContent? content = null)
        {
            var request = new HttpRequestMessage(method, endpoint)
            {
                Content = content
            };
            var httpContext = _httpContextAccessor.HttpContext;
            var controller = httpContext?.Request.RouteValues["controller"]?.ToString();
            var tokenKey = string.Equals(controller, "Admin", StringComparison.OrdinalIgnoreCase)
                ? "AdminAccessToken"
                : "CustomerAccessToken";
            var token = httpContext?.Session.GetString(tokenKey);
            if (!string.IsNullOrWhiteSpace(token))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
            return request;
        }
        public async Task<T?> GetAsync<T>(string endpoint, CancellationToken cancellationToken = default)
        {
            using var request = CreateRequest(HttpMethod.Get, endpoint);
            using var response = await _httpClient.SendAsync(request, cancellationToken);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<T>(cancellationToken);
        }
        //public async Task<TResponse?> PostAsync<TRequest, TResponse>(string endpoint, TRequest request, CancellationToken cancellationToken = default)
        //{

        //    var response = await _httpClient.PostAsJsonAsync(endpoint, request, cancellationToken);
        //    response.EnsureSuccessStatusCode();
        //    return await response.Content.ReadFromJsonAsync<TResponse>(cancellationToken);
        //}
        public async Task<TResponse?> PostAsync<TRequest, TResponse>(string endpoint,TRequest body,CancellationToken cancellationToken = default)
        {
            using var content = JsonContent.Create(body);
            using var request = CreateRequest(HttpMethod.Post, endpoint, content);
            using var response = await _httpClient.SendAsync(request, cancellationToken);

            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<TResponse>(cancellationToken);
        }
        //public async Task<TResponse?> PutAsync<TRequest, TResponse>(string endpoint, TRequest request, CancellationToken cancellationToken = default)
        //{
        //    var response = await _httpClient.PutAsJsonAsync(endpoint, request, cancellationToken);
        //    response.EnsureSuccessStatusCode();
        //    return await response.Content.ReadFromJsonAsync<TResponse>(cancellationToken);
        //}
        public async Task<TResponse?> PutAsync<TRequest, TResponse>(string endpoint,TRequest body,CancellationToken cancellationToken = default)
        {
            using var content = JsonContent.Create(body);
            using var request = CreateRequest(HttpMethod.Put, endpoint, content);
            using var response = await _httpClient.SendAsync(request, cancellationToken);

            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<TResponse>(cancellationToken);
        }
        //public async Task<bool> DeleteAsync(string endpoint, CancellationToken cancellationToken = default)
        //{
        //    var response = await _httpClient.DeleteAsync(endpoint, cancellationToken);
        //    return response.IsSuccessStatusCode;
        //}
        public async Task<bool> DeleteAsync(string endpoint,CancellationToken cancellationToken = default)
        {
            using var request = CreateRequest(HttpMethod.Delete, endpoint);
            using var response = await _httpClient.SendAsync(request, cancellationToken);

            return response.IsSuccessStatusCode;
        }
        public async Task<TResponse?> PostMultipartAsync<TResponse>(string endpoint, MultipartFormDataContent content, CancellationToken cancellationToken = default)
        {
            using var request = CreateRequest(HttpMethod.Post, endpoint, content);
            using var response = await _httpClient.SendAsync(request, cancellationToken);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<TResponse>(cancellationToken);
        }

        public async Task<TResponse?> PutMultipartAsync<TResponse>(string endpoint, MultipartFormDataContent content, CancellationToken cancellationToken = default)
        {
            using var request = CreateRequest(HttpMethod.Put, endpoint, content);
            using var response = await _httpClient.SendAsync(request, cancellationToken);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<TResponse>(cancellationToken);
        }
    }
}
