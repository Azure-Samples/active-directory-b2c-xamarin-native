using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using UserDetailsClient.Core.Features.Home;
using UserDetailsClient.Core.Framework;
using Xamarin.Forms;

namespace UserDetailsClient.Core.Features.UserProfile
{
    public class UserProfileViewModel : BaseViewModel
    {
        public string UserIdentifier => AuthenticationService.GetCurrentContext().UserIdentifier;
        public string EMailAddress => AuthenticationService.GetCurrentContext().EMailAddress;

        public ICommand LoadCommand => new AsyncCommand(_ => LoadDataAsync());

        public UserProfileViewModel()
        {
            MessagingCenter.Subscribe<HomeViewModel>(this, HomeViewModel.AuthenticationActionCompletedMessage, _ => LoadCommand.Execute(null));
        }

        public override async Task InitializeAsync()
        {
            await base.InitializeAsync();

        }

        private async Task LoadDataAsync()
        {
            // force the UI to update the data bindings
            this.SetAndRaisePropertyChanged("UserIdentifier");
            this.SetAndRaisePropertyChanged("EMailAddress");
        }
    }
}