using System;
using System.Collections.Generic;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Reviewer.Services;

using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace Reviewer.Core
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MonkeyCache.FileStore.Barrel.ApplicationId = "buildreviewer";

            //DependencyService.Register<IDataService, MockDataService>();
            DependencyService.Register<IDataService, CosmosDataService>();
            //DependencyService.Register<IAPIService, MockAPIService>();
            DependencyService.Register<IAPIService, WebAPIService>();
            DependencyService.Register<IStorageService, StorageService>();

            var tabbedPage = new TabbedPage();

            tabbedPage.Children.Add(new NavigationPage(new BusinessListPage()) { Title = "Reviews" });
            //tabbedPage.Children.Add(new NavigationPage(new SignInPage()) { Title = "Me" });
            tabbedPage.Children.Add(new NavigationPage(new AccountPage()) { Title = "Me" });

            MainPage = tabbedPage;
        }

        protected override void OnStart()
        {
            base.OnStart();

            AppCenter.Start($"{APIKeys.AppCenterIOSKey}{APIKeys.AppCenterAndroidKey}");
        }

        protected override void OnSleep()
        {
            base.OnSleep();
        }

        protected override void OnResume()
        {
            base.OnResume();
        }
    }
}
