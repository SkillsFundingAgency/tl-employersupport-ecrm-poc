using System.Diagnostics;

namespace tl.employersupport.ecrm.poc.application.Model.Ecrm
{
    [DebuggerDisplay("{DebuggerDisplay(), nq}")]
    public class EmployerSearchRequest
    {
        public string CompanyName { get; init; }
        public string AddressLine1 { get; init; }
        public string Postcode { get; init; }
        public string NumberOfEmployees { get; init; }

        private string DebuggerDisplay()
            => $"{CompanyName} " +
               $"{Postcode} " +
               $"{nameof(NumberOfEmployees)}={NumberOfEmployees}";
    }
}
