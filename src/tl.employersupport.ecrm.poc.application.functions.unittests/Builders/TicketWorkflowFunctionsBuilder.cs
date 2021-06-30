using NSubstitute;
using tl.employersupport.ecrm.poc.application.Interfaces;

namespace tl.employersupport.ecrm.poc.functions.unittests.Builders
{
    public class TicketWorkflowFunctionsBuilder
    {
        public TicketWorkflowFunctions Build(
            IDateTimeService dateTimeService = null,
            IEmailService emailService = null,
            ITicketService ticketService = null,
            IEcrmService ecrmService = null)
        {
            emailService ??= Substitute.For<IEmailService>();
            dateTimeService ??= Substitute.For<IDateTimeService>();
            ticketService ??= Substitute.For<ITicketService>();
            ecrmService ??= Substitute.For<IEcrmService>();

            return new TicketWorkflowFunctions(
                dateTimeService, 
                emailService, 
                ticketService, 
                ecrmService);
        }
    }
}
