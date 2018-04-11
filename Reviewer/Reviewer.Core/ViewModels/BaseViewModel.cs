using System;
using Reviewer.SharedModels;

namespace Reviewer.Core
{
    public class BaseViewModel : ObservableObject
    {
        string title;
        public string Title
        {
            get => title;
            set => SetProperty(ref title, value);
        }

        bool isBusy;
        public bool IsBusy
        {
            get => isBusy;
            set => SetProperty(ref isBusy, value);
        }

    }
}
