using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using tl.employersupport.ecrm.poc.application.Interfaces;
using tl.employersupport.ecrm.poc.application.Model.Configuration;
using tl.employersupport.ecrm.poc.application.Model.Ecrm;

namespace tl.employersupport.ecrm.poc.application.Services
{
    public class EcrmService : IEcrmService
    {
        private readonly ILogger _logger;
        private readonly IEcrmApiClient _ecrmApiClient;
        private readonly IEcrmODataApiClient _ecrmODataApiClient;
        private readonly IEcrmXrmClient _ecrmXrmClient;

        public EcrmService(
            IEcrmApiClient ecrmApiClient,
            IEcrmODataApiClient ecrmODataApiClient,
            IEcrmXrmClient ecrmXrmClient,
            ILogger<EcrmService> logger)
        {
            _ecrmApiClient = ecrmApiClient ?? throw new ArgumentNullException(nameof(ecrmApiClient));
            _ecrmODataApiClient = ecrmODataApiClient ?? throw new ArgumentNullException(nameof(ecrmODataApiClient));
            _ecrmXrmClient = ecrmXrmClient ?? throw new ArgumentNullException(nameof(ecrmXrmClient));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<Employer> FindEmployer(EmployerSearchRequest searchRequest)
        {
            _logger.LogInformation($"Getting employer {searchRequest.CompanyName}");

            var employer = await _ecrmApiClient.GetEmployer(searchRequest);

            return employer;
        }

        public async Task<bool> Heartbeat()
        {
            return await _ecrmApiClient.GetHeartbeat();
        }

        public async Task<WhoAmI> WhoAmI()
        {
            //var result = await _ecrmODataApiClient.GetWhoAmI();
            var result = await _ecrmXrmClient.WhoAmI();
            return result;
        }

        public async Task<Guid> CreateAccount(Account account)
        {
            return await _ecrmODataApiClient.CreateAccount(account);
        }

        public async Task<Guid> CreateContact(Contact contact)
        {
            return await _ecrmXrmClient.CreateContact(contact);
        }

        public async Task<Guid> CreateNote(Note note)
        {
            return await _ecrmXrmClient.CreateNote(note);
        }

        public async Task UpdateAccountCustomerType(Guid accountId, int customerType)
        {
            await _ecrmXrmClient.UpdateAccountCustomerType(accountId, customerType);
        }

        public async Task<IEnumerable<Account>> FindDuplicateAccounts(Account account)
        {
            return await _ecrmXrmClient.FindDuplicateAccounts(account);
        }

        public async Task<IEnumerable<Contact>> FindDuplicateContacts(Contact contact)
        {
            return await _ecrmXrmClient.FindDuplicateContacts(contact);
        }
        
        public async Task<Account> GetAccount(Guid accountId)
        {
            //var testAccount =  await _ecrmApiClient.GetAccount(accountId);
            //var testAccount2 =  await _ecrmODataApiClient.GetAccount(accountId);
            var account =  await _ecrmXrmClient.GetAccount(accountId);
            return account;
        }
    }
}
