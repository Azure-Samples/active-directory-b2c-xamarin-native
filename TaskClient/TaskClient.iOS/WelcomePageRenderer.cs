using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Text;
using TaskClient;
using TaskClient.iOS;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(WelcomePage), typeof(WelcomePageRenderer))]
namespace TaskClient.iOS
{
    class WelcomePageRenderer : PageRenderer
    {
        WelcomePage page;
        protected override void OnElementChanged(VisualElementChangedEventArgs e)
        {
            base.OnElementChanged(e);
            page = e.NewElement as WelcomePage;
        }
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            page.platformParameters = new PlatformParameters(this);
        }
    }
}
