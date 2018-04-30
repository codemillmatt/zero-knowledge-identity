using System;
using Microsoft.Identity.Client;
namespace Reviewer.Core
{
    public interface IAuthenticationResult
    {
        IUser User { get; set; }
        string UniqueId { get; set; }
        string AccessToken { get; set; }
    }
}
