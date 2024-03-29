﻿// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable UnusedMember.Global

using System.Diagnostics;

namespace tl.employersupport.ecrm.poc.application.Model.Zendesk
{
    [DebuggerDisplay(nameof(TicketField) + 
                     " {" + nameof(Id) + "} " +
                     "{" + nameof(Title) + ", nq} " +
                     "({" + nameof(Type) + ", nq})")]
    public class TicketField
    {
        public long Id { get; init; }
        public string Type { get; init; }
        public string Title { get; init; }
        public bool Active { get; init; }
    }
}
