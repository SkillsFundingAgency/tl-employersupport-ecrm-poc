using System;
using System.Threading.Tasks;

namespace tl.employersupport.ecrm.poc.application.Interfaces
{
    public interface IEmailService
    {
        public Task<bool> SendZendeskTicketCreatedEmail(
            long ticketId,
            DateTime createdAt);
    }
}
