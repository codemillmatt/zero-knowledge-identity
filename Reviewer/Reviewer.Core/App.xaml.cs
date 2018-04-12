using System;
using System.Collections.Generic;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Reviewer.Services;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace Reviewer.Core
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            //DependencyService.Register<IDataService, MockDataService>();
            DependencyService.Register<IDataService, CosmosDataService>();
            DependencyService.Register<IAPIService, MockAPIService>();

            var tabbedPage = new TabbedPage();

            tabbedPage.Children.Add(new NavigationPage(new BusinessListPage()) { Title = "Reviews" });
            tabbedPage.Children.Add(new NavigationPage(new AccountPage()) { Title = "Me" });

            MainPage = tabbedPage;
        }

        protected override void OnStart()
        {
            base.OnStart();
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
