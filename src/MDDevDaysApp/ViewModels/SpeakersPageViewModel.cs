using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using MDDevDaysApp.DomainModel;
using Prism;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;

namespace MDDevDaysApp.ViewModels
{
    public class SpeakersPageViewModel : BindableBase, IActiveAware
    {
        private readonly ISpeakers _speakers;
        private readonly INavigationService _navigationService;
        private bool _isActive;

        public SpeakersPageViewModel(ISpeakers speakers, INavigationService navigationService)
        {
            _speakers = speakers;
            _navigationService = navigationService;
            SpeakersGroups = new ObservableCollection<SpeakersGroup>();
            Title = "Sprecher";
            ShowSpeaker = new DelegateCommand<Speaker>(ShowSpeakerExecute);

            PropertyChanged += OnPropertyChanged;
        }

        public string Title { get; }
        public ObservableCollection<SpeakersGroup> SpeakersGroups { get; set; }
        public ICommand ShowSpeaker { get; private set; }

        public bool IsActive
        {
            // ReSharper disable ArrangeAccessorOwnerBody - MobileCenter does not support C#7 Features yet
            get { return _isActive; }
            set { SetProperty(ref _isActive, value); }
            // ReSharper restore ArrangeAccessorOwnerBody
        }

        public event EventHandler IsActiveChanged;

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            if (propertyChangedEventArgs.PropertyName == nameof(IsActive) && IsActive)
                ActivateAsync();
        }

        private async void ActivateAsync()
        {
            if (SpeakersGroups.Any())
                return;

            var allSpeakers = await _speakers.AllAsync();
            var speakersGrouping = allSpeakers.GroupBy(s => s.FirstName.Substring(0, 1));
            foreach (var speakers in speakersGrouping)
            {
                var speakersGroup = new SpeakersGroup {Title = speakers.Key};
                foreach (var speaker in speakers)
                {
                    speakersGroup.Add(speaker);
                }

                SpeakersGroups.Add(speakersGroup);
            }
        }

        private void ShowSpeakerExecute(Speaker speaker)
        {
            _navigationService.NavigateAsync("SpeakerPage", new NavigationParameters { { "speaker", speaker } }, false);
        }
    }
}