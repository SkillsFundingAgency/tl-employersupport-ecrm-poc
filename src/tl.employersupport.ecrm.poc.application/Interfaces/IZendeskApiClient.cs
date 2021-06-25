using System.Text.Json;
using System.Threading.Tasks;
using tl.employersupport.ecrm.poc.application.Model.Zendesk;

namespace tl.employersupport.ecrm.poc.application.Interfaces
{
    public interface IZendeskApiClient
    {
        Task<string> GetTicketJson(long ticketId, string sideloads = null);
        
        Task<JsonDocument> GetTicketJsonDocument(long ticketId, string sideloads = null);

        Task<string> GetTicketCommentsJson(long ticketId);

        Task<string> GetTicketAuditsJson(long ticketId);

        Task<JsonDocument> GetTicketFieldsJsonDocument();

        Task<JsonDocument> GetTicketSearchResultsJsonDocument(string query);

        Task<JsonDocument> PostTags(long ticketId, SafeTags tags);

        Task<JsonDocument> PutTags(long ticketId, SafeTags tags);
    }
}
