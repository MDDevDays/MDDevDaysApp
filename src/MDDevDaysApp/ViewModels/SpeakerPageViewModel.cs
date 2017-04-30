using System.Collections.ObjectModel;
using System.Windows.Input;
using MDDevDaysApp.DomainModel;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;

namespace MDDevDaysApp.ViewModels
{
    public class SpeakerPageViewModel : BindableBase, INavigationAware
    {
        private readonly INavigationService _navigationService;
        private readonly ITimeslots _timeslots;
        private Speaker _speaker;

        public SpeakerPageViewModel(ITimeslots timeslots, INavigationService navigationService)
        {
            _timeslots = timeslots;
            _navigationService = navigationService;
            Title = "Sprecher";
            Timeslots = new ObservableCollection<Timeslot>();
            ShowTimeslot = new DelegateCommand<Timeslot>(ShowTimeslotExecute);
        }

        public Speaker Speaker
        {
            // ReSharper disable ArrangeAccessorOwnerBody
            get { return _speaker; }
            set { SetProperty(ref _speaker, value); }
            // ReSharper restore ArrangeAccessorOwnerBody
        }

        public ObservableCollection<Timeslot> Timeslots { get; }
        public string Title { get; }
        public ICommand ShowTimeslot { get; set; }

        public void OnNavigatedFrom(NavigationParameters parameters)
        {
        }

        public void OnNavigatedTo(NavigationParameters parameters)
        {
            if (parameters.ContainsKey("speaker"))
                SetSpeakerAndSessions((Speaker) parameters["speaker"]);
        }

        public void OnNavigatingTo(NavigationParameters parameters)
        {
        }

        private void ShowTimeslotExecute(Timeslot timeslot)
        {
            _navigationService.NavigateAsync("TimeslotPage", new NavigationParameters {{"timeslot", timeslot}}, false);
        }

        private async void SetSpeakerAndSessions(Speaker speaker)
        {
            Speaker = speaker;

            Timeslots.Clear();
            foreach (var speakerTimeslot in await _timeslots.AllBySpeakerAsync(speaker))
                Timeslots.Add(speakerTimeslot);
        }
    }
}