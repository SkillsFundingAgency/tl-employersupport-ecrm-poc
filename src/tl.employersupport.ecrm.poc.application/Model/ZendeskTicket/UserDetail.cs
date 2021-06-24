using System.Diagnostics;

namespace tl.employersupport.ecrm.poc.application.Model.ZendeskTicket
{
    [DebuggerDisplay(nameof(UserDetail) +
                     " {" + nameof(Id) + "} " +
                     "{" + nameof(Name) + ", nq} " +
                     "({" + nameof(Email) + ", nq})")]
    public class UserDetail
    {
        public long Id { get; init; }
        public string Name { get; init; }
        public string Email { get; init; }
    }
}