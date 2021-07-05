using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
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
            var accessToken = _authenticationService.GetAccessToken();

            var request = new HttpRequestMessage(HttpMethod.Get, "WhoAmI");
            request.Headers.CacheControl = new CacheControlHeaderValue { NoCache = true };
            //request.Headers.Add("Ocp-Apim-Trace", "true");
            //Authorization $"Bearer {accessToken}");
            //request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

            var response = await _httpClient.SendAsync(request);

            /*
{
    "@odata.context": "https://esfa-cs-dev.api.crm4.dynamics.com/api/data/v9.2/$metadata#Microsoft.Dynamics.CRM.WhoAmIResponse",
    "BusinessUnitId": "0736860e-34ff-e811-a990-000d3a2cd74d",
    "UserId": "8cca658d-59da-eb11-bacb-0022487fdd4b",
    "OrganizationId": "ea5a0e24-b412-47f1-b726-efd241f0d9f3"
}
            */

            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();

            return JsonSerializer
                .Deserialize<WhoAmIResponse>(
                    json,
                    JsonExtensions.DefaultJsonSerializerOptions);
        }
    }
}
