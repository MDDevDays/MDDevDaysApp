using System.Collections.Generic;
using System.Threading.Tasks;

namespace MDDevDaysApp.DomainModel
{
    public interface ITimeslots
    {
        Task<IEnumerable<Timeslot>> AllAsync();
    }
}