using System;
using System.Diagnostics;

namespace tl.employersupport.ecrm.poc.application.Model.Ecrm
{
    [DebuggerDisplay("{DebuggerDisplay(), nq}")]
    public class Employer
    {
        public Guid AccountId { get; init; }
        public string CompanyName { get; init; }

        private string DebuggerDisplay()
            => $"{AccountId} " +
               $"{CompanyName} ";
    }
}
