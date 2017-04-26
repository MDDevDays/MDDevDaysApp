using Prism.Mvvm;
using MDDevDaysApp.DomainModel;
using Prism.Navigation;

namespace MDDevDaysApp.ViewModels
{
    public class TimeslotPageViewModel : BindableBase, INavigationAware
    {
        private Timeslot _speaker;

        public TimeslotPageViewModel()
        {
            Title = "Session Details";
        }

        public Timeslot Timeslot
        {
            get { return _speaker; }
            set { SetProperty(ref _speaker, value); }
        }

        public string Title { get; }

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
    }
}
