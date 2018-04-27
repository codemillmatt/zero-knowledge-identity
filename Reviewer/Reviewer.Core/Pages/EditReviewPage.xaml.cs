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
            videoList.SelectedItemChanged += VideoList_SelectedItemChanged;
        }

        public EditReviewPage(Review review) : base()
        {
            InitializeComponent();

            vm = new EditReviewViewModel(review);
            BindingContext = vm;
            isNew = false;

            vm.SaveComplete += SaveComplete;

            videoList.SelectedItemChanged += VideoList_SelectedItemChanged;
        }

        async void VideoList_SelectedItemChanged(object sender, EventArgs e)
        {
            if (!(sender is HorizontalList horizontalList))
                return;

            if (!(horizontalList.SelectedItem is Video video))
                return;

            var playerPage = new NavigationPage(new VideoPlayerPage(video));

            await Navigation.PushModalAsync(playerPage, true);
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
