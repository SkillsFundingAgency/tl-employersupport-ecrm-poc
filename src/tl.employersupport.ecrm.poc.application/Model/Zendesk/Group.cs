// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable UnusedMember.Global

using System;

namespace tl.employersupport.ecrm.poc.application.Model.Zendesk
{
    public class Group
    {
        public Uri Url { get; set; }

        public long? Id { get; set; }

        public string Name { get; set; }

        public bool? Deleted { get; set; }

        public DateTimeOffset? CreatedAt { get; set; }

        public DateTimeOffset? UpdatedAt { get; set; }
    }
}