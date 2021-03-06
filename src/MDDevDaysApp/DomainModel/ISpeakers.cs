﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MDDevDaysApp.DomainModel
{
    public interface ISpeakers
    {
        Task<IEnumerable<Speaker>> AllAsync();
        Task<Speaker> GetByAsync(Guid speakerId);
    }
}