using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Text;
using TaskClient;
using TaskClient.iOS;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(TasksPage), typeof(TasksPageRenderer))]
namespace TaskClient.iOS
{
    class TasksPageRenderer : PageRenderer
    {
        TasksPage page;
        protected override void OnElementChanged(VisualElementChangedEventArgs e)
        {
            base.OnElementChanged(e);
            page = e.NewElement as TasksPage;
        }
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            page.platformParameters = new PlatformParameters(this);
        }
    }
}

   
  