using System.Diagnostics;

namespace tl.employersupport.ecrm.poc.application.Model.ZendeskTicket
{
    [DebuggerDisplay(nameof(UserDetail) +
                     " {" + nameof(Id) + "} " +
                     "({" + nameof(Name) + ", nq})")]
    public class OrganisationDetail
    {
        public long Id { get; init; }
        public string Name { get; init; }
    }
}