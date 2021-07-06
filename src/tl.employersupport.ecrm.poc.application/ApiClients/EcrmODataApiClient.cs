using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using tl.employersupport.ecrm.poc.application.Extensions;
using tl.employersupport.ecrm.poc.application.Interfaces;
using tl.employersupport.ecrm.poc.application.Model.Ecrm;

namespace tl.employersupport.ecrm.poc.application.ApiClients
{
    public class EcrmODataApiClient : IEcrmODataApiClient
    {
        private readonly IAuthenticationService _authenticationService;
        private readonly HttpClient _httpClient;
        // ReSharper disable once NotAccessedField.Local
        private readonly ILogger _logger;

        public EcrmODataApiClient(
            HttpClient httpClient,
            IAuthenticationService authenticationService,
            ILogger<EcrmODataApiClient> logger)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _authenticationService = authenticationService ?? throw new ArgumentNullException(nameof(authenticationService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<WhoAmIResponse> GetWhoAmI()
        {
            var accessToken = await _authenticationService.GetAccessToken();
            
            var request = new HttpRequestMessage(HttpMethod.Get, "WhoAmI");
            request.Headers.CacheControl = new CacheControlHeaderValue { NoCache = true };
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer",  accessToken );

            var response = await _httpClient.SendAsync(request);

            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();

            var whoAmI = JsonSerializer
                .Deserialize<WhoAmIResponse>(
                    json,
                    JsonExtensions.DefaultJsonSerializerOptions);
            return whoAmI;
        }
    }
}
