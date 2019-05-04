using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using UserDetailsClient.Core.Features.Logging;
using UserDetailsClient.Core.Features.LogOn;
using Xamarin.Forms;

namespace UserDetailsClient.Core.Framework
{
    public class BaseViewModel : INotifyPropertyChanged
    {
        protected readonly IAuthenticationService AuthenticationService;
        protected readonly ILoggingService LoggingService;

        private bool isBusy;
        public bool IsBusy
        {
            get => isBusy;
            set => SetAndRaisePropertyChanged(ref isBusy, value);
        }

        public BaseViewModel()
        {
            AuthenticationService = DependencyService.Get<IAuthenticationService>();
            LoggingService = DependencyService.Get<ILoggingService>();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public virtual Task InitializeAsync() => Task.CompletedTask;

        public virtual Task UninitializeAsync() => Task.CompletedTask;

        protected static async Task ShowFeatureNotAvailableAsync()
        {
            await Application.Current.MainPage.DisplayAlert(
                "Feature Not Available",
                "Feature Not Available",
                "OK");
        }

        protected void SetAndRaisePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        protected void SetAndRaisePropertyChanged<TRef>(ref TRef field, TRef value, [CallerMemberName]string propertyName = null)
        {
            field = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        protected void SetAndRaisePropertyChangedIfDifferentValues<TRef>(ref TRef field, TRef value, [CallerMemberName] string propertyName = null)
            where TRef : class
        {
            if (field == null || !field.Equals(value))
            {
                SetAndRaisePropertyChanged(ref field, value, propertyName);
            }
        }
    }
}