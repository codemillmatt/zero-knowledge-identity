using System;
using System.Collections.Generic;

using Xamarin.Forms;
using Reviewer.SharedModels;

namespace Reviewer.Core
{
    public partial class BusinessReviewsPage : ContentPage
    {
        BusinessReviewViewModel vm;

        public BusinessReviewsPage(Business business)
        {
            InitializeComponent();

            vm = new BusinessReviewViewModel(business);

            BindingContext = vm;

            reviewList.ItemTapped += (sender, args) => reviewList.SelectedItem = null;
            reviewList.ItemSelected += ReviewList_ItemSelected;

            addNewReview.Clicked += async (sender, e) =>
            {
                var editPage = new EditReviewPage(vm.Business);

                await Navigation.PushModalAsync(new NavigationPage(editPage));
            };
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            vm.RefreshCommand.Execute(null);
        }

        async void ReviewList_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            var review = e.SelectedItem as Review;
            if (review == null)
                return;

            await Navigation.PushAsync(new ReviewDetailPage(review, vm.Business));
        }
    }
}
