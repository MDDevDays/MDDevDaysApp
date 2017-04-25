using System;
using System.Collections.Generic;
using System.Linq;

namespace MDDevDaysApp.DomainModel
{
    public class Timeslot
    {
        public Timeslot()
        {
            SpeakerIds = new List<Guid>();
            Speakers = new List<Speaker>();
        }

        public Guid Id { get; set; }
        public DateTime Start { get; set; }
        public DateTime? End { get; set; }
        public string Title { get; set; }
        public string Room { get; set; }
        public List<Guid> SpeakerIds { get; set; }
        public List<Speaker> Speakers { get; set; }

        public bool IsRoomDefined => !string.IsNullOrEmpty(Room);
        public string TimeDisplayShort => $"{Start:M}, {Start:t}";
        public string TimeDisplayLong => End.HasValue ? $"{TimeDisplayShort} - {End:t}" : $"{TimeDisplayShort} - Open End";
        public string SpeakerNames => string.Join(", ", Speakers.Select(s => s.FullName));
        public bool HasSpeakers => Speakers.Any();
    }
}