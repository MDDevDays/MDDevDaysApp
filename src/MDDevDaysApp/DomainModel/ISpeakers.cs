using System.Collections.Generic;

namespace MDDevDaysApp.DomainModel
{
    public interface ISpeakers
    {
        IEnumerable<Speaker> All();
    }
}