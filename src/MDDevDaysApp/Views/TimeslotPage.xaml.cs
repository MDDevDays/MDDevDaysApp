using System.ComponentModel;
using MDDevDaysApp.ViewModels;
using Xamarin.Forms;

namespace MDDevDaysApp.Views
{
    public partial class TimeslotPage : ContentPage
    {
        private TimeslotPageViewModel _vm;

        public TimeslotPage()
        {
            InitializeComponent();

            ViewModel.PropertyChanged += ViewModelOnPropertyChanged;
        }

        private TimeslotPageViewModel ViewModel => _vm ?? (_vm = BindingContext as TimeslotPageViewModel);

        private void ViewModelOnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            if (propertyChangedEventArgs.PropertyName == nameof(ViewModel.Timeslot) && ViewModel != null)
                SpeakersListView.HeightRequest = ViewModel.Timeslot.Speakers.Count * SpeakersListView.RowHeight;
        }

        private void SpeakersListView_OnItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (e.Item == null)
                return;

            ((ListView)sender).SelectedItem = null;
        }
    }
}