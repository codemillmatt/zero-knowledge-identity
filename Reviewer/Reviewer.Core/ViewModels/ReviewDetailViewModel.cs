using System;
using Reviewer.SharedModels;
namespace Reviewer.Core
{
    public class ReviewDetailViewModel : BaseViewModel
    {
        Review review;
        public Review Review { get => review; set => SetProperty(ref review, value); }

        Business business;
        public Business Business { get => business; set => SetProperty(ref business, value); }

        public ReviewDetailViewModel(Review review, Business business)
        {
            Review = review;
            Business = business;

            Title = "Details";
        }
    }
}
