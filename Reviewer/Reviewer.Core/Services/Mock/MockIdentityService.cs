using System;
using System.Threading.Tasks;
using Microsoft.Identity.Client;
using Xamarin.Forms;
using Reviewer.Core;

//[assembly: Dependency(typeof(MockIdentityService))]
namespace Reviewer.Core
{
    public class MockIdentityService : IIdentityService
    {
        public MockIdentityService()
        {
        }

        string displayName = "NonLoggedIn User!!";
        public string DisplayName { get => displayName; set => displayName = value; }

        public UIParent UIParent { get; set; }

        public Task<IAuthenticationResult> GetCachedSignInToken()
        {
            var auth = new AuthResult
            {
                User = new MockUser(),
                AccessToken = "",
                UniqueId = Guid.Empty.ToString()
            };

            return Task.FromResult((IAuthenticationResult)auth);
        }

        public Task<IAuthenticationResult> Login()
        {

            var auth = new AuthResult
            {
                User = new MockUser(),
                AccessToken = "",
                UniqueId = Guid.Empty.ToString()
            };

            return Task.FromResult((IAuthenticationResult)auth);
        }

        public void Logout()
        {
            // intentionally blank; 
        }
    }
}
