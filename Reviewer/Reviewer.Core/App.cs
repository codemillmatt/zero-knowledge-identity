using System;
using Xamarin.Forms;
using System.Collections.Generic;

namespace Reviewer.Core
{
    public class App : Application
    {
        public App()
        {
            var tabbedPage = new TabbedPage();
            tabbedPage.Children.Add(new NavigationPage(new BusinessListPage()) { Title = "Reviews" });
            tabbedPage.Children.Add(new NavigationPage(new AccountPage()) { Title = "Me" });

            MainPage = tabbedPage;
        }
    }
}
