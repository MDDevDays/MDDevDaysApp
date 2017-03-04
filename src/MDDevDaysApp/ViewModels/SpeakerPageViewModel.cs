using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using MDDevDaysApp.DomainModel;
using Prism;
using Prism.Mvvm;

namespace MDDevDaysApp.ViewModels
{
    public class SpeakerPageViewModel : BindableBase, IActiveAware
    {
        private readonly ISpeakers _speakers;
        private bool _isActive;

        public SpeakerPageViewModel(ISpeakers speakers)
        {
            _speakers = speakers;
            SpeakersGroups = new ObservableCollection<SpeakersGroup>();
            Title = "Sprecher";

            PropertyChanged += OnPropertyChanged;
        }

        public string Title { get; }
        public ObservableCollection<SpeakersGroup> SpeakersGroups { get; set; }

        public bool IsActive
        {
            get { return _isActive; }
            set { SetProperty(ref _isActive, value); }
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
    }
}