using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace TaskClient
{
    public partial class WelcomePage : ContentPage
    {
        // TODO - add PageRenderers on iOS, Android
        public IPlatformParameters platformParameters { get; set; }
        

        public WelcomePage()
        {
            InitializeComponent();           
        }
        protected override async void OnAppearing()
        {
            App.PCApplication.PlatformParameters = platformParameters;
        }
            // TODO - can I combine those and select policy on sender?
            async void OnSignUp(object sender, EventArgs e)
        {            
            try
            {
                AuthenticationResult ar = await App.PCApplication.AcquireTokenAsync(App.Scopes, "", UiOptions.SelectAccount, string.Empty, null, App.Authority, App.SignUppolicy);
                Navigation.PushAsync(new TasksPage());
            }
            catch (Exception ee)
            {
                DisplayAlert("An error has occurred", "Exception message: " + ee.Message, "Dismiss");
            }            
        }
        async void OnSignIn(object sender, EventArgs e)
        {
            try
            {
                AuthenticationResult ar = await App.PCApplication.AcquireTokenAsync(App.Scopes, "", UiOptions.SelectAccount, string.Empty, null, App.Authority, App.SignInpolicy);
                Navigation.PushAsync(new TasksPage());
            }
            catch (Exception ee)
            {
                DisplayAlert("An error has occurred", "Exception message: " + ee.Message, "Dismiss");
            }
        }
    }
}
