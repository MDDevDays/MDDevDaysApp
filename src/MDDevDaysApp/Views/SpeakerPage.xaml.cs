using System.ComponentModel;
using MDDevDaysApp.ViewModels;
using Xamarin.Forms;

namespace MDDevDaysApp.Views
{
    public partial class SpeakerPage : ContentPage
    {
        private SpeakerPageViewModel _vm;

        public SpeakerPage()
        {
            InitializeComponent();

            ViewModel.PropertyChanged += ViewModelOnPropertyChanged;
        }

        private SpeakerPageViewModel ViewModel => _vm ?? (_vm = BindingContext as SpeakerPageViewModel);

        private void ViewModelOnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            if (propertyChangedEventArgs.PropertyName == nameof(ViewModel.Timeslots) && ViewModel != null)
                TimeslotsListView.HeightRequest = ViewModel.Timeslots.Count * TimeslotsListView.RowHeight;
        }

        private void TimeslotsListView_OnItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (e.Item == null)
                return;

            ((ListView) sender).SelectedItem = null;
        }
    }
}