using System;
using Microsoft.Identity.Client;
using System.Threading.Tasks;
namespace Reviewer.Core
{
    public interface IIdentityService
    {
        string DisplayName { get; set; }

        Task<IAuthenticationResult> Login();
        Task<IAuthenticationResult> GetCachedSignInToken();
        void Logout();
        UIParent UIParent { get; set; }
    }
}
