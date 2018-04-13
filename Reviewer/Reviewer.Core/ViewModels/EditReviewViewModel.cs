using System;
using Reviewer.SharedModels;
using System.Windows.Input;
using Xamarin.Forms;
using System.Threading.Tasks;
namespace Reviewer.Core
{
    public class EditReviewViewModel : BaseViewModel
    {
        Review review;
        public Review Review { get => review; set => SetProperty(ref review, value); }

        Business business;
        public Business Business { get => business; set => SetProperty(ref business, value); }

        bool isNew;
        public bool IsNew { get => isNew; set => SetProperty(ref isNew, value); }

        public ICommand SaveCommand { get; }

        public event EventHandler SaveComplete;

        IIdentityService idService;

        public EditReviewViewModel(Review theReview, Business theBusiness)
        {
            Review = theReview;
            Business = theBusiness;

            SaveCommand = new Command(async () => await ExecuteSaveCommand()); ;

            Title = "A Review";

            IsNew = false;

            idService = DependencyService.Get<IIdentityService>();

            Review.Author = idService.DisplayName;
        }

        public EditReviewViewModel(Business theBusiness) :
            this(new Review { Id = Guid.NewGuid().ToString(), BusinessId = theBusiness.Id, BusinessName = theBusiness.Name }, theBusiness)
        {
            IsNew = true;
        }

        async Task ExecuteSaveCommand()
        {
            if (IsBusy)
                return;

            try
            {
                IsBusy = true;
                //vidService = DependencyService.Get<IIdentityService>();
                var authResult = await idService.GetCachedSignInToken();

                var webAPI = DependencyService.Get<IAPIService>();

                if (IsNew)
                {
                    Review.AuthorId = authResult.UniqueId;
                    await webAPI.InsertReview(Review, authResult.AccessToken);
                }
                else
                    await webAPI.UpdateReview(Review, authResult.AccessToken);
            }
            finally
            {
                IsBusy = false;
            }

            SaveComplete?.Invoke(this, new EventArgs());
        }
    }
}
