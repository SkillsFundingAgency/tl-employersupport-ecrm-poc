using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using tl.employersupport.ecrm.poc.application.Extensions;
using tl.employersupport.ecrm.poc.application.Interfaces;
using tl.employersupport.ecrm.poc.application.Model.Ecrm;

namespace tl.employersupport.ecrm.poc.application.ApiClients
{
    public class EcrmApiClient : IEcrmApiClient
    {
        private readonly HttpClient _httpClient;
        // ReSharper disable once NotAccessedField.Local
        private readonly ILogger _logger;

        public EcrmApiClient(
            HttpClient httpClient,
            ILogger<EcrmApiClient> logger)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<Employer> GetEmployer(EmployerSearchRequest searchRequest)
        {
            //Temp test code
            if (_httpClient!.BaseAddress!.AbsoluteUri.StartsWith("https://dev.api.nationalcareersservice.org.uk"))
            {
                var testQueryString =
                    "{\r\n" +
                    "\"subjectKeyword\": \"Maths\",\r\n" +
                    "\"distance\": 10.0,\r\n" +
                    "\"postcode\": \"CV1 2WT\",\r\n" +
                    "\"limit\": 5\r\n" +
                    "}";

                var contentType = "application/json-patch+json";
                //_httpClient.DefaultRequestHeaders.Contains[ ]
                //req.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                // ReSharper disable once StringLiteralTypo

                var request = new HttpRequestMessage(HttpMethod.Post, "coursesearch");
                request.Content = new StringContent(testQueryString,
                    Encoding.UTF8,
                    contentType);//CONTENT-TYPE header

                // ReSharper disable StringLiteralTypo
                //var ncsContent = await _httpClient.PostAsJson("coursesearch", testQueryString);
                var response = await _httpClient.PostAsync("coursesearch",
                    new StringContent(testQueryString,
                        Encoding.UTF8,
                        contentType));
                // ReSharper restore StringLiteralTypo

                if (response.StatusCode != HttpStatusCode.OK)
                {
                    //_logger.LogError($"API call failed with {response.StatusCode} - {response.ReasonPhrase}");
                }

                response.EnsureSuccessStatusCode();

                var jsonResult = await response.Content.ReadAsStringAsync();

                return new Employer
                {
                    AccountId = Guid.NewGuid(),
                    CompanyName = searchRequest.CompanyName
                };
            }

            var requestQueryString = JsonSerializer.Serialize(searchRequest, JsonExtensions.DefaultJsonSerializerOptions);
            
            var content = await _httpClient.PostAsJson($"employerSearch", searchRequest);

            var json = await content.ReadAsStringAsync();

            return JsonSerializer
                .Deserialize<Employer>(
                    json,
                    JsonExtensions.DefaultJsonSerializerOptions);
        }
    }
}
