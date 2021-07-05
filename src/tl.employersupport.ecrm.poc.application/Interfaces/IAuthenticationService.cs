using System.Threading.Tasks;

namespace tl.employersupport.ecrm.poc.application.Interfaces
{
    public interface IAuthenticationService
    {
        Task<string> GetAccessToken();
    }
}
