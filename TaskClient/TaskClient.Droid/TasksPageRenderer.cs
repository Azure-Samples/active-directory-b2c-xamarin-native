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
using Xamarin.Forms.Platform.Android;
using Xamarin.Forms;
using Microsoft.Identity.Client;
using TaskClient;
using TaskClient.Droid;

[assembly: ExportRenderer(typeof(TasksPage), typeof(TasksPageRenderer))]
namespace TaskClient.Droid
{
    class TasksPageRenderer : PageRenderer
    {
        TasksPage page;

        protected override void OnElementChanged(ElementChangedEventArgs<Page> e)
        {
            base.OnElementChanged(e);
            page = e.NewElement as TasksPage;
            var activity = this.Context as Activity;
            page.platformParameters = new PlatformParameters(activity);
        }

    }
}