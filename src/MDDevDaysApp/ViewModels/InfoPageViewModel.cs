using System.Reflection;
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
        }

        public string Title { get; }
        public ImageSource BannerSource { get; }
        public ImageSource LocationPlanSource { get; }
    }
}
