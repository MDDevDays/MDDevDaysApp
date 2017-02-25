using Prism.Mvvm;

namespace MDDevDaysApp.ViewModels
{
    public class SpeakerPageViewModel : BindableBase
    {
        public SpeakerPageViewModel()
        {
            Title = "Sprecher";
        }

        public string Title { get; }
    }
}
