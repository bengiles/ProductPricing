using System.Net.Http.Json;

namespace ProductPricing.Helpers
{
    public interface IApiHelper
    {
        Task<T?> GetAsync<T>(string url);
        Task<T?> PostAsync<T>(string url, object data);
        Task<T?> PutAsync<T>(string url, object data);
    }

    public class ApiHelper(HttpClient httpClient) : IApiHelper
    {
        public async Task<T?> GetAsync<T>(string url)
        {
            var response = await httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<T>();
        }

        public async Task<T?> PostAsync<T>(string url, object data)
        {
            var response = await httpClient.PostAsJsonAsync(url, data);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<T>();
        }

        public async Task<T?> PutAsync<T>(string url, object data)
        {
            var response = await httpClient.PutAsJsonAsync(url, data);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<T>();
        }
    }
}
