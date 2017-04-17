using System;

namespace MDDevDaysApp.DomainModel
{
    public class Timeslot
    {
        public Guid Id { get; set; }
        public DateTime Start { get; set; }
        public DateTime? End { get; set; }
        public string Title { get; set; }
        public string Room { get; set; }
    }
}