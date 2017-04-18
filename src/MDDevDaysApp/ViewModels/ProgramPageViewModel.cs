using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using MDDevDaysApp.DomainModel;
using Prism;
using Prism.Mvvm;

namespace MDDevDaysApp.ViewModels
{
    public class ProgramPageViewModel : BindableBase, IActiveAware
    {
        private readonly ITimeslots _timeslots;
        private bool _isActive;

        public ProgramPageViewModel(ITimeslots timeslots)
        {
            _timeslots = timeslots;
            Title = "Programm";
            Timeslots = new ObservableCollection<Timeslot>();

            PropertyChanged += OnPropertyChanged;
        }

        public ObservableCollection<Timeslot> Timeslots { get; private set; }

        public string Title { get; }

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
            if (Timeslots.Any())
                return;

            var allTimeslots = await _timeslots.AllAsync();
            foreach (var timeslot in allTimeslots)
                Timeslots.Add(timeslot);
        }
    }
}