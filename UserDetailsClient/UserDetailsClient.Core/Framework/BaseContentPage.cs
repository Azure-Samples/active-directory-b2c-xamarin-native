﻿using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace UserDetailsClient.Core.Framework
{
    public abstract class BaseContentPage<T> : ContentPage
        where T : BaseViewModel
    {
        private bool isAlreadyInitialized;
        private bool isAlreadyUninitialized;

        protected virtual T ViewModel => BindingContext as T;

        protected override void OnAppearing()
        {
            base.OnAppearing();

            NavigationProxy.Inner = App.NavigationRoot?.NavigationProxy;

            if (!isAlreadyInitialized)
            {
                ViewModel.InitializeAsync();
                isAlreadyInitialized = true;
            }
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();


            if (!isAlreadyUninitialized)
            {
                ViewModel.UninitializeAsync();
                isAlreadyUninitialized = true;
            }
        }
    }
}