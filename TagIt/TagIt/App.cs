using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;
using TagIt.Views;

namespace TagIt
{
    public class App : Application
    {

        public static IPlatformSpecific PlatformSpecific { get; private set; }

        private static double tagAccuracy;
        public static double TagAccuracy
        {
            get {
                return TagAccuracy * 0.01;
            }
            set { tagAccuracy = value; }
        }

        public static void Init(IPlatformSpecific customPlatformSpecific = null)
        {
            App.PlatformSpecific = customPlatformSpecific;
        }

        public App()
        {
            //// The root page of your application
            //MainPage = new ContentPage
            //{
            //    Content = new StackLayout
            //    {
            //        VerticalOptions = LayoutOptions.Center,
            //        Children = {
            //             new Label {
            //                 HorizontalTextAlignment = TextAlignment.Center,
            //                 Text = "Welcome to Xamarin Forms!"
            //             }
            //         }
            //    }
            //};
            var tabs = new TabbedPage
            {
                Title = "TagIt",
                BackgroundColor = Color.White,
                //BindingContext = new AutoHashViewModel(),
                Children =
                {
                    new TagItMain(),
                    //new Settings(),
                    new About()
                }
            };

            MainPage = new NavigationPage(tabs)
            {
                BarBackgroundColor = Color.FromHex("3498db"),
                BarTextColor = Color.White,
                BackgroundColor = Color.White
            };
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
