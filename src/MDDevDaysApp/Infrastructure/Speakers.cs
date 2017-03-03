using System;
using System.Collections.Generic;
using MDDevDaysApp.DomainModel;

namespace MDDevDaysApp.Infrastructure
{
    public class Speakers : ISpeakers
    {
        public IEnumerable<Speaker> All()
        {
            return new List<Speaker>()
            {
                new Speaker {Id = Guid.NewGuid(), FirstName = "Quentin", LastName = "Tarantino"},
                new Speaker {Id = Guid.NewGuid(), FirstName = "Uma", LastName = "Thurman"}
            };
        }
    }
}