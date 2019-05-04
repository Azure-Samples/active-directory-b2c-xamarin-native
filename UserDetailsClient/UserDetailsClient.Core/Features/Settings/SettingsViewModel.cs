using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using UserDetailsClient.Core.Features.Home;
using UserDetailsClient.Core.Framework;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace UserDetailsClient.Core.Features.Settings
{
    public class SettingsViewModel : BaseViewModel
    {
        public string UserIdentifier => AuthenticationService.GetCurrentContext().UserIdentifier;
        public bool ForceAutomaticLogin
        {
            get
            {
                return Preferences.Get(nameof(ForceAutomaticLogin), DefaultSettings.ForceAutomaticLogin);
            }
            set
            {
                Preferences.Set(nameof(ForceAutomaticLogin), value);
                SetAndRaisePropertyChanged(nameof(ForceAutomaticLogin));
            }
        }

        public ICommand LoadCommand => new AsyncCommand(_ => LoadDataAsync());

        public SettingsViewModel()
        {
            //MessagingCenter.Subscribe<HomeViewModel>(this, HomeViewModel.AuthenticationActionCompletedMessage, _ => LoadCommand.Execute(null));
        }

        public override async Task InitializeAsync()
        {
            await base.InitializeAsync();

        }

        private async Task LoadDataAsync()
        {
        }
    }
}