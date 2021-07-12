using System;
using System.Threading.Tasks;
using tl.employersupport.ecrm.poc.application.Model.Ecrm;

namespace tl.employersupport.ecrm.poc.application.Interfaces
{
    public interface IEcrmODataApiClient
    {
        Task<Guid> CreateAccount(Account account);
        Task<WhoAmI> GetWhoAmI();
        Task<Account> GetAccount(Guid accountId);
        Task<Guid> CreateContact(Contact contact);
        Task<Guid> CreateNote(Note note);
    }
}
