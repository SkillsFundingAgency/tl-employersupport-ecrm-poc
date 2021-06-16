using NSubstitute;
using tl.employersupport.ecrm.poc.application.Interfaces;

namespace tl.employersupport.ecrm.poc.application.functions.tests.Builders
{
    public class TicketWorkflowFunctionsBuilder
    {
        public TicketWorkflowFunctions Build(
            IEmailService emailService = null,
            IDateTimeService dateTimeService = null)
        {
            emailService ??= Substitute.For<IEmailService>();
            dateTimeService ??= Substitute.For<IDateTimeService>();

            return new TicketWorkflowFunctions(emailService, dateTimeService);
        }
    }
}
