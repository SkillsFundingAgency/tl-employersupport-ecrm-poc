using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using tl.employersupport.ecrm.poc.application.Model;
using tl.employersupport.ecrm.poc.application.Model.Zendesk;

namespace tl.employersupport.ecrm.poc.application.Interfaces
{
    public interface ITicketService
    {
        Task<CombinedTicket> GetTicket(long ticketId);

        Task<IDictionary<long, TicketField>> GetTicketFields();

        Task<SafeTags> GetTicketTags(long ticketId);

        Task<IList<TicketSearchResult>> SearchTickets();
        
        Task AddTag(long ticketId, CombinedTicket ticket, string tag);
    }
}
