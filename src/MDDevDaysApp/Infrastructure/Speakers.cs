using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using MDDevDaysApp.DomainModel;
using Newtonsoft.Json;

namespace MDDevDaysApp.Infrastructure
{
    public class Speakers : ISpeakers
    {
        private IEnumerable<Speaker> _speakers;

        public async Task<IEnumerable<Speaker>> AllAsync()
        {
            return _speakers ?? (_speakers = await ReadSpeakersFromJSONAsync());
        }

        private async Task<IEnumerable<Speaker>> ReadSpeakersFromJSONAsync()
        {
            var assembly = typeof(Speakers).GetTypeInfo().Assembly;
            var stream = assembly.GetManifestResourceStream("MDDevDaysApp.Infrastructure.Data.speakers.json");

            using (var reader = new StreamReader(stream))
            {
                var json = await reader.ReadToEndAsync();
                return JsonConvert.DeserializeObject<List<Speaker>>(json);
            }
        }
    }
}