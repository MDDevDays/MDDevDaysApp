using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using MDDevDaysApp.DomainModel;
using Newtonsoft.Json;

namespace MDDevDaysApp.Infrastructure
{
    public class Timeslots : ITimeslots
    {
        private readonly ISpeakers _speakers;
        private IEnumerable<Timeslot> _timeslots;

        public Timeslots(ISpeakers speakers)
        {
            _speakers = speakers;
        }

        public async Task<IEnumerable<Timeslot>> AllAsync()
        {
            await EnsureTimeslotsAreLoaded();
            return _timeslots;
        }

        public async Task<IEnumerable<Timeslot>> AllBySpeakerAsync(Speaker speaker)
        {
            await EnsureTimeslotsAreLoaded();
            return _timeslots.Where(ts => ts.SpeakerIds.Contains(speaker.Id)).AsEnumerable();
        }

        private async Task EnsureTimeslotsAreLoaded()
        {
            if (_timeslots != null)
                return;

            _timeslots = await ReadTimeslotsFromJSONAsync();
            await FillSpeakersInTimeslots();
        }

        private async Task<IEnumerable<Timeslot>> ReadTimeslotsFromJSONAsync()
        {
            var assembly = typeof(Timeslot).GetTypeInfo().Assembly;
            var stream = assembly.GetManifestResourceStream("MDDevDaysApp.Infrastructure.Data.timeslots.json");

            using (var reader = new StreamReader(stream))
            {
                var json = await reader.ReadToEndAsync();
                return JsonConvert.DeserializeObject<List<Timeslot>>(json);
            }
        }

        private async Task FillSpeakersInTimeslots()
        {
            foreach (var timeslot in _timeslots)
            foreach (var speakerId in timeslot.SpeakerIds)
                timeslot.Speakers.Add(await _speakers.GetByAsync(speakerId));
        }
    }
}