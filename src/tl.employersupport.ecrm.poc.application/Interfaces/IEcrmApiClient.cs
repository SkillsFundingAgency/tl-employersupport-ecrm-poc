using System.Threading.Tasks;
using tl.employersupport.ecrm.poc.application.Model.Ecrm;

namespace tl.employersupport.ecrm.poc.application.Interfaces
{
    public interface IEcrmApiClient
    {
        Task<Employer> GetEmployer(EmployerSearchRequest searchRequest);
        Task<bool> GetHeartbeat();
    }
}
