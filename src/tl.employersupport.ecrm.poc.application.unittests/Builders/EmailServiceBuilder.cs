using System;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Notify.Interfaces;
using NSubstitute;
using tl.employersupport.ecrm.poc.application.Interfaces;
using tl.employersupport.ecrm.poc.application.Model.Configuration;
using tl.employersupport.ecrm.poc.application.Services;

namespace tl.employersupport.ecrm.poc.application.unittests.Builders
{
    public class EmailServiceBuilder
    {
        public IEmailService Build(
            IOptions<EmailConfiguration> configuration = null,
            IAsyncNotificationClient notificationClient = null,
            ILogger<EmailService> logger = null)
        {
            notificationClient ??= Substitute.For<IAsyncNotificationClient>();
            logger ??= Substitute.For<ILogger<EmailService>>();

            configuration ??= new Func<IOptions<EmailConfiguration>>(() =>  {
                var config = Substitute.For<IOptions<EmailConfiguration>>();
                config.Value.Returns(new EmailConfiguration());
                return config;
            }).Invoke();
            
            return new EmailService(configuration, notificationClient, logger);
        }
    }
}
