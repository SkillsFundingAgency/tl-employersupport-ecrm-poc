using tl.employersupport.ecrm.poc.application.Model;
using tl.employersupport.ecrm.poc.application.Model.ZendeskTicket;

namespace tl.employersupport.ecrm.poc.application.functions.unittests.Builders
{
    public class EmployerContactTicketBuilder
    {
        private EmployerContactTicket _employerContactTicket;

        //public EmployerContactTicketBuilder()
        //{
        //    _employerContactTicket = new EmployerContactTicket();
        //}

        public EmployerContactTicket Build() =>
            _employerContactTicket;

        public EmployerContactTicketBuilder WithDefaultValues()
        {
            _employerContactTicket = new EmployerContactTicket()
            {
                Id = 4485
            };
            return this;
        }
    }
}
