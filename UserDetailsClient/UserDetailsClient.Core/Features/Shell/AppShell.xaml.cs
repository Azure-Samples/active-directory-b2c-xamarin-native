using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace UserDetailsClient.Core.Features.Shell
{
	public partial class AppShell : Xamarin.Forms.Shell
    {
        public static readonly TimeSpan TimeFlyoutCloses = TimeSpan.FromSeconds(0.5f);

        public AppShell ()
		{
			InitializeComponent();

            BindingContext = new AppShellViewModel();
        }

        internal async Task CloseFlyoutAsync()
        {
            FlyoutIsPresented = false;
            await Task.Delay(TimeFlyoutCloses);
        }
    }
}