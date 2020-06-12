using System.Windows.Input;

using WPF.Pure.ViewModels;

namespace WPF.Pure.States.Navigator
{
    public interface INavigator
    {
        BaseViewModel CurrentViewModel { get; set; }

        ICommand UpdataCurrentViewModelCommand { get; }
    }
}
