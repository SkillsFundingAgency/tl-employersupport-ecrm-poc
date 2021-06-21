using NSubstitute;
using tl.employersupport.ecrm.poc.application.Interfaces;

namespace tl.employersupport.ecrm.poc.application.functions.unittests.Builders
{
    public class TicketWorkflowFunctionsBuilder
    {
        public TicketWorkflowFunctions Build(
            IDateTimeService dateTimeService = null,
            IEmailService emailService = null,
            ITicketService ticketService = null)
        {
            emailService ??= Substitute.For<IEmailService>();
            dateTimeService ??= Substitute.For<IDateTimeService>();
            ticketService ??= Substitute.For<ITicketService>();

            return new TicketWorkflowFunctions(dateTimeService, emailService, ticketService);
        }
    }
}
