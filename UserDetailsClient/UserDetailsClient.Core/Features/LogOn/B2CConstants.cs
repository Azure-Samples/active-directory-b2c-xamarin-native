using System;
using System.Collections.Generic;
using System.Text;

namespace UserDetailsClient.Core.Features.LogOn
{
    public static class B2CConstants
    {
        // Azure AD B2C Coordinates
        public static string Tenant = "b2cDemo2019Tenant.onmicrosoft.com";
        public static string AzureADB2CHostname = "b2cDemo2019Tenant.b2clogin.com";
        public static string ClientID = "f3557bf1-2d23-4537-a79f-8569c1876fb2";
        public static string PolicySignUpSignIn = "B2C_1_signup_signin";
        public static string PolicyEditProfile = "B2C_1_edit_profile";
        public static string PolicyResetPassword = "B2C_1_password_reset";

        public static string[] Scopes = { "https://b2cDemo2019Tenant.onmicrosoft.com/api/hello.read" };

        public static string AuthorityBase = $"https://{AzureADB2CHostname}/tfp/{Tenant}/";
        public static string Authority = $"{AuthorityBase}{PolicySignUpSignIn}";
        public static string AuthorityEditProfile = $"{AuthorityBase}{PolicyEditProfile}";
        public static string AuthorityPasswordReset = $"{AuthorityBase}{PolicyResetPassword}";
        public static string IOSKeyChainGroup = "com.microsoft.adalcache";
    }
}