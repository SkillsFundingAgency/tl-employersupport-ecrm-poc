using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Extensions.Logging;
using Microsoft.PowerPlatform.Dataverse.Client;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
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

        public async Task<Guid> CreateAccount(Account account)
        {
            var entity = new Entity("account")
            {
                ["name"] = account.Name,
                ["address1_primarycontactname"] = account.AddressPrimaryContact,
                ["address1_line1"] = account.AddressLine,
                ["address1_postalcode"] = account.Postcode,
                ["address1_city"] = account.AddressCity,
                //["customertypecode"] = 200008,
                //["customersizecode"] = ,
                ["emailaddress1"] = account.EmailAddress,
                ["telephone1"] = account.Phone
            };

            try
            {
                var newAccountId = _organizationService.Create(entity);
                return newAccountId;
            }
            catch (Exception e)
            {
                throw;
            }
        }

        public async Task<Account> GetAccount(Guid accountId)
        {
            var cols = new ColumnSet(
                new[]
                { 
                    "accountid",
                    "name",
                    "address1_primarycontactname",
                    "address1_line1",
                    "address1_postalcode",
                    "address1_city",
                    "customertypecode",
                    "lsc_noofemployees",
                    "emailaddress1",
                    "telephone1"
                });

            var entity = _organizationService.Retrieve("account", accountId, cols);
            if (entity != null)
            {
                foreach (var attr in entity.Attributes)
                {
                    Debug.WriteLine($"{attr.Key} = {attr.Value}");
                }
            }

            return entity != null ? EntityToAccount(entity) : null;
        }

        public (string displayValue, IList<(int, string)> itemList) GetPicklistMetadata(string entityName, string attributeName)
        {
            if (_organizationService is not ServiceClient serviceClient)
                return (null, null);

            try
            {
                var metaElement = serviceClient.GetPickListElementFromMetadataEntity(entityName, attributeName);
                if (metaElement != null)
                {
                    Debug.WriteLine($"Got picklist {metaElement.DisplayValue}");

                    var itemsList = new List<(int, string)>();
                    foreach (var metaElementItem in metaElement.Items)
                    {
                        itemsList.Add(new(metaElementItem.PickListItemId, metaElementItem.DisplayLabel));
                        Debug.WriteLine($"  {metaElementItem.PickListItemId} ({metaElementItem.DisplayLabel})");
                    }

                    return (metaElement.DisplayValue, itemsList);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to retrieve pick list for {entityName}, {attributeName}");
            }

            return (null, null);
        }

        public async Task UpdateAccountCustomerType(Guid accountId, int customerType)
        {
            try
            {
                if (_organizationService is ServiceClient serviceClient)
                {
                    var customerTypes = GetPicklistMetadata("account", "customertypecode");
                    var customerSizes = GetPicklistMetadata("account", "lsc_noofemployees");
                }

                // Retrieve the account containing several of its attributes.
                var cols = new ColumnSet(
                    new[]
                    {
                        "name",
                        "address1_postalcode",
                        "customertypecode",
                        "versionnumber"
                    });
                var account = _organizationService.Retrieve("account", accountId, cols);

                if (account != null)
                {
                    //retrievedAccount["customertypecode"] = customerType;
                    account["customertypecode"] = new OptionSetValue(customerType);
                    _organizationService.Update(account);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to read and update account {accountId:D}");
                throw;
            }
        }

        public async Task<IEnumerable<Account>> FindDuplicateAccounts(Account account)
        {
            //https://www.inogic.com/blog/2015/11/use-retrieveduplicaterequest-in-dynamics-crm/
            //https://stackoverflow.com/questions/54647279/retrieve-all-duplicate-records-of-any-specific-entity-using-console-application
            //TODO: Create specific entities and extensions to convert or map them
            var entity = AccountToEntity(account);
                //new Entity("account")
                //{
                //    ["name"] = account.Name,
                //};

            var request = new RetrieveDuplicatesRequest
            {
                BusinessEntity = entity,
                MatchingEntityName = entity.LogicalName,
                PagingInfo = new PagingInfo { PageNumber = 1, Count = 50 },
            };

            var response = (RetrieveDuplicatesResponse)_organizationService.Execute(request);

            if (response.DuplicateCollection.Entities.Count >= 1)
            {
                _logger.LogInformation("{0} Duplicate rows found.", response.DuplicateCollection.Entities.Count);
                return response.DuplicateCollection.Entities.Select(EntityToAccount);
            }
            else
            {
                return new List<Account>();

                //https://stackoverflow.com/questions/54647279/retrieve-all-duplicate-records-of-any-specific-entity-using-console-application
                var entityLogicalName = "account";
                var duplicatedColumn = "name";

                var query = new QueryExpression(entityLogicalName)
                {
                    ColumnSet = new ColumnSet(duplicatedColumn)
                };
                query.PageInfo = new PagingInfo
                {
                    PageNumber = 1
                };
                query.AddOrder(duplicatedColumn, 0);

                var results = _organizationService.RetrieveMultiple(query);
                //.GroupBy(e => e.GetAttributeValue<string>(duplicatedColumn), e => e);
                foreach (var r in results.Entities)
                {
                    if (r.Attributes.TryGetValue("name", out var n) && (string)n == account.Name)
                    {
                    }
                }

                //Try with bulk request
                var bulkQuery = new QueryExpression
                {
                    EntityName = "account"
                };

                // Create the request (do not send an e-mail).
                var bulkRequest = new BulkDetectDuplicatesRequest();
                bulkRequest.JobName = "Detect Duplicate Accounts";
                bulkRequest.Query = bulkQuery;
                bulkRequest.RecurrencePattern = string.Empty;
                //bulkRequest.RecurrenceStartTime = new CrmDateTime();
                bulkRequest.RecurrenceStartTime = DateTime.Now;
                bulkRequest.SendEmailNotification = false;
                bulkRequest.ToRecipients = new Guid[0];
                bulkRequest.CCRecipients = new Guid[0];
                bulkRequest.TemplateId = Guid.Empty;

                // Execute the request.
                //var bulkResponse = (BulkDetectDuplicatesResponse)_organizationService.Execute(bulkRequest);
                //Guid jobId = bulkResponse.JobId;
            }
            
            return new List<Account>();
        }

        public async Task<IEnumerable<Contact>> FindDuplicateContacts(Contact contact)
        {
            //https://www.inogic.com/blog/2015/11/use-retrieveduplicaterequest-in-dynamics-crm/
            var entity = new Entity("contact")
            {
                ["firstname"] = contact.FirstName,
                ["lastname"] = contact.LastName,
                ["emailaddress1"] = contact.EmailAddress
            };

            var request = new RetrieveDuplicatesRequest
            {
                BusinessEntity = entity,
                MatchingEntityName = entity.LogicalName,
                PagingInfo = new PagingInfo { PageNumber = 1, Count = 50 }
            };

            var response = (RetrieveDuplicatesResponse)_organizationService.Execute(request);

            if (response.DuplicateCollection.Entities.Count >= 1)
            {
                _logger.LogInformation("{0} Duplicate rows found.", response.DuplicateCollection.Entities.Count);
                return response.DuplicateCollection.Entities.Select(EntityToContact);
            }

            return new List<Contact>();
        }
        public async Task<Guid> CreateContact(Contact contact)
        {
            //https://www.crmug.com/communities/community-home/digestviewer/viewthread?MessageKey=2eb2e7e4-ebe2-4411-b8c5-2cf9bd19728f&CommunityKey=dc83c23b-ede0-4070-ae7a-dd90859148a6&tab=digestviewer

            var contactEntity = new Entity("contact")
            {
                ["firstname"] = contact.FirstName,
                ["lastname"] = contact.LastName,
                ["address1_postalcode"] = contact.Postcode,
                ["address1_line1"] = contact.AddressLine1,
                ["emailaddress1"] = contact.EmailAddress,
                ["telephone1"] = contact.Phone,
                ["parentcustomerid_account@odata.bind"] = $"/accounts({contact.ParentAccountId:D})"
            };

            //contactEntity["new_cpf"] = TextBoxCPF.Text;
            //contactEntity["parentcustomerid"] = contact.;
            //contactEntity["new_cnpj"] = contact.;
            //contactEntity["mobilephone"] = contact.;
            //    $parentAccount = new- crmentityreference - EntityLogicalName account - Id "ECA74DB9-795F-E711-80F8-70106FAAEAD1"
            //Set - CrmRecord - conn $conn - CrmRecord $contact - Fields @{ "parentcustomerid" = $parentAccount}

            try
            {
                var newContactId = _organizationService.Create(contactEntity);
                return newContactId;
            }
            catch (Exception e)
            {
                throw;
            }
        }

        public Task<Contact> GetContact(Guid contactId)
        {
            throw new NotImplementedException();
        }

        public async Task<Guid> CreateNote(Note note)
        {
            var entity = new Entity("annotation")
            {
                ["subject"] = note.Subject,
                ["notetext"] = note.NoteText,
                //["filename"] = note.,
                ["isdocument"] = true,
                //["documentbody"] = note.,
                //["mimetype"] = note.MimeType, //"application/pdf"
                //["objectid_account@odata.bind"] = "/accounts()",
                ["objectid_account@odata.bind"] = $"/accounts({note.ParentAccountId:D})"
                //note["objectid_account@odata.bind"] = "/accounts(C5DDA727-B375-E611-80C8-00155D00083F)";
            };

            try
            {
                //Xrm.WebApi.online.createRecord("annotation", entity).then(
                var newNoteId = _organizationService.Create(entity);
                return newNoteId;
            }
            catch (Exception e)
            {
                throw;
            }
        }

        public Task<Contact> GetNotes(Guid accountId)
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

        private static Entity AccountToEntity(Account account) =>
            new("account")
            {
                //ignoring accountid for now
                ["name"] = account.Name,
                ["address1_primarycontactname"] = account.AddressPrimaryContact,
                ["address1_line1"] = account.AddressLine,
                ["address1_postalcode"] = account.Postcode,
                ["address1_city"] = account.AddressCity,
                ["customertypecode"] = account.CustomerTypeCode.HasValue 
                    ? new OptionSetValue(account.CustomerTypeCode.Value)
                    : null,
                ["lsc_noofemployees"] = account.NumberOfEmployees.HasValue
                    ? new OptionSetValue(account.NumberOfEmployees.Value)
                    : null,
                ["emailaddress1"] = account.EmailAddress,
                ["telephone1"] = account.Phone
            };

        private static Account EntityToAccount(Entity entity) =>
            new()
            {
                AccountId = (Guid)entity["accountid"],
                Name = entity["name"] as string,
                AddressPrimaryContact = entity["address1_primarycontactname"] as string,
                AddressLine = entity["address1_line1"] as string,
                Postcode = entity["address1_postalcode"] as string,
                AddressCity = entity["address1_city"] as string,
                CustomerTypeCode = ((OptionSetValue)entity["customertypecode"])?.Value,
                NumberOfEmployees = ((OptionSetValue)entity["lsc_noofemployees"])?.Value,
                EmailAddress = entity["emailaddress1"] as string,
                Phone = entity["telephone1"] as string
            };

        private static Contact EntityToContact(Entity entity) =>
            new()
            {
                FirstName = entity["firstname"] as string,
                LastName = entity["lastname"] as string,
                AddressLine1 = entity["address1_line1"] as string,
                Postcode = entity["address1_postalcode"] as string,
                EmailAddress = entity["emailaddress1"] as string,
                Phone = entity["telephone1"] as string
            };
    }
}
