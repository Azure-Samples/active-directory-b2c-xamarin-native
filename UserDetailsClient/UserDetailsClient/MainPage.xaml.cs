using Microsoft.Identity.Client;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace UserDetailsClient
{
    public partial class MainPage : ContentPage
    {
        
        public MainPage()
        {
            InitializeComponent();
        }
        protected override async void OnAppearing()
        {
            UpdateSignInState(false);

            // Check to see if we have a User
            // in the cache already.
            try
            {
                if (App.PCA.Users.Count() > 0)
                {
                    AuthenticationResult ar = await App.PCA.AcquireTokenSilentAsync(App.Scopes, App.PCA.Users.First(), App.Authority, false);
                    UpdateUserInfo(ar.IdToken);
                    UpdateSignInState(true);
                }
            }
            catch (Exception)
            {
                // Uncomment for debugging purposes
                // await DisplayAlert($"Exception:", ex.ToString(), "Dismiss");

                // Doesn't matter, we go in interactive mode
                UpdateSignInState(false);
            }
        }
        async void OnSignInSignOut(object sender, EventArgs e)
        {
            try
            {
                if (btnSignInSignOut.Text == "Sign in")
                {
                    AuthenticationResult ar = await App.PCA.AcquireTokenAsync(App.Scopes, App.UiParent);
                    UpdateUserInfo(ar.IdToken); //TODO:Refactor
                    UpdateSignInState(true);
                }
                else
                {
                    foreach (var user in App.PCA.Users)
                    {
                        App.PCA.Remove(user);
                    }
                    UpdateSignInState(false);
                }
            }
            catch(Exception ex)
            {
                // Password reset TODO:Comment indicate B2C only
                if (ex.Message.Contains("AADB2C90118"))
                    OnPasswordReset();
                // Alert if any exception excludig user cancelling sign-in dialog
                else if (((ex as MsalException)?.ErrorCode != "authentication_canceled"))
                    await DisplayAlert($"Exception:", ex.ToString(), "Dismiss");
            }
        }
        public void UpdateUserInfo(string idToken)
        {
            JObject user = ParseIdToken(idToken);
            lblDisplayName.Text = user["displayName"]?.ToString();
            lblGivenName.Text = user["givenName"]?.ToString();
            lblId.Text = user["oid"]?.ToString();               
            lblSurname.Text = user["surname"]?.ToString();
            lblUserPrincipalName.Text = user["identityProvider"]?.ToString();
        }

        JObject ParseIdToken(string idToken)
        {
            // Get the piece with actual user info
            idToken = idToken.Split('.')[1];

            // Ensure it's the proper length for decode
            // TODO: Figure out whether B2C should do this automatically
            idToken = idToken.PadRight(idToken.Length + (idToken.Length % 4), '=');

            // Decode
            var byteArray = Convert.FromBase64String(idToken);
            var jsonString = UTF8Encoding.UTF8.GetString(byteArray, 0, byteArray.Count());
            
            // Parse
            return JObject.Parse(jsonString);
        }
        async void OnCallApi(object sender, EventArgs e)
        {
            try
            {
                // TODO: Confirm if need to handle fail and call Interactive...
                AuthenticationResult ar = await App.PCA.AcquireTokenSilentAsync(App.Scopes, App.PCA.Users.FirstOrDefault(), App.Authority, false);
                string token = ar.AccessToken;

                // Get data from API
                HttpClient client = new HttpClient();
                HttpRequestMessage message = new HttpRequestMessage(HttpMethod.Get, App.ApiEndpoint);
                message.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                HttpResponseMessage response = await client.SendAsync(message);
                string responseString = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode)
                {
                    // TODO: Switch to update UI rather than a popup
                    await DisplayAlert($"Response from API {App.ApiEndpoint}", responseString, "Dismiss");

                }
                else
                {
                    await DisplayAlert("Something went wrong with the API call", responseString, "Dismiss");
                }
            }
            catch (MsalUiRequiredException ex)
            {
                //Call interactive, alert and sign-out
            }
            catch (Exception ex)
            {
                await DisplayAlert($"Exception:", ex.ToString(), "Dismiss");
            }
        }

        async void OnEditProfile(object sender, EventArgs e)
        {
            try
            {
                // TODO: Figure out why
                // 1. This is not doing SSO
                AuthenticationResult ar = await App.PCA.AcquireTokenAsync(App.Scopes, App.PCA.Users.FirstOrDefault(), UIBehavior.SelectAccount, string.Empty, null, App.AuthorityEditProfile, App.UiParent);
                UpdateUserInfo(ar.IdToken);
            }
            catch (Exception ex)
            {
                // Alert if any exception excludig user cancelling sign-in dialog
                if (((ex as MsalException)?.ErrorCode != "authentication_canceled"))
                    await DisplayAlert($"Exception:", ex.ToString(), "Dismiss");
            }
        }
        async void OnPasswordReset()
        {
            try
            {
                // TODO: Align with Edit Profile once it's fixed
                // TODO: Decide on whether we want to UpdateSignInState or not
                AuthenticationResult ar = await App.PCA.AcquireTokenAsync(App.Scopes, App.PCA.Users.FirstOrDefault(), UIBehavior.SelectAccount, string.Empty, null, App.AuthorityPasswordReset, App.UiParent);
                UpdateUserInfo(ar.IdToken);
            }
            catch (Exception ex)
            {
                // Alert if any exception excludig user cancelling sign-in dialog
                if (((ex as MsalException)?.ErrorCode != "authentication_canceled"))
                    await DisplayAlert($"Exception:", ex.ToString(), "Dismiss");
            }
        }

        void UpdateSignInState(bool isSignedIn)
        {
            btnSignInSignOut.Text = isSignedIn ? "Sign out" : "Sign in";
            btnEditProfile.IsVisible = isSignedIn;
            btnCallApi.IsVisible = isSignedIn;
            slUser.IsVisible = isSignedIn;
        }
    }

    
}
