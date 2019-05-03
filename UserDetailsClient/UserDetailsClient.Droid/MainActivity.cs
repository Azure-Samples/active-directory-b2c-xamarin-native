
using Android.App;
using Android.Content.PM;
using Microsoft.Identity.Client;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;

using UserDetailsClient.Core;
using Xamarin.Forms.Platform.Android;
using Xamarin.Forms;
using UserDetailsClient.Core.Features.LogOn;

namespace UserDetailsClient.Droid
{
    [Activity(Label = "UserDetailsClient", Icon = "@drawable/icon", Theme = "@style/MyTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(bundle);

            global::Xamarin.Forms.Forms.Init(this, bundle);

            LoadApplication(new App());

            var authenticationService = DependencyService.Get<IAuthenticationService>();
            // Default system browser
            authenticationService.SetParent(this);
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            AuthenticationContinuationHelper.SetAuthenticationContinuationEventArgs(requestCode, resultCode, data);
        }
    }
}

