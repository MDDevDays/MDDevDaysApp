using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using MDDevDaysApp.DomainModel;
using Prism;
using Prism.Mvvm;

namespace MDDevDaysApp.ViewModels
{
    public class TimeslotsPageViewModel : BindableBase, IActiveAware
    {
        private readonly ITimeslots _timeslots;
        private readonly ISpeakers _speakers;
        private bool _isActive;

        public TimeslotsPageViewModel(ITimeslots timeslots, ISpeakers speakers)
        {
            _timeslots = timeslots;
            _speakers = speakers;
            Title = "Programm";
            Timeslots = new ObservableCollection<Grouping<string, Timeslot>>();

            PropertyChanged += OnPropertyChanged;
        }

        public ObservableCollection<Grouping<string, Timeslot>> Timeslots { get; }

        public string Title { get; }

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
            if (Timeslots.Any())
                return;

            var allTimeslots = (await _timeslots.AllAsync()).ToList();
            await SetSpeakersInTimeSlots(allTimeslots);
            var groupedTimeslots = from timeslot in allTimeslots
                orderby timeslot.Start
                group timeslot by timeslot.TimeDisplayShort
                into timeslotGroup
                select new Grouping<string, Timeslot>(timeslotGroup.Key, timeslotGroup);

            foreach (var grouping in groupedTimeslots)
                Timeslots.Add(grouping);
        }

        private async Task SetSpeakersInTimeSlots(IEnumerable<Timeslot> allTimeslots)
        {
            foreach (var timeslot in allTimeslots)
            {
                foreach (var speakerId in timeslot.SpeakerIds)
                {
                    timeslot.Speakers.Add(await _speakers.GetByAsync(speakerId));
                }
            }
        }
    }
}