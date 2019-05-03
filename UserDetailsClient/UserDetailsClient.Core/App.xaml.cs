using System;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Microsoft.Identity.Client;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace UserDetailsClient.Core
{
	public partial class App : Application
    {
        public static IPublicClientApplication PCA = null;

        // Azure AD B2C Coordinates
        public static string Tenant = "fabrikamb2c.onmicrosoft.com";
        public static string AzureADB2CHostname = "login.microsoftonline.com";
        public static string ClientID = "90c0fe63-bcf2-44d5-8fb7-b8bbc0b29dc6";
        public static string PolicySignUpSignIn = "b2c_1_susi";
        public static string PolicyEditProfile = "b2c_1_edit_profile";
        public static string PolicyResetPassword = "b2c_1_reset";

        public static string[] Scopes = { "https://fabrikamb2c.onmicrosoft.com/helloapi/demo.read" };
        public static string ApiEndpoint = "https://fabrikamb2chello.azurewebsites.net/hello";

        public static string AuthorityBase = $"https://{AzureADB2CHostname}/tfp/{Tenant}/";
        public static string Authority = $"{AuthorityBase}{PolicySignUpSignIn}";
        public static string AuthorityEditProfile = $"{AuthorityBase}{PolicyEditProfile}";
        public static string AuthorityPasswordReset = $"{AuthorityBase}{PolicyResetPassword}";

        public static object ParentActivityOrWindow { get; set; }

        public App ()
		{
            InitializeComponent();

            // default redirectURI; each platform specific project will have to override it with its own
            App.PCA = PublicClientApplicationBuilder.Create(ClientID)
                .WithB2CAuthority(Authority)
                .WithRedirectUri($"msal{ClientID}://auth")
                .Build();

            MainPage = new NavigationPage(new MainPage());
        }

        //public static Page GetMainPage()
        //{
        //    var rootPage = new MasterPage();
        //    return rootPage;
        //}

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