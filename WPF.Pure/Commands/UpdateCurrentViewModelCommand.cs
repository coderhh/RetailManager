using System;
using System.Windows.Input;

using WPF.Pure.States;
using WPF.Pure.States.Navigator;
using WPF.Pure.ViewModels;

namespace WPF.Pure.Commands
{
    public class UpdateCurrentViewModelCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;
        private INavigator _navigator;

        public UpdateCurrentViewModelCommand(INavigator navigator)
        {
            _navigator = navigator;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            if (parameter is ViewType)
            {
                ViewType viewType = (ViewType)parameter;
                switch (viewType)
                {
                    case ViewType.Home:
                        _navigator.CurrentViewModel = new HomeViewModel();
                        break;
                    case ViewType.Products:
                        _navigator.CurrentViewModel = new ProductsViewModel();
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
