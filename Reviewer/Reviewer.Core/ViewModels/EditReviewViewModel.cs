using System;
using Reviewer.SharedModels;
namespace Reviewer.Core
{
    public class EditReviewViewModel : BaseViewModel
    {
        Review review;
        public Review Review { get => review; set => SetProperty(ref review, value); }

        Business business;
        public Business Business { get => business; set => SetProperty(ref business, value); }

        public EditReviewViewModel(Review theReview, Business theBusiness)
        {
            Review = theReview;

            Title = "A Review";
        }

        public EditReviewViewModel(Business theBusiness) : this(null, theBusiness)
        {

        }
    }
}
