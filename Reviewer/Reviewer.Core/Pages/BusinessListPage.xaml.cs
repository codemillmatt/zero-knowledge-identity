using System;
using System.Collections.Generic;

using Xamarin.Forms;
using Reviewer.SharedModels;

namespace Reviewer.Core
{
    public partial class BusinessListPage : ContentPage
    {
        BusinessListViewModel vm;

        public BusinessListPage()
        {
            InitializeComponent();

            vm = new BusinessListViewModel();
            BindingContext = vm;

            allBusList.ItemSelected += listItemSelected;
            allBusList.ItemTapped += (sender, args) => allBusList.SelectedItem = null;

            vm.Title = "Businesses";
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            vm.RefreshCommand.Execute(null);
        }

        protected async void listItemSelected(object sender, SelectedItemChangedEventArgs args)
        {
            var business = args.SelectedItem as Business;

            if (business == null)
                return;

            await Navigation.PushAsync(new BusinessReviewsPage(business));
        }
    }
}
