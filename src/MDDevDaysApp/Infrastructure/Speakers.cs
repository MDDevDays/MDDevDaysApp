using System.Collections.Generic;
using MDDevDaysApp.DomainModel;

namespace MDDevDaysApp.Infrastructure
{
    public class Speakers : ISpeakers
    {
        public IEnumerable<Speaker> All()
        {
            return new List<Speaker>();
        }
    }
}