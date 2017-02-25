using Prism.Mvvm;

namespace MDDevDaysApp.ViewModels
{
    public class ProgramPageViewModel : BindableBase
    {
        public ProgramPageViewModel()
        {
            Title = "Programm";
        }

        public string Title { get; }
    }
}
