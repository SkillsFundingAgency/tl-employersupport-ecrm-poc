using System.Threading.Tasks;

namespace tl.employersupport.ecrm.poc.application.Interfaces
{
    public interface IFunctionsApiClient
    {
        Task CallTestTimeoutFunction(int clientTimeout, int minTimeout, int maxTimeout);
    }
}
