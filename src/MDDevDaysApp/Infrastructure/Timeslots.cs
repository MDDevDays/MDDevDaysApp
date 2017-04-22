using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using MDDevDaysApp.DomainModel;
using Newtonsoft.Json;

namespace MDDevDaysApp.Infrastructure
{
    public class Timeslots : ITimeslots
    {
        private IEnumerable<Timeslot> _timeslots;

        public async Task<IEnumerable<Timeslot>> AllAsync()
        {
            return _timeslots ?? (_timeslots = await ReadTimeslotsFromJSONAsync());
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
    }
}