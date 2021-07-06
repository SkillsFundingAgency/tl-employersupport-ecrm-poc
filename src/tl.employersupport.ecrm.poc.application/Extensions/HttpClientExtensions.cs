using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;

namespace tl.employersupport.ecrm.poc.application.Extensions
{
    public static class HttpClientExtensions
    {
        public static async Task<string> GetJson(this HttpClient httpClient, string requestUri)
        {
            var content = await httpClient.GetHttp(requestUri);
            return await content.ReadAsStringAsync();
        }

        public static async Task<JsonDocument> GetJsonDocument(this HttpClient httpClient, string requestUri)
        {
            var content = await httpClient.GetHttp(requestUri);
            return await JsonDocument.ParseAsync(await content.ReadAsStreamAsync());
        }

        public static async Task<HttpContent> GetHttp(this HttpClient httpClient, string requestUri)
        {
            //_logger.LogInformation($"Calling API {_httpClient.BaseAddress} endpoint {requestUri}");

            var response = await httpClient.GetAsync(requestUri);
            if (response.StatusCode != HttpStatusCode.OK)
            {
                //_logger.LogError($"API call failed with {response.StatusCode} - {response.ReasonPhrase}");
            }

            response.EnsureSuccessStatusCode();

            return response.Content;
        }

        public static async Task<HttpContent> PostAsJson<TValue>(this HttpClient httpClient, string requestUri, TValue value)
        {
            //_logger.LogInformation($"Calling API {_httpClient.BaseAddress} endpoint {requestUri}");

            var response = await httpClient.PostAsJsonAsync(requestUri, value, JsonExtensions.CamelCaseJsonSerializerOptions);

            if (response.StatusCode != HttpStatusCode.OK)
            {
                //_logger.LogError($"API call failed with {response.StatusCode} - {response.ReasonPhrase}");
            }

            response.EnsureSuccessStatusCode();

            return response.Content;
        }

        public static async Task<HttpContent> PutAsJson<TValue>(this HttpClient httpClient, string requestUri, TValue value)
        {
            //_logger.LogInformation($"Calling API {_httpClient.BaseAddress} endpoint {requestUri}");

            var response = await httpClient.PutAsJsonAsync(requestUri, value, JsonExtensions.CamelCaseJsonSerializerOptions);

            if (response.StatusCode != HttpStatusCode.OK)
            {
                //_logger.LogError($"API call failed with {response.StatusCode} - {response.ReasonPhrase}");
            }

            response.EnsureSuccessStatusCode();

            return response.Content;
        }
    }
}
