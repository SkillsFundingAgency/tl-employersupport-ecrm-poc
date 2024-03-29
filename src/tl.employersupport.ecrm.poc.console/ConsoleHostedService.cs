﻿using System;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using tl.employersupport.ecrm.poc.application.Extensions;
using tl.employersupport.ecrm.poc.application.Interfaces;
using tl.employersupport.ecrm.poc.application.Model.Ecrm;
using tl.employersupport.ecrm.poc.application.Model.ZendeskTicket;

namespace tl.employersupport.ecrm.poc.console
{
    internal sealed class ConsoleHostedService : IHostedService
    {
        private readonly ILogger _logger;
        private readonly IHostApplicationLifetime _appLifetime;
        private readonly IMonitorService _functionsApiService;
        private readonly IEmailService _emailService;
        private readonly ITicketService _ticketService;
        private readonly IEcrmService _ecrmService;

        private TicketFieldCollection _ticketFields;

        private int? _exitCode;

        public ConsoleHostedService(
            ILogger<ConsoleHostedService> logger,
            IHostApplicationLifetime appLifetime,
            IEmailService emailService,
            ITicketService ticketService,
            IMonitorService functionsApiService,
            IEcrmService ecrmService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _appLifetime = appLifetime ?? throw new ArgumentNullException(nameof(appLifetime));
            _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
            _functionsApiService = functionsApiService ?? throw new ArgumentNullException(nameof(functionsApiService));
            _ticketService = ticketService ?? throw new ArgumentNullException(nameof(ticketService));
            _ecrmService = ecrmService ?? throw new ArgumentNullException(nameof(ecrmService));
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            var args = Environment.GetCommandLineArgs();
            _logger.LogDebug($"Starting with arguments: {string.Join(" ", args[1..])}");

            _appLifetime.ApplicationStarted.Register(() =>
            {
                Task.Run(async () =>
                {
                    try
                    {
                        if (args.HasArgument("--help"))
                        {
                            WriteHelpText();
                        }

                        var ticketId = args.GetLongFromArgument("ticketId", ":");

                        if (!Guid.TryParse(
                            args.GetStringFromArgument("ecrmAccountId", ":"),
                            out var ecrmAccountId))
                        {
                            Console.WriteLine("Missing or invalid ECRM Account Id");
                        }

                        CombinedTicket ticket = null;

                        var pause = args.HasArgument("--pause");

                        if (args.HasArgument("--ecrmHeartbeat"))
                        {
                            await CheckEcrmHeartBeat();
                            if (pause) WaitForUserInput();
                        }

                        if (args.HasArgument("--ecrmWhoAmI"))
                        {
                            await CheckEcrmWhoAmI();
                            if (pause) WaitForUserInput();
                        }

                        if (args.HasArgument("--ecrmGetAccount"))
                        {
                            await GetEcrmAccount(ecrmAccountId);
                            if (pause) WaitForUserInput();
                        }

                        if (args.HasArgument("--ecrmFindDuplicates"))
                        {
                            await FindDuplicates();
                            if (pause) WaitForUserInput();
                        }

                        if (args.HasArgument("--ecrmCreateAccount"))
                        {
                            ecrmAccountId = await CreateEcrmAccount();
                            if (pause) WaitForUserInput();
                        }

                        if (args.HasArgument("--ecrmUpdateCustomerType"))
                        {
                            //parse ecrmCustomerType
                            var ecrmCustomerType = args.GetIntFromArgument("customerType", ":");
                            if (ecrmCustomerType > 0)
                            {
                                await UpdateEcrmAccountCustomerType(ecrmAccountId, ecrmCustomerType);
                            }
                        }

                        if (args.HasArgument("--ecrmCreateContact"))
                        {
                            //TODO: If account created with id in previous step, use that
                            await CreateEcrmContact(ecrmAccountId);
                            if (pause) WaitForUserInput();
                        }

                        if (args.HasArgument("--ecrmCreateNote"))
                        {
                            //TODO: If account created with id in previous step, use that
                            await CreateEcrmNote(ecrmAccountId);
                            if (pause) WaitForUserInput();
                        }

                        if (args.HasArgument("--sendTicketCreatedEmail"))
                        {
                            await SendTicketCreatedEmail();
                            if (pause) WaitForUserInput();
                        }

                        if (args.HasArgument("--pollyTest"))
                        {
                            var minTimeout = args.GetIntFromArgument("minTimeout", ":");
                            var maxTimeout = args.GetIntFromArgument("maxTimeout", ":", 300);
                            var clientTimeout = args.GetIntFromArgument("clientTimeout", ":", 60);
                            await TestPollyPolicy(clientTimeout, minTimeout, maxTimeout);
                            if (pause) WaitForUserInput();
                        }

                        if (args.HasArgument("--getTicketFields"))
                        {
                            //save in member variable to simulate cache
                            _ticketFields = await GetTicketFieldsFromZendesk();

                            if (pause) WaitForUserInput();
                        }

                        if (args.HasArgument("--searchTickets"))
                        {
                            await SearchTicketsInZendesk();
                            if (pause) WaitForUserInput();
                        }

                        if (args.HasArgument("--getTicket") && ticketId > 0)
                        {
                            ticket = await GetTicketFromZendesk(ticketId);

                            _ticketFields ??= await GetTicketFieldsFromZendesk();

                            LogTicketDetails(ticket, _ticketFields);

                            if (pause) WaitForUserInput();
                        }

                        if (args.HasArgument("--getContactTicket") && ticketId > 0)
                        {
                            var employerContactTicket = await GetEmployerContactTicketFromZendesk(ticketId);

                            LogEmployerContactTicketDetails(employerContactTicket);

                            if (pause) WaitForUserInput();
                        }

                        if (args.HasArgument("--updateTicket")
                            && ticketId > 0
                            && ticket is not null)
                        {
                            await UpdateTicketInZendesk(ticketId, ticket);
                            if (pause) WaitForUserInput();
                        }

                        _exitCode = 0;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Unhandled exception!");
                        _exitCode = 1;
                    }
                    finally
                    {
                        // Stop the application once the work is done
                        _appLifetime.StopApplication();
                    }
                }, cancellationToken);
            });

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogDebug($"Exiting with return code: {_exitCode}");
            // Exit code may be null if the user cancelled via Ctrl+C/SIGTERM
            Environment.ExitCode = _exitCode.GetValueOrDefault(-1);
            return Task.CompletedTask;
        }

        private async Task<Guid> CreateEcrmAccount()
        {
            var account = BuildTestAccount();

            var newId = await _ecrmService.CreateAccount(account);

            _logger.LogInformation($"ECRM create account result: {newId:D}");

            return newId;
        }

        private async Task UpdateEcrmAccountCustomerType(Guid accountId, int customerType)
        {
            await _ecrmService.UpdateAccountCustomerType(accountId, customerType);
        }

        private async Task<Guid> CreateEcrmContact(Guid accountId)
        {
            var contact = new Contact
            {
                ParentAccountId = accountId,
                CustomerTypeCode = 1,
                FirstName = "Mikey",
                LastName = "Mike",
                AddressLine1 = "156 Lime Road",
                Postcode = "OX2 8EY",
                City = "Oxford",
                EmailAddress = "mikey.mike@mike.test.com",
                Phone = "0771234568"
            };

            var newId = await _ecrmService.CreateContact(contact);

            _logger.LogInformation($"ECRM create contact result: {newId:D}");

            return newId;
        }

        private async Task<Guid> CreateEcrmNote(Guid accountId)
        {
            var note = new Note
            {
                ParentAccountId = accountId,
                Subject = "Test note",
                NoteText = "Hello notes world"
            };

            var newId = await _ecrmService.CreateNote(note);

            _logger.LogInformation($"ECRM create note result: {newId:D}");

            return newId;
        }
        private async Task GetEcrmAccount(Guid accountId)
        {
            var account = await _ecrmService.GetAccount(accountId);
            if (account is null)
            {
                _logger.LogInformation($"ECRM Account {accountId} not found");
            }
            else
            {
                _logger.LogInformation("ECRM Account:");
                var accountInfoBuilder = new StringBuilder();
                accountInfoBuilder.AppendLine($"    Account id:      {account.AccountId}");
                accountInfoBuilder.AppendLine($"    Name:            {account.Name}");
                accountInfoBuilder.AppendLine($"    Address line 1:  { account.AddressLine}");
                accountInfoBuilder.AppendLine($"    Postcode:        {account.Postcode}");
                accountInfoBuilder.AppendLine($"    City:            {account.AddressCity}");
                accountInfoBuilder.AppendLine($"    Primary Contact: {account.AddressPrimaryContact}");
                accountInfoBuilder.AppendLine($"    EmailAddress:    {account.EmailAddress}");
                accountInfoBuilder.AppendLine($"    Phone:           {account.Phone}");
                _logger.LogInformation(accountInfoBuilder.ToString());
            }
        }

        private async Task FindDuplicates()
        {
            var account = BuildTestAccount();
            var contact = BuildTestContact();

            var duplicateAccounts = 
                await _ecrmService.FindDuplicateAccounts(account);

            _logger.LogInformation($"Found {duplicateAccounts?.Count()} duplicate accounts");

            var duplicateContacts =
                await _ecrmService.FindDuplicateContacts(contact);

            _logger.LogInformation($"Found {duplicateContacts?.Count()} duplicate contacts");
        }

        private async Task CheckEcrmHeartBeat()
        {
            var heartbeat = await _ecrmService.Heartbeat();
            _logger.LogInformation($"ECRM Heartbeat result: {(heartbeat ? "Alive" : "Goner")}");
        }

        private async Task CheckEcrmWhoAmI()
        {
            var whoAmI = await _ecrmService.WhoAmI();
            if (whoAmI is null)
            {
                _logger.LogInformation("ECRM WhoAmI returned null");
            }
            else
            {
                _logger.LogInformation("ECRM WhoAmI:");
                var whoAmIBuilder = new StringBuilder();
                whoAmIBuilder.AppendLine($"    Business unit: {whoAmI.BusinessUnitId}");
                whoAmIBuilder.AppendLine($"    Organisation:  {whoAmI.UserId}");
                whoAmIBuilder.AppendLine($"    User:          {whoAmI.OrganizationId}");
                _logger.LogInformation(whoAmIBuilder.ToString());
            }
        }

        private Account BuildTestAccount() => 
            new()
            {
                //AccountId = Guid.NewGuid(),
                Name = "T Levels Test",
                AddressLine = "155 Lime Road",
                Postcode = "OX2 9EG",
                AddressCity = "Oxford",
                EmailAddress = "mike@mike.test.com",
                AddressPrimaryContact = "Mike Wild",
                Phone = "0771234567",
                CustomerTypeCode = 200008, //new CustomerTypeCode {Value = 200008 },
                NumberOfEmployees = 7
            };

        private Contact BuildTestContact() =>
            new()
            {
                FirstName = "Mikey",
                LastName = "Mike",
                AddressLine1 = "155 Lime Road",
                Postcode = "OX2 9EG",
                EmailAddress = "mikey.mike@mike.test.com",
                Phone = "0771234568"
            };

        private async Task SendTicketCreatedEmail()
        {
            var random = new Random();
            var ticketId = random.Next();
            var sent = await _emailService
                .SendZendeskTicketCreatedEmail(
                    ticketId,
                    DateTime.UtcNow);
            _logger.LogInformation($"Email sent - {sent}");
        }

        private async Task TestPollyPolicy(int clientTimeout, int minTimeout, int maxTimeout)
        {
            await _functionsApiService
                .CallTestTimeout(clientTimeout, minTimeout, maxTimeout);
        }

        private async Task<TicketFieldCollection> GetTicketFieldsFromZendesk()
        {
            var ticketFields = await _ticketService.GetTicketFields();

            var ticketFieldList = new StringBuilder();
            ticketFieldList.AppendLine($"Fields: ({ticketFields.Count})");
            foreach (var (key, value) in ticketFields)
            {
                ticketFieldList.AppendLine($"    {key}: '{value.Title}' - '{value.Type}' - active:{value.Active}");
            }
            ticketFieldList.AppendLine("");
            _logger.LogInformation(ticketFieldList.ToString());

            ticketFieldList.Clear();
            ticketFieldList.AppendLine($"T Level Fields: ({ticketFields.Count})");
            foreach (var (key, value) in ticketFields.Where(t => t.Value.Title.StartsWith("T Level")))
            {
                ticketFieldList.AppendLine($"    {key}: '{value.Title}' - '{value.Type}' - active:{value.Active}");
            }
            ticketFieldList.AppendLine("");
            _logger.LogInformation(ticketFieldList.ToString());

            return ticketFields;
        }

        private async Task<CombinedTicket> GetTicketFromZendesk(long ticketId)
        {
            _logger.LogInformation("Getting ticket...");

            var ticket = await _ticketService.GetTicket(ticketId);

            return ticket;
        }

        private async Task<EmployerContactTicket> GetEmployerContactTicketFromZendesk(long ticketId)
        {
            _logger.LogInformation("Getting ticket...");

            var ticket = await _ticketService.GetEmployerContactTicket(ticketId);

            return ticket;
        }

        private void LogTicketDetails(CombinedTicket ticket, TicketFieldCollection ticketFields)
        {
            if (ticket is null)
                return;

            var ticketDetail = new StringBuilder();

            ticketDetail.AppendLine($"Id:      {ticket.Id}");
            if (ticket.Ticket != null)
            {
                ticketDetail.AppendLine($"Id:      {ticket.Ticket.Description}");
                ticketDetail.AppendLine($"Tags: ({ticket.Ticket.Tags.Length})");
                foreach (var tag in ticket.Ticket.Tags)
                {
                    ticketDetail.AppendLine($"         {tag}");
                }
                ticketDetail.AppendLine("");

                if (ticketFields != null && ticketFields.Any())
                {
                    ticketDetail.AppendLine("Ticket fields: ({ticket.Ticket.Fields.Count()})");

                    //foreach (var (key, value) in ticketFields)
                    //foreach (var field in ticket.Ticket.Fields)
                    //{
                    //    var fieldFound = ticketFields.TryGetValue(field.Id, out var fieldInfo);
                    //    if (fieldFound && fieldInfo.Title.StartsWith("T Level"))
                    //    {
                    //        ticketDetail.AppendLine(
                    //            $"         {fieldInfo.Title} - '{field.Value}' [{fieldInfo.Type} - {fieldInfo.Active}]");
                    //    }
                    //        //ticketDetail.AppendLine(fieldFound
                    //        //? $"         {fieldInfo.Title} - '{field.Value}' [{fieldInfo.Type} - {fieldInfo.Active}]"
                    //        //: $"         {field.Id} - {field.Value}");
                    //}
                    //ticketDetail.AppendLine("");

                    ticketDetail.AppendLine("Ticket custom fields: ({ticket.Ticket.CustomFields.Count()})");
                    foreach (var field in ticket.Ticket.CustomFields)
                    {
                        var fieldFound = ticketFields.TryGetValue(field.Id, out var fieldInfo);
                        if (fieldFound && fieldInfo.Title.StartsWith("T Level"))
                        {
                            ticketDetail.AppendLine(
                                $"         {fieldInfo.Title} - '{field.Value}' [{fieldInfo.Type} - {fieldInfo.Active}]");
                        }
                        //ticketDetail.AppendLine(ticketFields.TryGetValue(field.Id, out var fieldInfo)
                        //    ? $"         {fieldInfo.Title} - '{field.Value}' [{fieldInfo.Type} - {fieldInfo.Active}]"
                        //    : $"         {field.Id} - {field.Value}");
                    }
                    ticketDetail.AppendLine("");
                }

                ticketDetail.AppendLine($"Users ({ticket.Users.Count()}):");
                foreach (var user in ticket.Users)
                {
                    ticketDetail.AppendLine($"         {user.Email}");
                    if (user.UserFields?.AddressLine1 is not null)
                        ticketDetail.AppendLine($"         {user.UserFields?.AddressLine1}");
                    if (user.UserFields?.AddressLine2 is not null)
                        ticketDetail.AppendLine($"         {user.UserFields?.AddressLine2}");
                    if (user.UserFields?.AddressLine3 is not null)
                        ticketDetail.AppendLine($"         {user.UserFields?.AddressLine3}");
                    if (user.UserFields?.City is not null)
                        ticketDetail.AppendLine($"         {user.UserFields?.City}");
                    if (user.UserFields?.Postcode is not null)
                        ticketDetail.AppendLine($"         {user.UserFields?.Postcode}");
                    ticketDetail.AppendLine("");
                }

                ticketDetail.AppendLine($"Organizations ({ticket.Organizations.Count()}):");
                foreach (var organization in ticket.Organizations)
                {
                    ticketDetail.AppendLine($"         {organization.Id}");
                    ticketDetail.AppendLine($"         {organization.Name}");
                    if (organization.OrganizationFields?.AddressLine1 is not null)
                        ticketDetail.AppendLine($"         {organization.OrganizationFields?.AddressLine1}");
                    if (organization.OrganizationFields?.AddressLine2 is not null)
                        ticketDetail.AppendLine($"         {organization.OrganizationFields?.AddressLine2}");
                    if (organization.OrganizationFields?.AddressLine3 is not null)
                        ticketDetail.AppendLine($"         {organization.OrganizationFields?.AddressLine3}");
                    if (organization.OrganizationFields?.City is not null)
                        ticketDetail.AppendLine($"         {organization.OrganizationFields?.City}");
                    if (organization.OrganizationFields?.Postcode is not null)
                        ticketDetail.AppendLine($"         {organization.OrganizationFields?.Postcode}");
                    ticketDetail.AppendLine("");
                }
            }

            _logger.LogInformation($"Retrieved ticket {ticket.Id}");
            _logger.LogInformation(ticketDetail.ToString());
        }

        private void LogEmployerContactTicketDetails(EmployerContactTicket ticket)
        {
            if (ticket is null)
                return;

            var ticketDetail = new StringBuilder();

            ticketDetail.AppendLine($"Id:      {ticket.Id}");

            ticketDetail.AppendLine($"Created: ({ticket.CreatedAt:yyyy-MM-ddTHH:mm:ssZ})");
            ticketDetail.AppendLine($"Updated: ({ticket.UpdatedAt:yyyy-MM-ddTHH:mm:ssZ})");

            ticketDetail.AppendLine($"Tags: ({ticket.Tags.Count})");
            foreach (var tag in ticket.Tags)
            {
                ticketDetail.AppendLine($"         {tag}");
            }
            ticketDetail.AppendLine("");

            _logger.LogInformation($"Retrieved ticket {ticket.Id}");
            _logger.LogInformation(ticketDetail.ToString());
        }

        private async Task SearchTicketsInZendesk()
        {
            _logger.LogInformation("Searching for tickets...");

            var ticketSearchResults = await _ticketService.SearchTickets();

            _logger.LogInformation($"Found {ticketSearchResults.Count} tickets");
            if (ticketSearchResults.Any())
            {
                var ticketList = new StringBuilder();
                foreach (var ticket in ticketSearchResults)
                {
                    ticketList.AppendLine($"    {ticket.Id}");
                    ticketList.AppendLine($"    {ticket.Subject}");
                }

                _logger.LogInformation(ticketList.ToString());
            }
        }

        private async Task UpdateTicketInZendesk(long ticketId, CombinedTicket ticket)
        {
            if (ticket is null)
                return;

            _logger.LogInformation("Adding tag...");
            //var tag = $"monitor_update_{DateTime.UtcNow:s}";
            var tag = $"monitor_updated_{ticket.Ticket.Tags.Count(t => t.StartsWith("monitor_updated"))}";
            await _ticketService.AddTag(ticketId, ticket, tag);
            _logger.LogInformation($"Added tag {tag}");
        }

        private static void WaitForUserInput()
        {
            Console.WriteLine("...");
            Console.ReadKey();
        }

        private static void WriteHelpText()
        {
            Console.WriteLine("Zendesk - ECRM Demo");
            Console.WriteLine("  --ecrmHeartbeat          - test ECRM API heartbeat");
            Console.WriteLine("  --ecrmWhoAmI             - test ECRM API WhoAmI");
            Console.WriteLine("  --ecrmFindDuplicates     - test ECRM API duplicate account query");
            Console.WriteLine("  --ecrmGetAccount         - test ECRM API account query (requires accountId)");
            Console.WriteLine("  --ecrmCreateAccount      - test ECRM API account creation");
            Console.WriteLine("  --ecrmCreateContact      - test ECRM API contact creation (requires accountId)");
            Console.WriteLine("  --ecrmCreateNote         - test ECRM API contact creation (requires accountId)");
            Console.WriteLine("  --ecrmUpdateCustomerType - test ECRM API account type update (requires accountId and customer type)");
            Console.WriteLine("  --getTicket              - get a ticket from Zendesk (requires ticketId)");
            Console.WriteLine("  --getTicketFields        - get all ticket fields (requires ticketId)");
            Console.WriteLine("  --getContactTicket       - get a ticket with contact details from Zendesk (requires ticketId)");
            Console.WriteLine("  --searchTickets          - looks for recently created tickets");
            Console.WriteLine("  --updateTicket           - update the ticket tags in Zendesk");
            Console.WriteLine("  --sendTicketCreatedEmail - send a test email");
            Console.WriteLine("  --pollyTest              - test the Polly timeout policy");
            Console.WriteLine("  --pause                  - pauses between steps");
            Console.WriteLine("  --help                   - shows this help text");
            Console.WriteLine("  ticketId:n               - ticket id for steps that use it");
            Console.WriteLine("  ecrmAccountId:n          - account id in ECRM (guid)");
            Console.WriteLine("  ecrmCustomerType:n          - customer type code for ECRM (int)");
            Console.WriteLine("  clientTimeout:n          - the time to HTTP client timeout in seconds; used by --pollyTest");
            Console.WriteLine("  minTimeout:n             - minimum function timeout in seconds; used by --pollyTest");
            Console.WriteLine("  maxTimeout:n             - maximum function timeout in seconds; used by --pollyTest");
        }
    }
}
