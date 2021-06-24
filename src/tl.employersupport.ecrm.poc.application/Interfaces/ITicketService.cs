using System.Collections.Generic;
using System.Threading.Tasks;
using tl.employersupport.ecrm.poc.application.Model;
using tl.employersupport.ecrm.poc.application.Model.Zendesk;
using tl.employersupport.ecrm.poc.application.Model.ZendeskTicket;

namespace tl.employersupport.ecrm.poc.application.Interfaces
{
    public interface ITicketService
    {
        Task<EmployerContactTicket> GetEmployerContactTicket(long ticketId);

        Task<CombinedTicket> GetTicket(long ticketId);

        Task<IDictionary<long, TicketField>> GetTicketFields();

        Task<SafeTags> GetTicketTags(long ticketId);

        Task<IList<TicketSearchResult>> SearchTickets();

        Task AddTag(long ticketId, CombinedTicket ticket, string tag);

        Task ModifyTags(long ticketId, SafeTags tags);
    }
}
