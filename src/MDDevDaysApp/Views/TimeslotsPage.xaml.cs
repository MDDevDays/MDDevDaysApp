﻿using Xamarin.Forms;

namespace MDDevDaysApp.Views
{
    public partial class TimeslotsPage : ContentPage
    {
        public TimeslotsPage()
        {
            InitializeComponent();
        }

        private void ListView_OnItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (e.Item == null)
                return;

            ((ListView)sender).SelectedItem = null;
        }
    }
}
