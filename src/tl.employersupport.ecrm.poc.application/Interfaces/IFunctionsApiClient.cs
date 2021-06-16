using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tl.employersupport.ecrm.poc.application.Interfaces
{
    public interface IFunctionsApiClient
    {
        Task CallTestTimeoutFunction(int clientTimeout, int minTimeout, int maxTimeout);
    }
}
