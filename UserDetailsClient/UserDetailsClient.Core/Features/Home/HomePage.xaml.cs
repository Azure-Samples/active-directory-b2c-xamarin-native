using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace UserDetailsClient.Core.Features.Home
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class HomePage 
	{
		public HomePage ()
		{
			InitializeComponent ();

            App.NavigationRoot = this;

            BindingContext = new HomeViewModel();
        }
	}
}