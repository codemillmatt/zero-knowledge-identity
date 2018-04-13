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

        public EditReviewPage(Business business)
        {
            InitializeComponent();
            vm = new EditReviewViewModel(business);
            BindingContext = vm;
            isNew = true;

            vm.SaveComplete += SaveComplete;
        }

        public EditReviewPage(Business business, Review review) : base()
        {
            InitializeComponent();

            vm = new EditReviewViewModel(review, business);
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
