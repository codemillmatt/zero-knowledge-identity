using System;
using Reviewer.SharedModels;
namespace Reviewer.Core
{
    public class VideoPlayerViewModel : BaseViewModel
    {
        Video video;
        public Video Video { get => video; set => SetProperty(ref video, value); }

        public VideoPlayerViewModel(Video video)
        {
            this.Video = video;
        }
    }
}
