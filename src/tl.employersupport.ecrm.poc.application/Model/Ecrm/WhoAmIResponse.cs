using System;

namespace tl.employersupport.ecrm.poc.application.Model.Ecrm
{
    public class WhoAmIResponse
    {
        public Guid BusinessUnitId { get; init; }
        public Guid UserId { get; init; }
        public Guid OrganizationId { get; init; }
    }
}