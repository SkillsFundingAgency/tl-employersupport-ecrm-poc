using System;
using System.Threading.Tasks;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Extensions.Logging;
using Microsoft.Xrm.Sdk;
using tl.employersupport.ecrm.poc.application.Interfaces;
using tl.employersupport.ecrm.poc.application.Model.Ecrm;

namespace tl.employersupport.ecrm.poc.application.ApiClients
{
    public class EcrmXrmClient : IEcrmXrmClient
    {
        private readonly IOrganizationService _organizationService;

        // ReSharper disable once NotAccessedField.Local
        private readonly ILogger _logger;

        //Quick start: https://docs.microsoft.com/en-us/powerapps/developer/data-platform/xrm-tooling/sample-simplified-connection-quick-start

        public EcrmXrmClient(
            IOrganizationService organizationService,
            ILogger<EcrmXrmClient> logger)
        {
            _organizationService = organizationService ?? throw new ArgumentNullException(nameof(organizationService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public Task<Guid> CreateAccount(Account account)
        {
            throw new NotImplementedException();
        }

        public Task<Account> GetAccount(Guid accountId)
        {
            throw new NotImplementedException();
        }

        public Task<Guid> CreateContact(Contact contact)
        {
            throw new NotImplementedException();
        }

        public Task<Contact> GetContact(Guid contactId)
        {
            throw new NotImplementedException();
        }

        public async Task<WhoAmI> WhoAmI()
        {
            return _organizationService.Execute(new WhoAmIRequest()) is WhoAmIResponse response 
                ? new WhoAmI
                {
                    BusinessUnitId = response.BusinessUnitId,
                    OrganizationId = response.OrganizationId,
                    UserId = response.UserId,
                }
                : null;
        }
    }
}
