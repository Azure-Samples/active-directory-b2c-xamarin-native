---
services: active-directory-b2c
platforms: dotnet, xamarin
author: jmprieur
---

# Integrate Azure AD B2C into a Xamarin forms app using MSAL
This is a simple Xamarin Forms app showcasing how to use MSAL to authenticate users via Azure Active Directory B2C, and access an ASP.NET Web API with the resulting token. For more information on Azure B2C, see the [Azure AD B2C documentation](https://docs.microsoft.com/azure/active-directory-b2c/active-directory-b2c-overview).

## How To Run This Sample

To run this sample you will need:
- Visual Studio 2017
- An Internet connection
- An Azure AD B2C tenant

If you don't have an Azure AD B2C tenant, you can follow [those instructions](https://azure.microsoft.com/documentation/articles/active-directory-b2c-get-started/) to create one. 
If you just want to see the sample in action, you don't need to create your own tenant as the project comes with some settings associated to a test tenant and application; however it is highly recommend that you register your own app and experience going through the configuration steps below.   

### Step 1: Clone or download this repository

From your shell or command line:

```powershell
git clone https://github.com/Azure-Samples/active-directory-b2c-xamarin-native.git
```

### [OPTIONAL] Step 2: Get your own Azure AD B2C tenant

You can also modify the sample to use your own Azure AD B2C tenant.  First, you'll need to create an Azure AD B2C tenant by following [these instructions](https://azure.microsoft.com/documentation/articles/active-directory-b2c-get-started).

> *IMPORTANT*: if you choose to perform one of the optional steps, you have to perform ALL of them for the sample to work as expected.

### [OPTIONAL] Step 3: Create your own policies

This sample uses three types of policies: a unified sign-up/sign-in policy & a profile editing policy.  Create one policy of each type by following [the instructions here](https://azure.microsoft.com/documentation/articles/active-directory-b2c-reference-policies).  You may choose to include as many or as few identity providers as you wish.

If you already have existing policies in your Azure AD B2C tenant, feel free to re-use those.  No need to create new ones just for this sample.

### [OPTIONAL] Step 4: Create your own Web API

This sample calls an API at https://fabrikamb2chello.azurewebsites.net which has the same code as the sample [Node.js Web API with Azure AD B2C](https://github.com/Azure-Samples/active-directory-b2c-javascript-nodejs-webapi). You'll need your own API or at the very least, you'll need to [register a Web API with Azure AD B2C](https://docs.microsoft.com/azure/active-directory-b2c/active-directory-b2c-app-registration#register-a-web-api) so that you can define the scopes that your single page application will request access tokens for. 

Your web API registration should include the following information:

- Enable the **Web App/Web API** setting for your application.
- Set the **Reply URL** to the appropriate value indicated in the sample or provide any URL if you're only doing the web api registration, for example `https://myapi`.
- Make sure you also provide a **AppID URI**, for example `demoapi`, this is used to construct the scopes that are configured in you single page application's code.
- (Optional) Once you're app is created, open the app's **Published Scopes** blade and add any extra scopes you want.
- Copy the **AppID URI** and **Published Scopes values**, so you can input them in your application's code.

### [OPTIONAL] Step 5: Create your own Native app

Now you need to [register your native app in your B2C tenant](https://docs.microsoft.com/azure/active-directory-b2c/active-directory-b2c-app-registration#register-a-mobilenative-application), so that it has its own Application ID. Don't forget to grant your application API Access to the web API you registered in the previous step.

Your native application registration should include the following information:

- Enable the **Native Client** setting for your application.
- Once your app is created, open the app's **Properties** blade and set the **Custom Redirect URI** for your app to `msal<Application Id>://auth`.
- Once your app is created, open the app's **API access** blade and **Add** the API you created in the previous step.
- Copy the Application ID generated for your application, so you can use it in the next step.

### [OPTIONAL] Step 6: Configure the Visual Studio project with your app coordinates

1. Open the solution in Visual Studio.
1. Open the `UserDetailsClient\App.cs` file.
1. Find the assignment for `public static string Tenant` and replace the value with your tenant name.
1. Find the assignment for `public static string ClientID` and replace the value with the Application ID from Step 2.
1. Find the assignment for each of the policies `public static string PolicyX` and replace the names of the policies you created in Step 3.
1. Find the assignment for the scopes `public static string[] Scopes` and replace the scopes with those you created in Step 4.

#### [OPTIONAL] Step 6a: Configure the iOS project with your app's return URI
 1. Open the `UserDetailsClient.iOS\info.plist` file in a text editor (opening it in Visual Studio won't work for this step as you need to edit the text)
 1. In the URL types, section, add an entry for the authorization schema used in your redirectUri.
 ```xml
 <key>CFBundleURLSchemes</key>
 <array>
  <string>msal[APPLICATIONID]</string>
 </array>
 ```
 where `[APPLICATIONID]` is the identifier you copied in step 2. Save the file.
 
 #### [OPTIONAL] Step 6b: Configure the Android project with your app's return URI
 
 1. Open the `UserDetailsClient.Droid\Properties\AndroidManifest.xml`
 1. Add or modify the `<application>` element as in the following
 ```xml
<application>
  <activity android:name="microsoft.identity.client.BrowserTabActivity">
    <intent-filter>
      <action android:name="android.intent.action.VIEW" />
      <category android:name="android.intent.category.DEFAULT" />
      <category android:name="android.intent.category.BROWSABLE" />
      <data android:scheme="msal[APPLICATIONID]" android:host="auth" />
    </intent-filter>
  </activity>
</application>
 ```
 where `[APPLICATIONID]` is the identifier you copied in step 2. Save the file.

### Step 7: Run the sample

1. Choose the platform you want to work on by setting the startup project in the Solution Explorer. Make sure that your platform of choice is marked for build and deploy in the Configuration Manager.
1. Clean the solution, rebuild the solution, and run it.
1. Click the sign-in button at the bottom of the application screen. The sample works exactly in the same way regardless of the account type you choose, apart from some visual differences in the authentication and consent experience. Upon successful sign in, the application screen will list some basic profile info for the authenticated user and show buttons that allow you to edit your profile, call an API and sign out.
1. Close the application and reopen it. You will see that the app retains access to the API and retrieves the user info right away, without the need to sign in again.
1. Sign out by clicking the Sign out button and confirm that you lose access to the API until the exit interactive sign in.  

#### Running in an Android Emulator

MSAL in Android requires support for Custom Chrome Tabs for displaying authentication prompts.
Not every emulator image comes with Chrome on board: please refer to [this document](https://github.com/Azure-Samples/active-directory-general-docs/blob/master/AndroidEmulator.md) for instructions on how to ensure that your emulator supports the features required by MSAL. 
 
## About the code

The structure of the solution is straightforward. All the application logic and UX reside in UserDetailsClient (portable).
MSAL's main primitive for native clients, `PublicClientApplication`, is initialized as a static variable in App.cs.
At application startup, the main page attempts to get a token without showing any UX - just in case a suitable token is already present in the cache from previous sessions. This is the code performing that logic:

```csharp
protected override async void OnAppearing()
{
    UpdateSignInState(false);

    // Check to see if we have a User in the cache already.
    try
    {
        AuthenticationResult ar = await App.PCA.AcquireTokenSilentAsync(App.Scopes, GetUserByPolicy(App.PCA.Users, App.PolicySignUpSignIn), App.Authority, false);
        UpdateUserInfo(ar);
        UpdateSignInState(true);
    }
    catch (Exception)
    {
        // Doesn't matter, we go in interactive mode
        UpdateSignInState(false);
    }
}
```

If the attempt to obtain a token silently fails, we do nothing and display the screen with the sign in button.
When the sign in button is pressed, we execute the same logic - but using a method that shows interactive UX:

```csharp
AuthenticationResult ar = await App.PCA.AcquireTokenAsync(App.Scopes, GetUserByPolicy(App.PCA.Users, App.PolicySignUpSignIn), App.UiParent);
```
The `Scopes` parameter indicates the permissions the application needs to gain access to the data requested through subsequent web API call (in this sample, encapsulated in `OnCallApi`). 
The UiParent is used in Android to tie the authentication flow to the current activity, and is ignored on all other platforms. For more platform specific considerations, please see below.

The sign out logic is very simple. In this sample we have just one user, however we are demonstrating a more generic sign out logic that you can apply if you have multiple concurrent users and you want to clear up the entire cache.               
```csharp
foreach (var user in App.PCA.Users)
{
    App.PCA.Remove(user);
}
```

### Android specific considerations

The platform specific projects require only a couple of extra lines to accommodate for individual platform differences.

UserDetailsClient.Droid requires one two extra lines in the `MainActivity.cs` file.
In `OnActivityResult`, we need to add

```csharp
AuthenticationContinuationHelper.SetAuthenticationContinuationEventArgs(requestCode, resultCode, data);

```
That line ensures that the control goes back to MSAL once the interactive portion of the authentication flow ended.

In `OnCreate`, we need to add the following assignment:
```csharp
App.UiParent = new UIParent(Xamarin.Forms.Forms.Context as Activity); 
```
That code ensures that the authentication flows occur in the context of the current activity.  


### iOS specific considerations

UserDetailsClient.iOS only requires one extra line, in AppDelegate.cs.
You need to ensure that the OpenUrl handler looks as ine snippet below:
```csharp
public override bool OpenUrl(UIApplication app, NSUrl url, NSDictionary options)
{
    AuthenticationContinuationHelper.SetAuthenticationContinuationEventArgs(url, "");
    return true;
}
```
Once again, this logic is meant to ensure that once the interactive portion of the authentication flow is concluded, the flow goes back to MSAL.

## More information

For more information on Azure B2C, see [the Azure AD B2C documentation homepage](http://aka.ms/aadb2c). 
