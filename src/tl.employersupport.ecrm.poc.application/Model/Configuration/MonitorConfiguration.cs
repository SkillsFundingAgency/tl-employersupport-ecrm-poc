
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable UnusedMember.Global
namespace tl.employersupport.ecrm.poc.application.Model.Configuration
{
    public class MonitorConfiguration
    {
        public string ApiBaseUri { get; init; }
        public int HttpTimeoutInSeconds { get; init; }
        public string TimeoutFunctionUri { get; init; }
        public string TimeoutFunctionCode { get; init; }
    }
}
