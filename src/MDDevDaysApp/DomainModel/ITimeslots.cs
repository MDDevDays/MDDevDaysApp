using System.Collections.Generic;
using System.Threading.Tasks;

namespace MDDevDaysApp.DomainModel
{
    public interface ITimeslots
    {
        Task<IEnumerable<Timeslot>> AllAsync();
        Task<IEnumerable<Timeslot>> AllBySpeakerAsync(Speaker speaker);
        Task EnsureTimeslotsAreLoaded();
    }
}