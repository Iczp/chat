﻿using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using IczpNet.Chat.SessionSections.SessionUnits;

namespace IczpNet.Chat.Follows
{
    public interface IFollowManager
    {
        Task<bool> CreateAsync(Guid ownerId, List<Guid> idList);

        Task<bool> CreateAsync(SessionUnit owner, List<Guid> idList);

        Task DeleteAsync(Guid ownerId, List<Guid> idList);

        Task DeleteAsync(SessionUnit owner, List<Guid> idList);

    }
}
