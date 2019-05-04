using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using UserDetailsClient.Core.Features.Logging;
using Xamarin.Forms;

namespace UserDetailsClient.Core.Features.LogOn
{
    public class ROPCAuthenticationService
    {
        private readonly ILoggingService loggingService;

        public ROPCAuthenticationService()
        {
            loggingService = DependencyService.Get<ILoggingService>();
        }

        public bool IsAnyOneLoggedOn => currentContext != null;

        private UserContext currentContext;
        public UserContext GetUserContext()
        {
            return currentContext;
        }

        public async Task LogOnAsync(string email, string password)
        {
            var url = "https://" + B2CConstants.AzureADB2CHostname + "/" + B2CConstants.Authority + "/oauth2/v2.0/token?p=" + B2CConstants.PolicyRPOC;
            var uri = new Uri(url);

            StringBuilder bodyBuilder = new StringBuilder();

            bodyBuilder.Append("username=");
            bodyBuilder.Append(email);
            bodyBuilder.Append("&");

            bodyBuilder.Append("password=");
            bodyBuilder.Append(password);
            bodyBuilder.Append("&");

            bodyBuilder.Append("grant_type=");
            bodyBuilder.Append("password");
            bodyBuilder.Append("&");

            bodyBuilder.Append("scope=");
            bodyBuilder.Append("openid " + B2CConstants.ROPC_APP_ID + " offline_access");
            bodyBuilder.Append("&");

            bodyBuilder.Append("client_id=");
            bodyBuilder.Append(B2CConstants.ROPC_APP_ID);
            bodyBuilder.Append("&");

            bodyBuilder.Append("response_type=");
            bodyBuilder.Append("token id_token");

            string body = bodyBuilder.ToString();
            var content = new StringContent(body, Encoding.UTF8, "application/x-www-form-urlencoded");

            HttpClient client = new HttpClient();
            var response = await client.PostAsync(uri, content);

            var responseContent = await response.Content.ReadAsStringAsync();

            loggingService.Debug(responseContent);

            JObject responseObject = JObject.Parse(responseContent);

            var accessToken = responseObject["access_token"].Value<string>();
            var idToken = responseObject["id_token"].Value<string>();

            var jwtToken = new JwtSecurityToken(idToken);

            loggingService.Debug(accessToken);

            var userId = jwtToken.Payload["oid"].ToString();
            var givenName = jwtToken.Payload["given_name"].ToString();
            var familyName = jwtToken.Payload["family_name"].ToString();
            var state = jwtToken.Payload["state"].ToString();
            var country = jwtToken.Payload["country"].ToString();
            var postalCode = jwtToken.Payload["postalCode"].ToString();
            var emails = jwtToken.Payload["emails"] as JArray;
            var primaryEmail = emails[0].ToString();

            var newContext = new UserContext();
            newContext.UserIdentifier = userId;
            newContext.GivenName = givenName;
            newContext.FamilyName = familyName;
            newContext.Province = state;
            newContext.PostalCode = postalCode;
            newContext.Country = country;
            newContext.EMailAddress = primaryEmail;

            this.currentContext = newContext;
        }
    }
}