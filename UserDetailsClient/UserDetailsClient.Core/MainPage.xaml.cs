using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using Microsoft.Identity.Client;
using Newtonsoft.Json.Linq;
using Xamarin.Forms;

namespace UserDetailsClient.Core
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
            CheckForAccountsAsync();
        }

        private async void CheckForAccountsAsync()
        {
            UpdateSignInState(false);
            
            try
            {
                // Check if we already have an account in the cache
                IEnumerable<IAccount> accounts = await App.PCA.GetAccountsAsync();
                AuthenticationResult authResult = await
                    App.PCA.AcquireTokenSilentAsync(
                        App.Scopes,
                        GetAccountByPolicy(
                            accounts,
                            App.PolicySignUpSignIn),
                        App.Authority,
                        false);
                UpdateUserInfo(authResult);
                UpdateSignInState(true);
            }
            catch (Exception ex)
            {
                // Doesn't matter, we go to interactive mode
                UpdateSignInState(false);
            }
        }

        async void OnSignInSignOut(object sender, EventArgs e)
        {
            IEnumerable<IAccount> accounts = await App.PCA.GetAccountsAsync();
            try
            {
                if (btnSignInSignOut.Text == "Sign in")
                {
                    AuthenticationResult ar = await App.PCA.AcquireTokenAsync(App.Scopes, GetAccountByPolicy(accounts, App.PolicySignUpSignIn), App.UiParent);
                    UpdateUserInfo(ar);
                    UpdateSignInState(true);
                }
                else
                {
                    while (accounts.Any())
                    {
                        await App.PCA.RemoveAsync(accounts.FirstOrDefault());
                        accounts = await App.PCA.GetAccountsAsync();
                    }
                    UpdateSignInState(false);
                }
            }
            catch (Exception ex)
            {
                // Checking the exception message 
                // should ONLY be done for B2C
                // reset and not any other error.
                if (ex.Message.Contains("AADB2C90118"))
                    OnPasswordReset();
                // Alert if any exception excludig user cancelling sign-in dialog
                else if (((ex as MsalException)?.ErrorCode != "authentication_canceled"))
                    await DisplayAlert($"Exception:", ex.ToString(), "Dismiss");
            }
        }

       private IAccount GetAccountByPolicy(IEnumerable<IAccount> accounts, string policy)
        {
            foreach (var account in accounts)
            {
                string userIdentifier = account.HomeAccountId.ObjectId.Split('.')[0];
                if (userIdentifier.EndsWith(policy.ToLower())) return account;
            }

            return null;
        }

        private string Base64UrlDecode(string s)
        {
            s = s.Replace('-', '+').Replace('_', '/');
            s = s.PadRight(s.Length + (4 - s.Length % 4) % 4, '=');
            var byteArray = Convert.FromBase64String(s);
            var decoded = Encoding.UTF8.GetString(byteArray, 0, byteArray.Count());
            return decoded;
        }

        public void UpdateUserInfo(AuthenticationResult ar)
        {
            JObject user = ParseIdToken(ar.IdToken);
            lblName.Text = user["name"]?.ToString();
            lblId.Text = user["oid"]?.ToString();
        }

        JObject ParseIdToken(string idToken)
        {
            // Get the piece with actual user info
            idToken = idToken.Split('.')[1];
            idToken = Base64UrlDecode(idToken);
            return JObject.Parse(idToken);
        }

        async void OnCallApi(object sender, EventArgs e)
        {
            IEnumerable<IAccount> accounts = await App.PCA.GetAccountsAsync();
            try
            {
                lblApi.Text = $"Calling API {App.ApiEndpoint}";
                AuthenticationResult ar = await App.PCA.AcquireTokenSilentAsync(App.Scopes, GetAccountByPolicy(accounts, App.PolicySignUpSignIn), App.Authority, false);
                string token = ar.AccessToken;

                // Get data from API
                HttpClient client = new HttpClient();
                HttpRequestMessage message = new HttpRequestMessage(HttpMethod.Get, App.ApiEndpoint);
                message.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                HttpResponseMessage response = await client.SendAsync(message);
                string responseString = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode)
                {
                    lblApi.Text = $"Response from API {App.ApiEndpoint} | {responseString}";
                }
                else
                {
                    lblApi.Text = $"Error calling API {App.ApiEndpoint} | {responseString}";
                }
            }
            catch (MsalUiRequiredException ex)
            {
                await DisplayAlert($"Session has expired, please sign out and back in.", ex.ToString(), "Dismiss");
            }
            catch (Exception ex)
            {
                await DisplayAlert($"Exception:", ex.ToString(), "Dismiss");
            }
        }

        async void OnEditProfile(object sender, EventArgs e)
        {
            IEnumerable<IAccount> accounts = await App.PCA.GetAccountsAsync();
            try
            {
                // KNOWN ISSUE:
                // User will get prompted 
                // to pick an IdP again.
                AuthenticationResult ar = await App.PCA.AcquireTokenAsync(App.Scopes, GetAccountByPolicy(accounts, App.PolicyEditProfile), UIBehavior.NoPrompt, string.Empty, null, App.AuthorityEditProfile, App.UiParent);
                UpdateUserInfo(ar);
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
                AuthenticationResult ar = await App.PCA.AcquireTokenAsync(App.Scopes, (IAccount)null, UIBehavior.NoPrompt, string.Empty, null, App.AuthorityPasswordReset, App.UiParent);
                UpdateUserInfo(ar);
            }
            catch (Exception ex)
            {
                // Alert if any exception excludig user cancelling sign-in dialog
                if (((ex as MsalException)?.ErrorCode != "authentication_canceled"))
                    await DisplayAlert($"Exception:", ex.ToString(), "Dismiss");
            }
        }

       private void UpdateSignInState(bool isSignedIn)
        {
            btnSignInSignOut.Text = isSignedIn ? "Sign out" : "Sign in";
            btnEditProfile.IsVisible = isSignedIn;
            btnCallApi.IsVisible = isSignedIn;
            slUser.IsVisible = isSignedIn;
            lblApi.Text = "";
        }
    }
}
