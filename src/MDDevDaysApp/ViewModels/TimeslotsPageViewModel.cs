using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using MDDevDaysApp.DomainModel;
using Prism;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;

namespace MDDevDaysApp.ViewModels
{
    public class TimeslotsPageViewModel : BindableBase, IActiveAware
    {
        private readonly ITimeslots _timeslots;
        private readonly ISpeakers _speakers;
        private readonly INavigationService _navigationService;
        private bool _isActive;

        public TimeslotsPageViewModel(ITimeslots timeslots, ISpeakers speakers, INavigationService navigationService)
        {
            _timeslots = timeslots;
            _speakers = speakers;
            _navigationService = navigationService;
            Title = "Programm";
            Timeslots = new ObservableCollection<Grouping<string, Timeslot>>();
            ShowTimeslot = new DelegateCommand<Timeslot>(ShowTimeslotExecute);

            PropertyChanged += OnPropertyChanged;
        }

        public string Title { get; }
        public ObservableCollection<Grouping<string, Timeslot>> Timeslots { get; }
        public ICommand ShowTimeslot { get; set; }

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
            var groupedTimeslots = from timeslot in allTimeslots
                orderby timeslot.Start
                group timeslot by timeslot.TimeDisplayShort
                into timeslotGroup
                select new Grouping<string, Timeslot>(timeslotGroup.Key, timeslotGroup);

            foreach (var grouping in groupedTimeslots)
                Timeslots.Add(grouping);
        }

        private void ShowTimeslotExecute(Timeslot timeslot)
        {
            _navigationService.NavigateAsync("TimeslotPage", new NavigationParameters {{"timeslot", timeslot}}, false);
        }
    }
}