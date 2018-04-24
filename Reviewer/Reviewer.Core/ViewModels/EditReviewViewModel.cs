using System;
using Reviewer.SharedModels;
using System.Windows.Input;
using Xamarin.Forms;
using System.Threading.Tasks;
using Plugin.Media.Abstractions;
using Plugin.Media;
using System.Collections.Generic;
using System.IO;
using System.Collections.ObjectModel;

namespace Reviewer.Core
{
    public class EditReviewViewModel : BaseViewModel
    {
        Review review;
        public Review Review { get => review; set => SetProperty(ref review, value); }

        bool isNew;
        public bool IsNew { get => isNew; set => SetProperty(ref isNew, value); }

        public ObservableCollection<ImageSource> Photos { get; private set; } = new ObservableCollection<ImageSource>();

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

            if (Review.Photos != null)
            {
                foreach (var photo in Review.Photos)
                {
                    Photos.Add(ImageSource.FromUri(new Uri(photo)));
                }
            }
            else
            {
                Review.Photos = new List<string>();
            }
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


            var result = await Application.Current.MainPage.DisplayActionSheet("Take or Pick Photo", "Cancel", null, actions.ToArray());

            MediaFile mediaFile = null;
            if (result == "Take Photo")
            {
                var options = new StoreCameraMediaOptions
                {
                    PhotoSize = PhotoSize.Medium,
                    AllowCropping = true,
                    DefaultCamera = CameraDevice.Rear
                };

                mediaFile = await CrossMedia.Current.TakePhotoAsync(options);
            }
            else if (result == "Pick Photo")
            {
                mediaFile = await CrossMedia.Current.PickPhotoAsync();
            }

            if (mediaFile == null)
                return;

            //var photoStream = mediaFile.GetStream();
            //var source = ImageSource.FromStream(() => photoStream);
            //var coll = new List<ImageSource> { source };

            //Photos = new ObservableCollection<ImageSource>(coll);
            //Photos.Add(ImageSource.FromStream(() => photoStream));

            //mediaFile.Dispose();
        }
    }
}
