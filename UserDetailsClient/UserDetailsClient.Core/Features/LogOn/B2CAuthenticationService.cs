using Microsoft.Identity.Client;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserDetailsClient.Core.Features.LogOn
{
    public class B2CAuthenticationService : IAuthenticationService
    {
        public IPublicClientApplication PCA = null;

        public object ParentActivityOrWindow { get; set; }

        public B2CAuthenticationService()
        {
            // default redirectURI; each platform specific project will have to override it with its own
            PCA = PublicClientApplicationBuilder.Create(B2CConstants.ClientID)
                .WithB2CAuthority(B2CConstants.Authority)
                .WithIosKeychainSecurityGroup("com.microsoft.adalcache")
                .WithRedirectUri($"msal{B2CConstants.ClientID}://auth")
                .Build();
        }

        public async Task<string> AcquireToken()
        {
            IEnumerable<IAccount> accounts = await PCA.GetAccountsAsync();
            AuthenticationResult ar = await PCA.AcquireTokenSilent(B2CConstants.Scopes, GetAccountByPolicy(accounts, B2CConstants.PolicySignUpSignIn))
                .WithB2CAuthority(B2CConstants.Authority)
               .ExecuteAsync();
            string token = ar.AccessToken;
            return token;
        }

        public async Task<UserContext> ResetPassword()
        {
            AuthenticationResult ar = await PCA.AcquireTokenInteractive(B2CConstants.Scopes)
                .WithPrompt(Prompt.NoPrompt)
                .WithAuthority(B2CConstants.AuthorityPasswordReset)
                .WithParentActivityOrWindow(ParentActivityOrWindow)
                .ExecuteAsync();

            var userContext = UpdateUserInfo(ar);

            return userContext;
        }

        public async Task<UserContext> EditProfile()
        {
            IEnumerable<IAccount> accounts = await PCA.GetAccountsAsync();
            // KNOWN ISSUE:
            // User will get prompted 
            // to pick an IdP again.
            AuthenticationResult ar = await PCA.AcquireTokenInteractive(B2CConstants.Scopes)
                .WithAccount(GetAccountByPolicy(accounts, B2CConstants.PolicyEditProfile))
                .WithPrompt(Prompt.NoPrompt)
                .WithAuthority(B2CConstants.AuthorityEditProfile)
                .WithParentActivityOrWindow(ParentActivityOrWindow)
                .ExecuteAsync();

            var userContext = UpdateUserInfo(ar);

            return userContext;
        }

        public async Task<UserContext> SignIn()
        {
            IEnumerable<IAccount> accounts = await PCA.GetAccountsAsync();
            AuthenticationResult ar = await PCA.AcquireTokenInteractive(B2CConstants.Scopes)
                .WithAccount(GetAccountByPolicy(accounts, B2CConstants.PolicySignUpSignIn))
                .WithParentActivityOrWindow(ParentActivityOrWindow)
                .ExecuteAsync();

            var newContext = UpdateUserInfo(ar);
            return newContext;
        }

        public async Task SignOut()
        {
            IEnumerable<IAccount> accounts = await PCA.GetAccountsAsync();
            while (accounts.Any())
            {
                await PCA.RemoveAsync(accounts.FirstOrDefault());
                accounts = await PCA.GetAccountsAsync();
            }
            //UpdateSignInState(false);
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

        public UserContext UpdateUserInfo(AuthenticationResult ar)
        {
            var newContext = new UserContext();
            newContext.IsLoggedOn = false;
            JObject user = ParseIdToken(ar.IdToken);
            newContext.Name = user["name"]?.ToString();
            newContext.UserIdentifier = user["oid"]?.ToString();
            newContext.IsLoggedOn = true;

            return newContext;
        }

        JObject ParseIdToken(string idToken)
        {
            // Get the piece with actual user info
            idToken = idToken.Split('.')[1];
            idToken = Base64UrlDecode(idToken);
            return JObject.Parse(idToken);
        }

        public void SetParent(object parent)
        {
            ParentActivityOrWindow = parent;
        }
    }
}