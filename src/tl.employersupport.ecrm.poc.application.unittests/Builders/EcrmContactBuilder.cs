using System;
using tl.employersupport.ecrm.poc.application.Model.Ecrm;

namespace tl.employersupport.ecrm.poc.application.unittests.Builders
{
    public class EcrmContactBuilder
    {
        public Contact Build() => 
            Build(Guid.Parse("437ed360-6ea0-489f-8dae-f89dbb50bddf"),
                Guid.Parse("b48bab30-04e1-45f6-9b13-13be025a1f41"));

        //TODO: Restructure builders -
        //  builder.WithContactId(x).WithParentAccountId(y).Build()
        public Contact Build(Guid parentAccountId) => 
            Build(null, parentAccountId);

        public Contact Build(Guid? contactId, Guid parentAccountId) =>
            new()
            {
                //Id = contactId,
                ParentAccountId = parentAccountId,
                FirstName = "Test",
                LastName = "Contact"
            };
    }
}
