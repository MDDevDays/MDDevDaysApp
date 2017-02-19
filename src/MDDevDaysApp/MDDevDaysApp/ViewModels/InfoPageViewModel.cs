using Prism.Mvvm;
using Xamarin.Forms;

namespace MDDevDaysApp.ViewModels
{
    public class InfoPageViewModel : BindableBase
    {
        public InfoPageViewModel()
        {
            Title = "Infos";
            BannerSource = ImageSource.FromResource("MDDevDaysApp.Images.Banner.png");
            ConferenceName = "Magdeburger Developer Days 2017";
            ConferenceDate = "10.05. & 11.05.2017";
            ConferenceLocation = "KONGRESS & KULTURWERK fichte in Magdeburg";
        }

        public string Title { get; }
        public ImageSource BannerSource { get; }
        public string ConferenceName { get; }
        public string ConferenceDate { get; }
        public string ConferenceLocation { get; }
    }
}
