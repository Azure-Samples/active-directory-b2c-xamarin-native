using System;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Microsoft.Identity.Client;
using UserDetailsClient.Core.Features.LogOn;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace UserDetailsClient.Core
{
	public partial class App : Application
    {

        public static string ApiEndpoint = "https://fabrikamb2chello.azurewebsites.net/hello";

        public App ()
		{
            InitializeComponent();

            DependencyService.Register<B2CAuthenticationService>();

            MainPage = new NavigationPage(new MainPage());
        }

        protected override void OnStart ()
		{
        }

		protected override void OnSleep ()
		{
			// Handle when your app sleeps
		}

		protected override void OnResume ()
		{
			// Handle when your app resumes
		}
	}
}