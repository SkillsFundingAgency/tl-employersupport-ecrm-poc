using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using tl.employersupport.ecrm.poc.application.Model.Ecrm;

namespace tl.employersupport.ecrm.poc.application.Interfaces
{
    public interface IEcrmXrmClient
    {
        Task<IEnumerable<Account>> FindDuplicateAccounts(Account account);
        Task<IEnumerable<Contact>> FindDuplicateContacts(Contact contact);
        
        Task<Guid> CreateAccount(Account account);
        Task<Account> GetAccount(Guid accountId);
        Task UpdateAccountCustomerType(Guid accountId, int customerType);

        Task<Guid> CreateContact(Contact contact);
        Task<Contact> GetContact(Guid contactId);

        Task<Guid> CreateNote(Note note);
        Task<Contact> GetNotes(Guid accountId);

        (string displayValue, IList<(int, string)> itemList) GetPicklistMetadata(
            string entityName, string attributeName);

        Task<WhoAmI> WhoAmI();
    }
}
