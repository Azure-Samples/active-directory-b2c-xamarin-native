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
            PCA = PublicClientApplicationBuilder.Create(B2CConstants.ClientId)
                .WithB2CAuthority(B2CConstants.AuthoritySignInSignUp)
                .WithIosKeychainSecurityGroup(B2CConstants.IosKeyChainGroup)
                .WithRedirectUri(B2CConstants.RedirectUri)
                .Build();
        }

        public async Task<UserContext> SignInAsync()
        {
            UserContext newContext = null;
            try
            {
                // acquire token silent
                newContext = await AcquireTokenSilent();
            }
            catch (MsalUiRequiredException ex)
            {
                // acquire token interactive
                newContext = await SignInInteractively();
            }
            return newContext;
        }

        private async Task<UserContext> AcquireTokenSilent()
        {
            IEnumerable<IAccount> accounts = await PCA.GetAccountsAsync();
            AuthenticationResult authResult =
                await PCA.AcquireTokenSilent(B2CConstants.Scopes, GetAccountByPolicy(accounts, B2CConstants.PolicySignUpSignIn))
               .WithB2CAuthority(B2CConstants.AuthoritySignInSignUp)
               .ExecuteAsync();

            var newContext = UpdateUserInfo(authResult);
            return newContext;
        }

        public async Task<UserContext> ResetPasswordAsync()
        {
            AuthenticationResult authResult = await PCA.AcquireTokenInteractive(B2CConstants.Scopes)
                .WithPrompt(Prompt.NoPrompt)
                .WithAuthority(B2CConstants.AuthorityPasswordReset)
                .WithParentActivityOrWindow(ParentActivityOrWindow)
                .ExecuteAsync();

            var userContext = UpdateUserInfo(authResult);

            return userContext;
        }

        public async Task<UserContext> EditProfileAsync()
        {
            AuthenticationResult authResult = await PCA.AcquireTokenInteractive(B2CConstants.Scopes)
                .WithPrompt(Prompt.NoPrompt)
                .WithAuthority(B2CConstants.AuthorityEditProfile)
                .WithParentActivityOrWindow(ParentActivityOrWindow)
                .ExecuteAsync();

            var userContext = UpdateUserInfo(authResult);

            return userContext;
        }

        private async Task<UserContext> SignInInteractively()
        {
            AuthenticationResult authResult = await PCA.AcquireTokenInteractive(B2CConstants.Scopes)
                .WithParentActivityOrWindow(ParentActivityOrWindow)
                .ExecuteAsync();

            var newContext = UpdateUserInfo(authResult);
            return newContext;
        }

        public async Task<UserContext> SignOutAsync()
        {
            IEnumerable<IAccount> accounts = await PCA.GetAccountsAsync();

            while (accounts.Any())
            {
                await PCA.RemoveAsync(accounts.FirstOrDefault());
                accounts = await PCA.GetAccountsAsync();
            }

            var signedOutContext = new UserContext();
            signedOutContext.IsLoggedOn = false;
            return signedOutContext;
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

            newContext.AccessToken = ar.AccessToken;
            newContext.Name = user["name"]?.ToString();
            newContext.UserIdentifier = user["oid"]?.ToString();

            newContext.GivenName = user["given_name"]?.ToString();
            newContext.FamilyName = user["family_name"]?.ToString();

            newContext.StreetAddress = user["streetAddress"]?.ToString();
            newContext.City = user["city"]?.ToString();
            newContext.Province = user["state"]?.ToString();
            newContext.PostalCode = user["postalCode"]?.ToString();
            newContext.Country = user["country"]?.ToString();

            newContext.JobTitle = user["jobTitle"]?.ToString();

            var emails = user["emails"] as JArray;
            if (emails != null)
            {
                newContext.EmailAddress = emails[0].ToString();
            }
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