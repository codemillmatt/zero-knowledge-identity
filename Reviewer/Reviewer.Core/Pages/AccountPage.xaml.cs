using System;
using System.Collections.Generic;

using Xamarin.Forms;
using Reviewer.SharedModels;

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

            authorReviewList.ItemTapped += (sender, e) => authorReviewList.SelectedItem = null;
            authorReviewList.ItemSelected += listItemSelected;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            vm.RefreshCommand.Execute(null);
        }

        protected async void listItemSelected(object sender, SelectedItemChangedEventArgs args)
        {
            var review = args.SelectedItem as Review;

            if (review == null)
                return;

            await Navigation.PushAsync(new EditReviewPage(review.BusinessId, review.BusinessName, review));
        }
    }
}
