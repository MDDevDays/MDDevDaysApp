using System.Collections.Generic;
using System.IO;
using System.Reflection;
using MDDevDaysApp.DomainModel;
using Newtonsoft.Json;

namespace MDDevDaysApp.Infrastructure
{
    public class Speakers : ISpeakers
    {
        private IEnumerable<Speaker> _speakers;

        public IEnumerable<Speaker> All()
        {
            return _speakers ?? (_speakers = ReadSpeakersFromJSON());
        }

        private IEnumerable<Speaker> ReadSpeakersFromJSON()
        {
            var assembly = typeof(Speakers).GetTypeInfo().Assembly;
            var stream = assembly.GetManifestResourceStream("MDDevDaysApp.Infrastructure.Data.speakers.json");

            using (var reader = new StreamReader(stream))
            {
                var json = reader.ReadToEnd();
                return JsonConvert.DeserializeObject<List<Speaker>>(json);
            }
        }
    }
}