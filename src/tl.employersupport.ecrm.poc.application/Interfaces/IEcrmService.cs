using System;
using System.Threading.Tasks;
using tl.employersupport.ecrm.poc.application.Model.Ecrm;

namespace tl.employersupport.ecrm.poc.application.Interfaces
{
    public interface IEcrmService
    {
        Task<Employer> FindEmployer(EmployerSearchRequest searchRequest);
        Task<bool> Heartbeat();
        Task<WhoAmIResponse> WhoAmI();
        Task<Account> GetAccount(Guid accountId);
    }
}
