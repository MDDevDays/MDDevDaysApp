using System;
using System.Reflection;
using System.Windows.Input;
using Prism.Commands;
using Prism.Mvvm;
using Xamarin.Forms;

namespace MDDevDaysApp.ViewModels
{
    public class InfoPageViewModel : BindableBase
    {
        public InfoPageViewModel()
        {
            Title = "Infos";

            // Angabe der Assembly ist notwendig, damit UWP im Release Mode das Bild auch anzeigt
            var assembly = typeof(InfoPageViewModel).GetTypeInfo().Assembly;
            BannerSource = ImageSource.FromResource("MDDevDaysApp.Images.Banner.png", assembly);
            LocationPlanSource = ImageSource.FromResource("MDDevDaysApp.Images.Lageplan.png", assembly);
            OpenTwitterStream = new DelegateCommand(OpenTwitterStreamExecute);
            OpenTwitterHashtag = new DelegateCommand(OpenTwitterHashtagExecute);
            OpenUrl = new DelegateCommand<string>(OpenUrlExecute);
        }

        public string Title { get; }
        public ImageSource BannerSource { get; }
        public ImageSource LocationPlanSource { get; }
        public ICommand OpenTwitterHashtag { get; set; }
        public ICommand OpenTwitterStream { get; set; }
        public ICommand OpenUrl { get; set; }

        private void OpenUrlExecute(string url)
        {
            Device.OpenUri(new Uri(url));
        }

        private void OpenTwitterStreamExecute()
        {
            try
            {
                Device.OpenUri(new Uri("twitter://user?id=4158533145"));
            }
            catch (Exception)
            {
                Device.OpenUri(new Uri("https://twitter.com/MiB_MD_DevDays"));
            }
        }

        private void OpenTwitterHashtagExecute()
        {
            try
            {
                Device.OpenUri(new Uri("twitter://search?query=%23MDDevDays"));
            }
            catch (Exception)
            {
                Device.OpenUri(new Uri("https://twitter.com/search?q=%23MDDevDays"));
            }
        }
    }
}