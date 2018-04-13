using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace Reviewer.Core
{
    public partial class SignInPage : ContentPage
    {
        SignInViewModel vm;

        public SignInPage()
        {
            InitializeComponent();

            vm = new SignInViewModel();
            BindingContext = vm;

            vm.SuccessfulSignIn += (sender, e) =>
            {
                System.Diagnostics.Debug.WriteLine("SUCCESS!!");
            };

            vm.UnsuccessfulSignIn += async (sender, e) =>
            {
                await DisplayAlert("Sign In Error", "An error occurred during sign in. Try again later.", "OK");
            };
        }
    }
}
