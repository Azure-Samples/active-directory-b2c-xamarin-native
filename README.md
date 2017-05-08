---
services: active-directory-b2c
platforms: dotnet, xamarin
author: gsacavdm
---

# Integrate Azure AD B2C into a Xamarin forms app using MSAL
This is a simple Xamarin Forms app showcasing how to use MSAL to authenticate users via Azure Active Directory B2C, and access an ASP.NET Web API with the resulting token. For more information on Azure B2C, see the [Azure AD B2C documentation](https://docs.microsoft.com/azure/active-directory-b2c/active-directory-b2c-overview).

## How To Run This Sample

To run this sample you will need:
- Visual Studio 2015
- An Internet connection
- At least one of the following accounts:
- An Azure AD B2C tenant

If you don't have an Azure AD B2C tenant, you can follow [those instructions](https://azure.microsoft.com/documentation/articles/active-directory-b2c-get-started/) to create one. 
If you just want to see the sample in action, you don't need to create your own tenant as the project comes with some settings associated to a test tenant and application; however it is highly recommend that you register your own app and experience going through the configuration steps below.   

### Step 1:  Clone or download this repository

From your shell or command line:

`git clone https://github.com/Azure-Samples/active-directory-b2c-xamarin-native.git`

### [OPTIONAL] Step 2: Create an Azure AD B2C application 

You can run the sample as is with its current settings, or you can optionally register it as a new application under your own developer account. Creating your own app is highly recommended.

> *IMPORTANT*: if you choose to perform one of the optional steps, you have to perform ALL of them for the sample to work as expected.

You can find detailed instructions on how to create a new mobile /native app on [this page](https://docs.microsoft.com/azure/active-directory-b2c/active-directory-b2c-app-registration#register-a-mobilenative-application) Make sure to:

- Copy down the **Application Id** assigned to your app, you'll need it in the next optional steps.
- Copy down the **Redirect URI** you configure for your app.

### [OPTIONAL] Step 3: Create your own policies

This sample requires your B2C app to the following policy types "Sign Up or Sign In", "Edit Profile" and "Reset Password".
You can follow the instructions in [this tutorial](https://docs.microsoft.com/azure/active-directory-b2c/active-directory-b2c-reference-policies) to create them.
Once created, replace the following value in the `UserDetailsClient/App.cs` file with your own policy name.  All B2C policies should begin with `b2c_1_`.

```C#
public static string PolicySignUpSignIn = "b2c_1_susi";
```

### [OPTIONAL] Step 4: Create your own API

PENDING

### [OPTIONAL] Step 5:  Configure the Visual Studio project with your app coordinates

1. Open the solution in Visual Studio 2015.
1. Open the `UserDetailsClient\App.cs` file.
1. Find the assignment for `public static string ClientID` and replace the value with the Application ID from Step 2.
1. Find the assignment for each of the policies `public static string PolicyX` and replace the names of the policies you created in Step 3.
1. Find the assignment for the scopes `public static string[] Scopes` and replace the scopes with those you created in Step 4.

### Step 4:  Run the sample

1. Choose the platform you want to work on by setting the startup project in the Solution Explorer. Make sure that your platform of choice is marked for build and deploy in the Configuration Manager.
1. Clean the solution, rebuild the solution, and run it.
1. Click the sign-in button at the bottom of the application screen. The sample works exactly in the same way regardless of the account type you choose, apart from some visual differences in the authentication and consent experience. Upon successful sign in, the application screen will list some basic profile info for the authenticated user and show buttons that allow you to edit your profile, call an API and sign out.
1. Close the application and reopen it. You will see that the app retains access to the API and retrieves the user info right away, without the need to sign in again.
1. Sign out by clicking the Sign out button and confirm that you lose access to the API until the exit interactive sign in.  

## More information
For more information on Azure B2C, see [the Azure AD B2C documentation homepage](http://aka.ms/aadb2c). 
