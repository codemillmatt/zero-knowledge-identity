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
using System.Diagnostics;

namespace Reviewer.Core
{
    public class EditReviewViewModel : BaseViewModel
    {
        Review review;
        public Review Review { get => review; set => SetProperty(ref review, value); }

        bool isNew;
        public bool IsNew { get => isNew; set => SetProperty(ref isNew, value); }

        //public ObservableCollection<ImageSource> Photos { get; private set; } = new ObservableCollection<ImageSource>();

        List<ImageSource> photos;
        public List<ImageSource> Photos { get => photos; set => SetProperty(ref photos, value); }

        public ICommand SaveCommand { get; }

        public event EventHandler SaveComplete;

        public ICommand TakePhotoCommand { get; }

        IIdentityService idService;

        public EditReviewViewModel(Review theReview)
        {
            Photos = new List<ImageSource>();

            Review = theReview;

            SaveCommand = new Command(async () => await ExecuteSaveCommand());
            TakePhotoCommand = new Command(async () => await ExecuteTakePhotoCommand());

            Title = "A Review";

            IsNew = false;

            idService = DependencyService.Get<IIdentityService>();

            Review.Author = idService.DisplayName;

            var thePhotos = new List<ImageSource>();

            if (Review.Photos != null)
            {
                //thePhotos.AddRange(Review.Photos);
                foreach (var photo in Review.Photos)
                {
                    thePhotos.Add(ImageSource.FromUri(new Uri(photo)));
                }

                Photos = thePhotos;
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
            if (IsBusy)
                return;

            try
            {
                IsBusy = true;

                var actions = new List<string>();

                if (CrossMedia.Current.IsTakePhotoSupported && CrossMedia.Current.IsCameraAvailable)
                    actions.Add("Take Photo");

                if (CrossMedia.Current.IsTakeVideoSupported && CrossMedia.Current.IsCameraAvailable)
                    actions.Add("Take Video");

                if (CrossMedia.Current.IsPickPhotoSupported)
                    actions.Add("Pick Photo");


                var result = await Application.Current.MainPage.DisplayActionSheet("Take or Pick Photo", "Cancel", null, actions.ToArray());


                bool isVideo = false;
                MediaFile mediaFile = null;
                if (result == "Take Photo")
                {
                    var options = new StoreCameraMediaOptions
                    {
                        PhotoSize = PhotoSize.Medium,
                        DefaultCamera = CameraDevice.Rear
                    };

                    mediaFile = await CrossMedia.Current.TakePhotoAsync(options);
                }
                else if (result == "Take Video")
                {
                    var options = new StoreVideoOptions
                    {
                        CompressionQuality = 50,
                        CustomPhotoSize = 50,
                        DefaultCamera = CameraDevice.Rear,
                        Quality = VideoQuality.Medium
                    };

                    mediaFile = await CrossMedia.Current.TakeVideoAsync(options);
                    isVideo = true;
                }
                else if (result == "Pick Photo")
                {
                    mediaFile = await CrossMedia.Current.PickPhotoAsync();
                }

                if (mediaFile == null)
                    return;

                UploadProgress progressUpdater = new UploadProgress();

                using (var mediaStream = mediaFile.GetStream())
                {
                    var storageService = DependencyService.Get<IStorageService>();

                    if (isVideo)
                    {
                        var vidConverter = DependencyService.Get<IVideoConversion>();

                        var finalPath = await vidConverter?.ConvertToMP4(mediaFile.Path);

                        var theStream = File.OpenRead(finalPath);
                    }

                    var blobAddress = await storageService.UploadBlob(mediaStream, progressUpdater);
                    Debug.WriteLine(blobAddress);

                    var thePhotos = new List<ImageSource>();
                    thePhotos.AddRange(Photos);
                    thePhotos.Add(ImageSource.FromUri(blobAddress));

                    Photos = thePhotos;

                    Review.Photos.Add(blobAddress.AbsoluteUri);

                    // Write to a queue to have the review record updated if we're in edit mode
                    // this way if a person takes a photo on an existing record, they don't have to click save
                    // in order for it to be persisted
                    if (!IsNew)
                    {
                        var functionApi = DependencyService.Get<IAPIService>();
                        await functionApi.WritePhotoInfoToQueue(Review.Id, blobAddress.AbsoluteUri);
                    }
                };
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
