using Microsoft.Owin.Security.ActiveDirectory;
using Microsoft.Owin.Security.Jwt;
using Microsoft.Owin.Security.OAuth;
using Owin;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IdentityModel.Tokens;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskService.App_Start;

namespace TaskService
{
    public partial class Startup
    {
        // These values are pulled from web.config
        public static string aadAuthority = ConfigurationManager.AppSettings["ida:AadAuthority"];
        public static string tenant = ConfigurationManager.AppSettings["ida:Tenant"];
        public static string clientId = ConfigurationManager.AppSettings["ida:ClientId"];
        public static string commonPolicy = ConfigurationManager.AppSettings["ida:PolicyId"];
        private const string discoverySuffix = ".well-known/openid-configuration";

        public void ConfigureAuth(IAppBuilder app)
        {   
            TokenValidationParameters tvps = new TokenValidationParameters
            {
                // This is where you specify that your API only accepts tokens from its own clients
                ValidAudience = clientId,
            };
            app.Use(async (Context, next) =>
            {
                await next.Invoke();
            });
            app.UseOAuthBearerAuthentication(new OAuthBearerAuthenticationOptions
            {   
                // This SecurityTokenProvider fetches the Azure AD B2C metadata & signing keys from the OpenIDConnect metadata endpoint
                AccessTokenFormat = new JwtFormat(tvps, new OpenIdConnectCachingSecurityTokenProvider(String.Format(aadAuthority, tenant, "v2.0", discoverySuffix, commonPolicy)))
            });
        }
    }
}
