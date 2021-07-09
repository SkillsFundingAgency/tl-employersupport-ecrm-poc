using System;
using System.Threading.Tasks;
using tl.employersupport.ecrm.poc.application.Model.Ecrm;

namespace tl.employersupport.ecrm.poc.application.Interfaces
{
    public interface IEcrmODataApiClient
    {
        Task<Guid> CreateAccount(Account account);
        Task<WhoAmIResponse> GetWhoAmI();
        Task<Account> GetAccount(Guid accountId);
        Task<Guid> CreateContact(Contact contact);
    }
}
