using System;
using Microsoft.Identity.Client;

namespace Reviewer.Core
{
    public class AuthResult : IAuthenticationResult
    {
        public AuthResult()
        {
        }

        public IUser User { get; set; }
        public string UniqueId { get; set; }
        public string AccessToken { get; set; }
    }

    public class MockUser : IUser
    {
        public string DisplayableId => throw new NotImplementedException();

        public string Name => throw new NotImplementedException();

        public string IdentityProvider => throw new NotImplementedException();

        public string Identifier => throw new NotImplementedException();
    }
}
