---
services: active-directory
platforms: dotnet, xamarin
author: vibronet
---
# Integrate Microsoft identity and the Microsoft Graph into a Xamarin forms app using MSAL
This is a simple Xamarin Forms app showcasing how to use MSAL to authenticate users via Azure Active Directory B2C, and access an ASP.NET Web API with the resulting token.

For more information on Azure B2C, see [the Azure AD B2C documentation homepage](https://azure.microsoft.com/en-us/documentation/services/active-directory-b2c/).

## How To Run This Sample

To run this sample you will need:
- Visual Studio 2015
- An Internet connection
- An Azure AD B2C tenant

If you don't have an Azure AD B2C tenant, you can follow [those instructions](https://azure.microsoft.com/en-us/documentation/articles/active-directory-b2c-get-started/) to create one. 
If you just want to see the sample in action, you don't need to create your own tenant as the project comes with some settings associated to a test tenant and application; however it is highly recommend that you register your own app and experience going through the configuration steps below.   

### Step 1:  Clone or download this repository

From your shell or command line:

`git clone https://github.com/Azure-Samples/active-directory-b2c-xamarin-native.git`

### Step 2: [OPTIONAL] Create an Azure AD B2C application 

You can run the sample as is with its current settings, or you can optionally register it as a new application under your own developer account. Creating your own app is highly recommended.

> *IMPORTANT*: if you choose to perform one of the optional steps, you have to perform ALL of them for the sample to work as expected.

You can find detailed instructions on how to create a new app on [this page](https://azure.microsoft.com/en-us/documentation/articles/active-directory-b2c-app-registration/) Make sure to:

- Copy down the **Application Id** assigned to your app, you'll need it in the next optional steps.
- Include both a **web app/web API** and a **Native Client**in your app.

### Step 3: [OPTIONAL] Create a "Sign up or Sign In" policy
This sample requires your B2C app to have a policy of type .
You can follow the instructions in [this tutorial](https://azure.microsoft.com/en-us/documentation/articles/active-directory-b2c-reference-policies/#how-to-create-a-sign-up-policy) to create one.
Please note: the sample code expects the policy to be named `"B2C_1_B2C_Signup_Signin_Policy"`. You can choose to name your policy with a different identifier, however in that case you need to go to the file TaskClient/App.cs, locate the assignment for `public static string SignUpSignInpolicy` and make sure that you substitute the default string with your own policy name.
### Step 4: [OPTIONAL] Configure the mobile app
1. Open the solution in Visual Studio 2015.
2. Open the `TaskClient\App.cs` file.
3. Find the assignment for `public static string ClientID` and replace the value with the Application ID from the app registration portal from Step 2.
4. Find the assignment for `Authority` one line below and change it to point to your Azure AD B2C tenant. 
5. Find the assignment for `APIbaseURL` one line below and change it to point to the URL at which you plan to host the Web API project from the sample. If you decide to host it on IIS Express, you can use the value assigned to the TaskService project - `"https://localhost:44332/"`. 

### Step 5: [OPTIONAL] Configure the web API
1. Open the solution in Visual Studio 2015.
2. Open the `TaskService\web.config` file.
3. Find the `appSettings` entry for `ida:Tenant` and replace the value with your Azure AD B2C tenant.
4. Find the `appSettings` entry for `ida:ClientId` and replace the value with the Application ID from the app registration portal from Step 2.
5. Find the `appSettings` entry for `PolicyId` and replace the identifier of your policy.

### Step 6:  Run the solution

Choose the platform you want to work on by setting the startup project in the Solution Explorer. Make sure that your platform of choice is marked for build and deploy in the Configuration Manager.
Clean the solution, rebuild the solution, and run it.
Click the Sign in or Sign up button at the bottom of the application screen. On the credentials gathering screen, choose sign up and go through the sign up experience. 
As soon as you successfully sign up, you will be transported to a screen where you can create new tasks. Also, the button at the bottom of the screen will turn into a Sign out button. 
Create some random tasks, then close the application and reopen it. You will see that the app retains access to the API and retrieves the tasks list right away, without the need to sign in again.
Sign out by clicking the Sign out button and confirm that you lose access to the API until the next interactive sign in. Note that now that you signed your test user up, you will be able to choose the sign in path at authentication experience time.


> *IMPORTANT*: If you are hosting your own TaskService instance on IIS Express, please be aware of the fact that the devices (real or emulated) you'll use to test the sample will have to trust the SSL certificate used by IIS Express for its HTTPS endpoints. An easy way to work around this issue is to deploy TaskService in your own Azure website. If you publish from Visual Studio, please remember to UNCHECK the "Enable Organizational Authentication" checkbox.  

> *IMPORTANT*: If you are using the sample with the settings that come pre-populated, please be aware that 
> 1. there is no guarantee that the sample will work at all times. The only way in which you can be certain to see the sample working is to follow the instructions and register the app in your own tenant.
> 2. user sign up information will be periodically wiped.
> 3. The test TaskService maintains a task list in memory, and as such it will be frequently cleared.

## Deploy this sample to Azure
Coming soon...
## About the code
The structure of the solution is straightforward. All the application logic and UX reside in TaskClient (portable).
TaskClient.Droid and TaskClient.iOS both include a WelcomePageRenderer and a TasksPageRenderer class, both used for assigning values at runtime to the `PlatformParameters` property of the paged. The `PlatformParameters` construct is used by MSAL for understanding at runtime on which platform it is running  - so that it can select the right authentication UX and token storage. Please note, MSAL does not need `PlatformParameters` for UWP apps. Also, technically TaskPage would not need a renderer either, given that in this sample that page never triggers interactive authentication. However we included one in case you want to add that functionality.
TaskClient.Droid requires one extra line of code to be added in the MainActivity.cs file:

```
AuthenticationAgentContinuationHelper.SetAuthenticationAgentContinuationEventArgs(requestCode, resultCode, data);

```
That line is used in `OnActivityResult` to ensure that the control goes back to MSAL once the interactive portion of the authentication flow ended.

The MSAL main primitive, `PublicClientApplication`, is initialized as a static variable in App.cs.
At application startup, WelcomePage attempts to get a token without showing any UX - just in case a suitable token is already present in the cache from previous sessions. This is the code performing that logic:
```
protected override async void OnAppearing()
{
    App.PCA.PlatformParameters = platformParameters;
    // let's see if we have a user in our belly already
    try
    {
        AuthenticationResult ar = await App.PCApplication.AcquireTokenSilentAsync(App.Scopes, "", App.Authority, App.SignUpSignInpolicy, false);
        Navigation.PushAsync(new TasksPage());
    }
```
Note that this code is also used for assigning the previously mentioned MSAL's `PlatformParameters ` with the `platformParameters` value, itself assigned by the platform specific PageRenderer as discussed. 
If the attempt to obtain a token silently fails, we do nothing and display the screen with the sign in/sign up button. If the attempt is successful, we navigate directly to TasksPage.
When the sign in/sign up button is pressed, we execute the same logic - but using a method that shows interactive UX:

```
AuthenticationResult ar = await App.PCApplication.AcquireTokenAsync(App.Scopes, "", UiOptions.SelectAccount, string.Empty, null, App.Authority, App.SignUpSignInpolicy);
```

The TasksPage attempts a silent token acquisition directly at load time - and given that the app navigates ot this page only upon a successful sign in/up or token acquisition, normally the operation will succeed.  
Following that, the page uses the token just obtained to secure a call to the web API, which results in a list of tasks.
The task creation logic works similarly.

The sign out logic is very simple.                
```
App.PCApplication.UserTokenCache.Clear(App.PCApplication.ClientId);
```

On the service side, the most interesting parts of TaskService are the Startup.Auth.cs ad the OpenIdConnectCachingSecurityTokenProvider.cs files.
OpenIdConnectCachingSecurityTokenProvider.cs contains specialized logic which tailors to the Azure AD B2C policy-driven metadata the default logic used to retrieve the data required to validate incoming tokens.
Startup.Auth.cs contains logic that initializes the OAuthBearerAuthentication middleware with the custom logic described above and with the application coordinates from the web.config file.  

## More information
For more information on Azure B2C, see [the Azure AD B2C documentation homepage](https://azure.microsoft.com/en-us/documentation/services/active-directory-b2c/).
