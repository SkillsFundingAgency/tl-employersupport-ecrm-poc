// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable UnusedMember.Global

namespace tl.employersupport.ecrm.poc.application.Model.Zendesk
{
    public class TicketResponse
    {
        public Ticket Ticket { get; set; }
        public User[] Users { get; set; }
        public Group[] Groups { get; set; }
        public Organization[] Organizations { get; set; }
    }
}