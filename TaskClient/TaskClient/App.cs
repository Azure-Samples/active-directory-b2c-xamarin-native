using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

using Xamarin.Forms;

namespace TaskClient
{
    public class App : Application
    {
        // the todos (databinded from TasksPage.XAML)
        public static ObservableCollection<string> Tasks { get; set; }
        // the app
        public static PublicClientApplication PCApplication { get; set;  }

        // app coordinates
		public static string ClientID = "90c0fe63-bcf2-44d5-8fb7-b8bbc0b29dc6";
        public static string[] Scopes = { ClientID };
        
        
		public static string SignUpSignInpolicy = "b2c_1_susi";        
        public static string ResetPasswordpolicy = "b2c_1_reset";        
        public static string Authority = "https://login.microsoftonline.com/fabrikamb2c.onmicrosoft.com/";
        public static string APIbaseURL = "https://aadb2cplayground.azurewebsites.net";

        public App()
        {
            Tasks = new ObservableCollection<string>();

            PCApplication = new PublicClientApplication(Authority, ClientID);
            //{
            //    RedirectUri = redirectURI,
            //};

            // The root page of your application
            MainPage = new NavigationPage(new TaskClient.WelcomePage());
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
