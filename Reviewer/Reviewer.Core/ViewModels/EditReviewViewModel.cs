using System;
using Reviewer.SharedModels;
namespace Reviewer.Core
{
    public class EditReviewViewModel : BaseViewModel
    {
        Review review;
        public Review Review { get => review; set => SetProperty(ref review, value); }

        public EditReviewViewModel(Review theReview)
        {
            Review = theReview;

            Title = "A Review";
        }
    }
}
