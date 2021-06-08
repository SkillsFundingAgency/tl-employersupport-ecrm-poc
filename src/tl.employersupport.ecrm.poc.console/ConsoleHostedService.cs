using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using tl.employersupport.ecrm.poc.application.Extensions;
using tl.employersupport.ecrm.poc.application.Interfaces;

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
            _logger.LogDebug($"Starting with arguments: {string.Join(" ", args)}");

            _appLifetime.ApplicationStarted.Register(() =>
            {
                Task.Run(async () =>
                {
                    try
                    {
                        _logger.LogInformation("Getting ticket...");

                        var ticketId = args.GetIntFromArgument("ticketId", 1);
                        var ticket = await _ticketService.GetTicket(ticketId);

                        if (ticket is not null)
                        {
                            _logger.LogInformation($"Retrieved ticket {ticket.Id}");
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
    }
}
