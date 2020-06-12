using System.Windows.Input;

using WPF.Pure.Commands;
using WPF.Pure.Models;
using WPF.Pure.ViewModels;

namespace WPF.Pure.States.Navigator
{
    public class Navigator : ObservableObject, INavigator
    {
        private BaseViewModel _currentViewModel { get; set; }

        public BaseViewModel CurrentViewModel
        {
            get { return _currentViewModel; }
            set
            {
                _currentViewModel = value;
                OnPropertyChanged(nameof(CurrentViewModel));
            }
        }

        public ICommand UpdataCurrentViewModelCommand => new UpdateCurrentViewModelCommand(this);
    }
}
