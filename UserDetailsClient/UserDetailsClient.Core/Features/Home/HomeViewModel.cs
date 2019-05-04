using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using UserDetailsClient.Core.Features.Settings;
using UserDetailsClient.Core.Features.Shell;
using UserDetailsClient.Core.Framework;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace UserDetailsClient.Core.Features.Home
{
    public class HomeViewModel : BaseViewModel
    {
        public const string AuthenticationActionCompletedMessage = nameof(AuthenticationActionCompletedMessage);

        public bool IsNoOneLoggedIn => !AuthenticationService.IsAnyOneLoggedOn;
        public string UserIdentifier => AuthenticationService.GetCurrentContext().UserIdentifier;

        public ICommand LoadCommand => new AsyncCommand(_ => LoadDataAsync());

        public HomeViewModel()
        {
            IsBusy = true;

            MessagingCenter.Subscribe<AppShellViewModel>(this, AppShellViewModel.AuthenticationRequestedMessage, _ => OnAuthenticationActionRequested(true));
        }

        public override async Task InitializeAsync()
        {
            await base.InitializeAsync();

            var forceAutomaticLogin = Preferences.Get(nameof(DefaultSettings.ForceAutomaticLogin), DefaultSettings.ForceAutomaticLogin);
            if (forceAutomaticLogin)
            {
                // auto-login the user
                await OnAuthenticationActionRequested(false);
            }
        }

        private async Task LoadDataAsync()
        {
            // force the UI to update the data bindings
            this.SetAndRaisePropertyChanged("UserIdentifier");
        }

        private async Task OnAuthenticationActionRequested(bool allowLogOff)
        {
            if (IsNoOneLoggedIn)
            {
                await AuthenticationService.SignInAsync();
            }
            else
            {
                if(allowLogOff)
                {
                    await AuthenticationService.SignOutAsync();
                }
            }

            LoadCommand.Execute(null);
            MessagingCenter.Send(this, AuthenticationActionCompletedMessage);
            IsBusy = false;
        }
    }
}