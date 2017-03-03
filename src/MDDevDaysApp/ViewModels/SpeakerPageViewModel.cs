using System.Collections.Generic;
using MDDevDaysApp.DomainModel;
using Prism.Mvvm;

namespace MDDevDaysApp.ViewModels
{
    public class SpeakerPageViewModel : BindableBase
    {
        private readonly ISpeakers _speakers;

        public SpeakerPageViewModel(ISpeakers speakers)
        {
            _speakers = speakers;
            Title = "Sprecher";
        }

        public string Title { get; }
        public IEnumerable<Speaker> Speakers => _speakers.All();
    }
}
