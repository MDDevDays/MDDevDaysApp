using Prism.Mvvm;
using Prism.Navigation;

namespace MDDevDaysApp.ViewModels
{
    public class MainTabbedPageViewModel : BindableBase, INavigationAware
    {
        public MainTabbedPageViewModel()
        {
            Title = "Magdeburger Developer Days";
        }

        public string Title { get; }

        public void OnNavigatedFrom(NavigationParameters parameters)
        {
        }

        public void OnNavigatedTo(NavigationParameters parameters)
        {
        }
    }
}