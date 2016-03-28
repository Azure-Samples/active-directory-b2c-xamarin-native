using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Xamarin.Forms;
using TaskClient;
using TaskClient.Droid;
using Xamarin.Forms.Platform.Android;
using Microsoft.Identity.Client;

[assembly: ExportRenderer(typeof(WelcomePage), typeof(WelcomePageRenderer))]
namespace TaskClient.Droid
{
    class WelcomePageRenderer : PageRenderer
    {
        WelcomePage page;

        protected override void OnElementChanged(ElementChangedEventArgs<Page> e)
        {
            base.OnElementChanged(e);
            page = e.NewElement as WelcomePage;
            var activity = this.Context as Activity;
            page.platformParameters = new PlatformParameters(activity);
        }

    }
}