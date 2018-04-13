using System;
using System.Windows.Input;
using System.Threading.Tasks;
using Xamarin.Forms;
using Microsoft.Identity.Client;

namespace Reviewer.Core
{
    public class SignInViewModel : BaseViewModel
    {
        public ICommand SignInCommand { get; }

        public event EventHandler SuccessfulSignIn;
        public event EventHandler UnsuccessfulSignIn;

        public SignInViewModel()
        {
            SignInCommand = new Command(async () => await ExecuteSignInCommand());
            IsBusy = false;
        }

        async Task ExecuteSignInCommand()
        {
            if (IsBusy)
                return;

            AuthenticationResult signInResult;
            try
            {
                IsBusy = true;

                var identityService = DependencyService.Get<IIdentityService>();

                signInResult = await identityService.Login();
            }
            finally
            {
                IsBusy = false;
            }

            if (signInResult?.User == null)
                UnsuccessfulSignIn?.Invoke(this, new EventArgs());
            else
                SuccessfulSignIn?.Invoke(this, new EventArgs());
        }
    }
}
