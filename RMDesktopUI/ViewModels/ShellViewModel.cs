using Caliburn.Micro;
using RMDesktopUI.EventModels;
using RMDesktopUI.Library.Models;

namespace RMDesktopUI.ViewModels
{
    public class ShellViewModel : Conductor<object>, IHandle<LogOnEvent>
    {
        private readonly IEventAggregator _events;
        private readonly SalesViewModel _salesVM;
        private readonly ILoggedInUserModel _user;
        public ShellViewModel(IEventAggregator events, SalesViewModel salesVM, ILoggedInUserModel user)
        {
            _events = events;
            _salesVM = salesVM;
            _user = user;
            _events.Subscribe(this);
            ActivateItem(IoC.Get<LoginViewModel>());
        }

        public void Handle(LogOnEvent message)
        {
            ActivateItem(_salesVM);
            NotifyOfPropertyChange(() => IsLoggedIn);
        }

        public bool IsLoggedIn
        {
            get
            {
                return !string.IsNullOrEmpty(_user.Token);
            }
            
        }
        public void ExitApplication()
        {
            TryClose();
        }

        public void LogOut()
        {
            _user.LogOffUser();
            ActivateItem(IoC.Get<LoginViewModel>());
            NotifyOfPropertyChange(() => IsLoggedIn);
        }
    }
}
