using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace UserDetailsClient.Core.Features.UserProfile
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class UserProfilePage
	{
		public UserProfilePage ()
		{
			InitializeComponent ();

            BindingContext = new UserProfileViewModel();
        }
	}
}