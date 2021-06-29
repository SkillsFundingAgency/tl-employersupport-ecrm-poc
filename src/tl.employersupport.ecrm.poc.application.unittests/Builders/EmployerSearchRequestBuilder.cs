using tl.employersupport.ecrm.poc.application.Model.Ecrm;

namespace tl.employersupport.ecrm.poc.application.unittests.Builders
{
    public class EmployerSearchRequestBuilder
    {
        public EmployerSearchRequest Build() =>
            new()
            {
                CompanyName = "Fake Company",
                AddressLine1 = "1 Quinton Road",
                Postcode = "CV1 2WT",
                NumberOfEmployees = "100"
            };
    }
}
