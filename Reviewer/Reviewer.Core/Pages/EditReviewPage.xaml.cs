using System;
using System.Collections.Generic;

using Xamarin.Forms;
using Reviewer.SharedModels;

namespace Reviewer.Core
{
    public partial class EditReviewPage : ContentPage
    {
        EditReviewViewModel vm;
        bool isNew;

        public EditReviewPage(string businessId, string businessName)
        {
            InitializeComponent();
            vm = new EditReviewViewModel(businessId, businessName);
            BindingContext = vm;
            isNew = true;

            vm.SaveComplete += SaveComplete;
        }

        public EditReviewPage(Review review) : base()
        {
            InitializeComponent();

            vm = new EditReviewViewModel(review);
            BindingContext = vm;
            isNew = false;

            vm.SaveComplete += SaveComplete;
        }

        async void SaveComplete(object sender, EventArgs args)
        {
            if (isNew)
                await Navigation.PopModalAsync();
            else
                await Navigation.PopAsync();
        }
    }

}
