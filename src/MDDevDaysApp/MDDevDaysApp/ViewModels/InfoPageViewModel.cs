using Prism.Mvvm;

namespace MDDevDaysApp.ViewModels
{
    public class InfoPageViewModel : BindableBase
    {
        public InfoPageViewModel()
        {
            Title = "Infos";
        }

        public string Title { get; }
    }
}