using System.Threading.Tasks;
using tl.employersupport.ecrm.poc.application.Model;

namespace tl.employersupport.ecrm.poc.application.Interfaces
{
    public interface ITicketService
    {
        Task<CombinedTicket> GetTicket(long ticketId);
    }
}
