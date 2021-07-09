using System;
using System.Threading.Tasks;
using tl.employersupport.ecrm.poc.application.Model.Ecrm;

namespace tl.employersupport.ecrm.poc.application.Interfaces
{
    public interface IEcrmXrmClient
    {
        Task<Guid> CreateAccount(Account account);
        Task<Account> GetAccount(Guid accountId);

        Task<Guid> CreateContact(Contact contact);
        Task<Contact> GetContact(Guid contactId);
        
        Task<WhoAmI> WhoAmI();
    }
}
