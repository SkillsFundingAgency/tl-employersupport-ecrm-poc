using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Notify.Interfaces;
using tl.employersupport.ecrm.poc.application.Interfaces;
using tl.employersupport.ecrm.poc.application.Model.Configuration;

namespace tl.employersupport.ecrm.poc.application.Services
{
    public class EmailService : IEmailService
    {
        private readonly EmailConfiguration _configuration;
        private readonly ILogger<EmailService> _logger;
        private readonly IAsyncNotificationClient _notificationClient;

        public EmailService(IOptions<EmailConfiguration> emailConfiguration,
            IAsyncNotificationClient notificationClient,
            ILogger<EmailService> logger)
        {
            _notificationClient = notificationClient ?? throw new ArgumentNullException(nameof(notificationClient));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            _configuration = emailConfiguration?.Value ??
                             throw new ArgumentNullException(
                                 $"{nameof(emailConfiguration)}",
                                 "Configuration or configuration value must not be null");
        }

        public async Task<bool> SendZendeskTicketCreatedEmail(
            long ticketId,
            DateTime createdAt)
        {
            var toAddresses = _configuration.SupportEmailAddress?.Split(';', StringSplitOptions.RemoveEmptyEntries);

            if (toAddresses == null || !toAddresses.Any())
            {
                _logger.LogError("There are no support email addresses defined.");
                return false;
            }

            var tokens = new Dictionary<string, dynamic>
            {
                { "ticket_id", ticketId },
                { "created_at", createdAt.ToString("dd MMM yyyy hh:mm:ss") }
            };

            var allEmailsSent = true;
            foreach (var toAddress in toAddresses)
            {
                allEmailsSent &= await SendEmail(toAddress,
                    _configuration.ZendeskTicketCreatedEmailTemplateId,
                    tokens);
            }

            return allEmailsSent;
        }

        private async Task<bool> SendEmail(string recipient, string emailTemplateId,
            Dictionary<string, dynamic> personalisationTokens)
        {
            var emailSent = false;

            try
            {
                var emailResponse = await _notificationClient.SendEmailAsync(recipient, emailTemplateId, personalisationTokens);

                _logger.LogInformation($"Email sent - notification id '{emailResponse.id}', " +
                                       $"reference '{emailResponse.reference}, " +
                                       $"content '{emailResponse.content}'");
                emailSent = true;
            }
            catch (Exception ex)
            {
                var message = $"Error sending email template {emailTemplateId} to {recipient}. {ex.Message}";
                _logger.LogError(ex, message);
            }

            return emailSent;
        }
    }
}
