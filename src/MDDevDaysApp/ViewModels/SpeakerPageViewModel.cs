using MDDevDaysApp.DomainModel;
using Prism.Mvvm;
using Prism.Navigation;

namespace MDDevDaysApp.ViewModels
{
    public class SpeakerPageViewModel : BindableBase, INavigationAware
    {
        private Speaker _speaker;

        public Speaker Speaker
        {
            get { return _speaker; }
            set { SetProperty(ref _speaker, value); }
        }

        public void OnNavigatedFrom(NavigationParameters parameters)
        {
        }

        public void OnNavigatedTo(NavigationParameters parameters)
        {
            if (parameters.ContainsKey("speaker"))
                Speaker = (Speaker) parameters["speaker"];
        }

        public void OnNavigatingTo(NavigationParameters parameters)
        {
        }
    }
}