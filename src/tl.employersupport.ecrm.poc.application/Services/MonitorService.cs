using System;
using System.Threading.Tasks;
using tl.employersupport.ecrm.poc.application.Interfaces;

namespace tl.employersupport.ecrm.poc.application.Services
{
    public class MonitorService : IMonitorService
    {
        private readonly IFunctionsApiClient _functionsApiClient;

        public MonitorService(IFunctionsApiClient functionsApiClient)
        {
            _functionsApiClient = functionsApiClient ?? throw new ArgumentNullException(nameof(functionsApiClient));
        }

        public async Task CallTestTimeout(int clientTimeout, int minTimeout, int maxTimeout)
        {
            await _functionsApiClient.CallTestTimeoutFunction(clientTimeout, minTimeout, maxTimeout);
        }
    }
}
