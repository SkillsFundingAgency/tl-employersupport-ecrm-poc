using System;
using System.Threading.Tasks;
using tl.employersupport.ecrm.poc.application.Model.Ecrm;

namespace tl.employersupport.ecrm.poc.application.Interfaces
{
    public interface IEcrmXrmClient
    {
        Task<Account> CheckForDuplicateAccount(Account account);
        Task<Guid> CreateAccount(Account account);
        Task<Account> GetAccount(Guid accountId);
        Task UpdateAccountCustomerType(Guid accountId, int customerType);

        Task<Guid> CreateContact(Contact contact);
        Task<Contact> GetContact(Guid contactId);

        Task<Guid> CreateNote(Note note);
        Task<Contact> GetNotes(Guid accountId);

        Task<WhoAmI> WhoAmI();
    }
}
