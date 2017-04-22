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
        private readonly INavigationService _navigationService;
        private readonly ISpeakers _speakers;
        private bool _isActive;

        public SpeakersPageViewModel(ISpeakers speakers, INavigationService navigationService)
        {
            _speakers = speakers;
            _navigationService = navigationService;
            Speakers = new ObservableCollection<Grouping<string, Speaker>>();
            Title = "Sprecher";
            ShowSpeaker = new DelegateCommand<Speaker>(ShowSpeakerExecute);

            PropertyChanged += OnPropertyChanged;
        }

        public string Title { get; }
        public ObservableCollection<Grouping<string, Speaker>> Speakers { get; set; }
        public ICommand ShowSpeaker { get; }

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
            if (Speakers.Any())
                return;

            var allSpeakers = await _speakers.AllAsync();
            var groupedSpeakers = from speaker in allSpeakers
                orderby speaker.FirstName
                group speaker by speaker.FirstName.Substring(0, 1)
                into speakerGroup
                select new Grouping<string, Speaker>(speakerGroup.Key, speakerGroup);

            foreach (var grouping in groupedSpeakers)
                Speakers.Add(grouping);
        }

        private void ShowSpeakerExecute(Speaker speaker)
        {
            _navigationService.NavigateAsync("SpeakerPage", new NavigationParameters {{"speaker", speaker}}, false);
        }
    }
}