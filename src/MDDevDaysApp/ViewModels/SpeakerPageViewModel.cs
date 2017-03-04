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
            Speakers = new ObservableCollection<Speaker>();
            Title = "Sprecher";

            PropertyChanged += OnPropertyChanged;
        }

        public string Title { get; }
        public ObservableCollection<Speaker> Speakers { get; }

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
            if (Speakers.Any())
                return;

            foreach (var speaker in await _speakers.AllAsync())
                Speakers.Add(speaker);
        }
    }
}