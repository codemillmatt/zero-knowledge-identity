using System;
using System.Collections.Generic;

using Xamarin.Forms;
using Reviewer.SharedModels;

namespace Reviewer.Core
{
    public partial class ReviewDetailPage : ContentPage
    {
        ReviewDetailViewModel vm;
        public ReviewDetailPage(Review review, Business business)
        {
            InitializeComponent();

            vm = new ReviewDetailViewModel(review, business);
            vm.Title = "Review Details";

            BindingContext = vm;
        }

        public ReviewDetailPage()
        {
            InitializeComponent();
        }
    }
}
