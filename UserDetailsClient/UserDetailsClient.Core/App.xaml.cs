using System;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Microsoft.Identity.Client;
using UserDetailsClient.Core.Features.LogOn;
using UserDetailsClient.Core.Features.Shell;
using System.Threading.Tasks;
using UserDetailsClient.Core.Features.Logging;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace UserDetailsClient.Core
{
	public partial class App : Application
    {
        public static AppShell Shell => Current.MainPage as AppShell;

        private static NavigableElement navigationRoot;
        public static NavigableElement NavigationRoot
        {
            get => GetShellSection(navigationRoot) ?? navigationRoot;
            set => navigationRoot = value;
        }

        public static string ApiEndpoint = "https://fabrikamb2chello.azurewebsites.net/hello";

        public App ()
		{
            InitializeComponent();

            RegisterServicesAndProviders();

            MainPage = new AppShell();
        }

        // It provides a navigatable section for elements which aren't explicitly defined within the Shell. For example,
        // ProductCategoryPage: it's accessed from the fly-out through a MenuItem but it doesn't belong to any section
        internal static ShellSection GetShellSection(Element element)
        {
            if (element == null)
            {
                return null;
            }

            var parent = element;
            var parentSection = parent as ShellSection;

            while (parentSection == null && parent != null)
            {
                parent = parent.Parent;
                parentSection = parent as ShellSection;
            }

            return parentSection;
        }

        internal static async Task NavigateBackAsync() => await NavigationRoot.Navigation.PopAsync();

        internal static async Task NavigateModallyBackAsync() => await NavigationRoot.Navigation.PopModalAsync();

        internal static async Task NavigateToAsync(Page page, bool closeFlyout = false)
        {
            if (closeFlyout)
            {
                await Shell.CloseFlyoutAsync();
            }

            await NavigationRoot.Navigation.PushAsync(page).ConfigureAwait(false);
        }

        internal static async Task NavigateModallyToAsync(Page page, bool animated = true)
        {
            await Shell.CloseFlyoutAsync();
            await NavigationRoot.Navigation.PushModalAsync(page, animated).ConfigureAwait(false);
        }

        protected override void OnStart ()
		{
        }

		protected override void OnSleep ()
		{
			// Handle when your app sleeps
		}

		protected override void OnResume ()
		{
            // Handle when your app resumes
        }

        private void RegisterServicesAndProviders()
        {

            /* NOTE on Dependency Injection in Xamarin:
             * 
             * 'B2CAuthenticationService' implements the 'IAuthenticationService' interface. 
             * Using the DependencyService we can register the 'B2CAuthenticationService' such 
             * that when we ask for an instance of the 'IAuthenticationService' like this:
             * 
             *      var authenticationService = DependencyService.Get<IAuthenticationService>();
             * 
             * it allows us to grab the instance of the B2CAuthenticationService that we register in the line below:
             * 
             * */
            DependencyService.Register<B2CAuthenticationService>();


            DependencyService.Register<DebugLoggingService>();
        }
    }
}