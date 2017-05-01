using System.Windows.Input;
using Prism.Mvvm;
using MDDevDaysApp.DomainModel;
using Prism.Commands;
using Prism.Navigation;

namespace MDDevDaysApp.ViewModels
{
    public class TimeslotPageViewModel : BindableBase, INavigationAware
    {
        private readonly INavigationService _navigationService;
        private Timeslot _speaker;

        public TimeslotPageViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;
            Title = "Session";
            ShowSpeaker = new DelegateCommand<Speaker>(ShowSpeakerExecute);
        }

        public string Title { get; }
        public ICommand ShowSpeaker { get; }
        public Timeslot Timeslot
        {
            get { return _speaker; }
            set { SetProperty(ref _speaker, value); }
        }

        public void OnNavigatedFrom(NavigationParameters parameters)
        {
        }

        public void OnNavigatedTo(NavigationParameters parameters)
        {
            if (parameters.ContainsKey("timeslot"))
                Timeslot = (Timeslot)parameters["timeslot"];
        }

        public void OnNavigatingTo(NavigationParameters parameters)
        {
        }

        private void ShowSpeakerExecute(Speaker speaker)
        {
            _navigationService.NavigateAsync("SpeakerPage", new NavigationParameters { { "speaker", speaker } }, false);
        }
    }
}
