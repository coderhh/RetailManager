using WPF.Pure.States.Navigator;

namespace WPF.Pure.ViewModels
{
    public class ProductsViewModel : BaseViewModel
    {
        public INavigator Navigator { get; set; } = new Navigator();

    }
}
