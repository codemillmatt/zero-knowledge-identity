using System;
using System.Windows.Input;
using System.Threading.Tasks;
using Microsoft.Identity.Client;
using Xamarin.Forms;
using Reviewer.SharedModels;
using System.Collections.Generic;

namespace Reviewer.Core
{
    public class AccountViewModel : BaseViewModel
    {
        public ICommand SignInCommand { get; }
        public ICommand RefreshCommand { get; }

        public event EventHandler SuccessfulSignIn;
        public event EventHandler UnsuccessfulSignIn;

        List<Review> reviews;
        public List<Review> Reviews { get => reviews; set => SetProperty(ref reviews, value); }

        bool loggedIn = false;
        public bool LoggedIn { get => loggedIn; set => SetProperty(ref loggedIn, value); }

        bool notLoggedIn = true;
        public bool NotLoggedIn { get => notLoggedIn; set => SetProperty(ref notLoggedIn, value); }

        string info;
        public string Info { get => info; set => SetProperty(ref info, value); }

        string notLoggedInInfo = "Sign in to unlock the wonderful world of reviews!";
        string loggedInInfo = "Hiya {user}! Here are your reviews so far!";

        AuthenticationResult authResult;

        IIdentityService identityService;

        public AccountViewModel()
        {
            Reviews = new List<Review>();

            SignInCommand = new Command(async () => await ExecuteSignInCommand());
            RefreshCommand = new Command(async () => await ExecuteRefreshCommand());

            Info = notLoggedInInfo;

            identityService = DependencyService.Get<IIdentityService>();

            Task.Run(async () => await CheckLoginStatus());
        }

        async Task ExecuteSignInCommand()
        {
            if (IsBusy)
                return;

            AuthenticationResult signInResult;
            try
            {
                IsBusy = true;

                signInResult = await identityService.Login();
            }
            finally
            {
                IsBusy = false;
            }

            if (signInResult?.User == null)
            {
                LoggedIn = false;
                NotLoggedIn = true;
                UnsuccessfulSignIn?.Invoke(this, new EventArgs());
            }
            else
            {
                LoggedIn = true;
                NotLoggedIn = false;
                SuccessfulSignIn?.Invoke(this, new EventArgs());
            }
        }

        async Task ExecuteRefreshCommand()
        {
            if (IsBusy)
                return;

            if (NotLoggedIn)
                return;

            try
            {
                IsBusy = true;

                var apiService = DependencyService.Get<IAPIService>();
                Reviews = await apiService.GetReviewsForAuthor(authResult.UniqueId, authResult.AccessToken);
            }
            finally
            {
                IsBusy = false;
            }
        }

        public async Task CheckLoginStatus()
        {
            if (IsBusy)
                return;

            try
            {
                IsBusy = true;
                authResult = await identityService.GetCachedSignInToken();

                if (authResult?.User != null)
                {
                    LoggedIn = true;
                    NotLoggedIn = false;

                    Title = identityService.DisplayName;
                    Info = loggedInInfo.Replace("{user}", identityService.DisplayName);
                }
                else
                {
                    Title = "Account";
                }
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
