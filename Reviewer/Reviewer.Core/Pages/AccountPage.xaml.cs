using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace Reviewer.Core
{
    public partial class AccountPage : ContentPage
    {
        AccountViewModel vm;
        public AccountPage()
        {
            InitializeComponent();

            vm = new AccountViewModel();
            BindingContext = vm;
        }

        //protected async override void OnAppearing()
        //{
        //    base.OnAppearing();

        //    await vm.CheckLoginStatus();
        //}
    }
}
