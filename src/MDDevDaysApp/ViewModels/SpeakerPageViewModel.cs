using System.Collections.ObjectModel;
using MDDevDaysApp.DomainModel;
using Prism.Mvvm;
using Prism.Navigation;

namespace MDDevDaysApp.ViewModels
{
    public class SpeakerPageViewModel : BindableBase, INavigationAware
    {
        private readonly ITimeslots _timeslots;
        private Speaker _speaker;

        public SpeakerPageViewModel(ITimeslots timeslots)
        {
            _timeslots = timeslots;
            Title = "Sprecher";
            Timeslots = new ObservableCollection<Timeslot>();
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

        private async void SetSpeakerAndSessions(Speaker speaker)
        {
            Speaker = speaker;

            Timeslots.Clear();
            foreach (var speakerTimeslot in await _timeslots.AllBySpeakerAsync(speaker))
                Timeslots.Add(speakerTimeslot);
        }
    }
}