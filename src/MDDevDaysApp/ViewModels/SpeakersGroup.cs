using System.Collections.ObjectModel;
using MDDevDaysApp.DomainModel;

namespace MDDevDaysApp.ViewModels
{
    public class SpeakersGroup : ObservableCollection<Speaker>
    {
        public string Title { get; set; }
    }
}