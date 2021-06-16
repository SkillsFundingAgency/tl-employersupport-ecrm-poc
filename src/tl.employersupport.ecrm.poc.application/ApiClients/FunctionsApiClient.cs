using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using tl.employersupport.ecrm.poc.application.Interfaces;
using tl.employersupport.ecrm.poc.application.Model.Configuration;

namespace tl.employersupport.ecrm.poc.application.ApiClients
{
    public class FunctionsApiClient : IFunctionsApiClient
    {
        private readonly MonitorConfiguration _configuration;
        private readonly HttpClient _httpClient;

        public FunctionsApiClient(
            HttpClient httpClient,
            IOptions<MonitorConfiguration> monitorConfiguration)
        {
            _httpClient = httpClient;

            if (monitorConfiguration is null)
                throw new ArgumentNullException(nameof(monitorConfiguration));

            _configuration = monitorConfiguration.Value ??
                             throw new ArgumentNullException(
                                 $"{nameof(monitorConfiguration)}.{nameof(monitorConfiguration.Value)}",
                                 "Configuration value must not be null");

            _httpClient.BaseAddress = new Uri(_configuration.ApiBaseUri);
            _httpClient.DefaultRequestHeaders.Add(HeaderNames.UserAgent, nameof(FunctionsApiClient));
        }

        public async Task CallTestTimeoutFunction(int clientTimeout, int minTimeout, int maxTimeout)
        {
            _httpClient.Timeout = TimeSpan.FromSeconds(clientTimeout);

            var queryParameters = HttpUtility.ParseQueryString(string.Empty);
            queryParameters["minTimeout"] = minTimeout.ToString();
            queryParameters["maxTimeout"] = maxTimeout.ToString();
            var query = queryParameters.ToString();

            var code = !string.IsNullOrEmpty(_configuration.TimeoutFunctionCode)
                ? $"&code={_configuration.TimeoutFunctionCode}"
                : "";
            var requestUri = $"{_configuration.TimeoutFunctionUri}?minTimeout={minTimeout}&maxTimeout={maxTimeout}{code}";

            var response = await _httpClient.GetAsync(requestUri);
        }
    }
}
