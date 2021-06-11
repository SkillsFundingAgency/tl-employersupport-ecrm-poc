using System;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using tl.employersupport.ecrm.poc.application.Extensions;
using tl.employersupport.ecrm.poc.application.Interfaces;
using tl.employersupport.ecrm.poc.application.Model;

namespace tl.employersupport.ecrm.poc.console
{
    internal sealed class ConsoleHostedService : IHostedService
    {
        private readonly ILogger _logger;
        private readonly IHostApplicationLifetime _appLifetime;
        private readonly ITicketService _ticketService;
        private int? _exitCode;

        public ConsoleHostedService(
            ILogger<ConsoleHostedService> logger,
            IHostApplicationLifetime appLifetime,
            ITicketService ticketService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _appLifetime = appLifetime ?? throw new ArgumentNullException(nameof(appLifetime));
            _ticketService = ticketService ?? throw new ArgumentNullException(nameof(ticketService));
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
                        var ticketId = args.GetLongFromArgument("ticketId", ":");
                        CombinedTicket ticket = null;
                        if (args.HasArgument("--getTicket"))
                        {
                            ticket = await GetTicketFromZendesk(ticketId);
                            LogTicketDetails(ticket);
                        }

                        if (args.HasArgument("--updateTicket"))
                        {
                            await UpdateTicketInZendesk(ticketId, ticket);
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

        private async Task<CombinedTicket> GetTicketFromZendesk(long ticketId)
        {
            _logger.LogInformation("Getting ticket...");

            var ticket = await _ticketService.GetTicket(ticketId);

            return ticket;
        }

        private void LogTicketDetails(CombinedTicket ticket)
        {
            if (ticket is null)
                return;

            var ticketDetail = new StringBuilder();

            ticketDetail.AppendLine($"Id:      {ticket.Id}");
            if (ticket.Ticket != null)
            {
                ticketDetail.AppendLine($"Id:      {ticket.Ticket.Description}");
                ticketDetail.AppendLine($"Tags:");
                foreach (var tag in ticket.Ticket.Tags)
                {
                    ticketDetail.AppendLine($"         {tag}");
                }
                ticketDetail.AppendLine("");

                ticketDetail.AppendLine($"Users ({ticket.Users.Count()}):");
                foreach (var user in ticket.Users)
                {
                    ticketDetail.AppendLine($"         {user.Email}");
                    ticketDetail.AppendLine($"         {user.UserFields?.AddressLine1}");
                    ticketDetail.AppendLine($"         {user.UserFields?.AddressLine2}");
                    ticketDetail.AppendLine($"         {user.UserFields?.AddressLine3}");
                    ticketDetail.AppendLine($"         {user.UserFields?.City}");
                    ticketDetail.AppendLine($"         {user.UserFields?.Postcode}");
                    ticketDetail.AppendLine("");
                }

                ticketDetail.AppendLine($"Organizations ({ticket.Organizations.Count()}):");
                foreach (var organization in ticket.Organizations)
                {
                    ticketDetail.AppendLine($"         {organization.Id}");
                    ticketDetail.AppendLine($"         {organization.Name}");
                    ticketDetail.AppendLine($"         {organization.OrganizationFields?.AddressLine1}");
                    ticketDetail.AppendLine($"         {organization.OrganizationFields?.AddressLine2}");
                    ticketDetail.AppendLine($"         {organization.OrganizationFields?.AddressLine3}");
                    ticketDetail.AppendLine($"         {organization.OrganizationFields?.City}");
                    ticketDetail.AppendLine($"         {organization.OrganizationFields?.Postcode}");
                    ticketDetail.AppendLine("");
                }
            }

            _logger.LogInformation($"Retrieved ticket {ticket.Id}");
            _logger.LogInformation(ticketDetail.ToString());
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
    }
}
