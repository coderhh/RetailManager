﻿using Caliburn.Micro;
using RMDesktopUI.EventModels;
using RMDesktopUI.Library.Api;
using System;
using System.Threading.Tasks;

namespace RMDesktopUI.ViewModels
{
    public class LoginViewModel : Screen
    {
        private string _userName = "test@gmail.com";
        private string _password = "a12358@Hyh";
        private IAPIHelper _apiHelper;
        private IEventAggregator _events;

        public LoginViewModel(IAPIHelper apiHelper, IEventAggregator events)
        {
            _apiHelper = apiHelper;
            _events = events;
        }

        public string UserName
        {
            get { return _userName; }
            set
            {
                _userName = value;
                NotifyOfPropertyChange(() => UserName);
                NotifyOfPropertyChange(() => CanLogIn);
            }
        }

        public string Password
        {
            get { return _password; }
            set
            {
                _password = value;
                NotifyOfPropertyChange(() => Password);
                NotifyOfPropertyChange(() => CanLogIn);
            }
        }

        private bool _isErrorVisible;

        public bool IsErrorVisible
        {
            get
            {
                var output = !String.IsNullOrEmpty(ErrorMessage);
                return output;
            }
            set
            {
                _isErrorVisible = value;
                NotifyOfPropertyChange(() => IsErrorVisible);
            }
        }

        private string errorMessage;

        public string ErrorMessage
        {
            get { return errorMessage; }
            set
            {
                errorMessage = value;
                NotifyOfPropertyChange(() => ErrorMessage);
                NotifyOfPropertyChange(() => IsErrorVisible);
            }
        }

        public bool CanLogIn
        {
            get
            {
                return (!String.IsNullOrEmpty(UserName) && !String.IsNullOrEmpty(Password));
            }
        }

        public async Task LogIn()
        {
            try
            {
                ErrorMessage = "";
                var result = await _apiHelper.Authenticate(UserName, Password);

                // Capture more information about the user
                await _apiHelper.GetLoggedInUserInfo(result.Access_Token);

                _events.PublishOnUIThread(new LogOnEvent());
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
            }

        }
    }
}
