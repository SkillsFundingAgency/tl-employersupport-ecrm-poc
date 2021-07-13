using System;
using System.Threading.Tasks;
using tl.employersupport.ecrm.poc.application.Model.Ecrm;

namespace tl.employersupport.ecrm.poc.application.Interfaces
{
    public interface IEcrmApiClient
    {
        Task<Account> GetAccount(Guid accountId);
        Task<Employer> GetEmployer(EmployerSearchRequest searchRequest);
        Task<bool> GetHeartbeat();
    }
}
