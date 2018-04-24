using System;
using Reviewer.SharedModels;
using System.Windows.Input;
using Xamarin.Forms;
using System.Threading.Tasks;
using Plugin.Media.Abstractions;
using Plugin.Media;
using System.Collections.Generic;

namespace Reviewer.Core
{
    public class EditReviewViewModel : BaseViewModel
    {
        Review review;
        public Review Review { get => review; set => SetProperty(ref review, value); }

        bool isNew;
        public bool IsNew { get => isNew; set => SetProperty(ref isNew, value); }

        public ICommand SaveCommand { get; }

        public event EventHandler SaveComplete;

        public ICommand TakePhotoCommand { get; }

        IIdentityService idService;

        public EditReviewViewModel(Review theReview)
        {
            Review = theReview;

            SaveCommand = new Command(async () => await ExecuteSaveCommand());
            TakePhotoCommand = new Command(async () => await ExecuteTakePhotoCommand());

            Title = "A Review";

            IsNew = false;

            idService = DependencyService.Get<IIdentityService>();

            Review.Author = idService.DisplayName;
        }

        public EditReviewViewModel(string businessId, string businessName) :
            this(new Review { Id = Guid.NewGuid().ToString(), BusinessId = businessId, BusinessName = businessName })
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

        async Task ExecuteTakePhotoCommand()
        {
            var actions = new List<string>();

            if (CrossMedia.Current.IsTakePhotoSupported && CrossMedia.Current.IsCameraAvailable)
                actions.Add("Take Photo");

            if (CrossMedia.Current.IsPickPhotoSupported)
                actions.Add("Pick Photo");


            var result = await Application.Current.MainPage.DisplayActionSheet("Take Photo", "Cancel", "", actions.ToArray());

            if (result == "Take Photo")
            {
                var photo = CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions { PhotoSize = PhotoSize.Medium });
            }
            else if (result == "Pick Photo")
            {
                var photo = CrossMedia.Current.PickPhotoAsync();
            }
        }
    }
}
