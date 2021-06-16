using System.Threading.Tasks;

namespace tl.employersupport.ecrm.poc.application.Interfaces
{
    public interface IMonitorService
    {
        Task CallTestTimeout(int clientTimeout, int minTimeout, int maxTimeout);
    }
}
