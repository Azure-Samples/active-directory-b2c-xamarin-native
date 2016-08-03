using Microsoft.Identity.Client;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace TaskClient
{
    public partial class TasksPage : ContentPage
    {
        public TasksPage()
        {
            InitializeComponent();            
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            try
            {
                AuthenticationResult ar = await App.PCApplication.AcquireTokenSilentAsync(App.Scopes, "", App.Authority, App.SignUpSignInpolicy, false);
                RefreshTaskList(ar.Token);
            }
            catch (Exception ee)
            {
                DisplayAlert("An error has occurred", "Exception message: " + ee.Message, "Dismiss");
                App.PCApplication.UserTokenCache.Clear(App.PCApplication.ClientId);
                Navigation.PushAsync(new WelcomePage());
            }
        }

        async void RefreshTaskList(string token)
        {            
            HttpClient client = new HttpClient();
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, App.APIbaseURL + "/api/tasks");

            // Add the token acquired from ADAL to the request headers
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            try
            {
                HttpResponseMessage response = await client.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    String responseString = await response.Content.ReadAsStringAsync();
                    JArray tasks = JArray.Parse(responseString);
                    App.Tasks.Clear();
                    foreach (var item in tasks.Children())
                    {
                        string taskValue = string.Empty;
                        if (item["Text"]!=null)
                        {
                            string taskText = item["Text"].ToString();
                            App.Tasks.Add(taskText);
                        }
                    }
                }
            }
            catch (Exception ee)
            {
                DisplayAlert("An error has occurred", "Exception message: " + ee.Message, "Dismiss");
            }
        }

        void OnSignOut(object sender, EventArgs e)
        {
            App.PCApplication.UserTokenCache.Clear(App.PCApplication.ClientId);
            Navigation.PushAsync(new WelcomePage());
        }

        async void OnAddTask(object sender, EventArgs e)
        {
            try
            {
                AuthenticationResult ar = await App.PCApplication.AcquireTokenSilentAsync(App.Scopes, "", App.Authority, App.SignUpSignInpolicy, false);

                HttpContent content = new FormUrlEncodedContent(new[] { new KeyValuePair<string, string>("Text", txtTask.Text) });
                HttpClient client = new HttpClient();
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, App.APIbaseURL + "/api/tasks");
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", ar.Token);
                request.Content = content;
                HttpResponseMessage response = await client.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    RefreshTaskList(ar.Token);
                }
            }
            catch (Exception ee)
            {
                DisplayAlert("An error has occurred", "Exception message: " + ee.Message, "Dismiss");
            }
        }
    }
}
