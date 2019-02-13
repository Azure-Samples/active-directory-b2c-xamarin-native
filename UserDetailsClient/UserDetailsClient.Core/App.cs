using System;
using Microsoft.Identity.Client;

using Xamarin.Forms;

namespace UserDetailsClient.Core
{
    public class App : Application
    {
        public static PublicClientApplication PCA = null;

        // Azure AD B2C Coordinates
        public static string Tenant = "navtest.partner.onmschina.cn";
        //public static string AzureADB2CHostname = "navtest.b2clogin.cn";
        public static string AzureADB2CHostname = "login.chinacloudapi.cn";
        public static string ClientID = "bb88579c-36d8-45ec-9b8b-a226ee7619d3";
        public static string PolicySignUpSignIn = "b2c_1_susi";
        public static string PolicyEditProfile = "b2c_1_edit_profile";
        public static string PolicyResetPassword = "b2c_1_reset";

        public static string[] Scopes = { "https://navtest.partner.onmschina.cn/navtestapp/user_impersonation" };
        public static string ApiEndpoint = "https://fabrikamb2chello.azurewebsites.net/hello";

        public static string AuthorityBase = $"https://{AzureADB2CHostname}/tfp/{Tenant}/";
        public static string Authority = $"{AuthorityBase}{PolicySignUpSignIn}";
        public static string AuthorityEditProfile = $"{AuthorityBase}{PolicyEditProfile}";
        public static string AuthorityPasswordReset = $"{AuthorityBase}{PolicyResetPassword}";

        public static UIParent UiParent { get; set; }

        public App()
        {
            // default redirectURI; each platform specific project will have to override it with its own
            PCA = new PublicClientApplication(ClientID, Authority);
            PCA.ValidateAuthority = false;
           
            PCA.RedirectUri = $"msal{ClientID}://auth";

            MainPage = new NavigationPage(new MainPage());
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
