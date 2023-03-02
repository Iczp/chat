﻿using IczpNet.Chat.ChatObjects;
using IczpNet.Chat.SessionSections.ReadedRecorders;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IczpNet.Chat.SessionSections.Sessions
{
    public interface ISessionRecorder
    {
        Task<long> GetAsync(long ownerId);

        Task<long> GetAsync(ChatObject ownerId);

        Task<long> UpdateAsync(long ownerId, long maxMessageAutoId, bool isForce = false);

        Task<long> UpdateAsync(ChatObject owner, long maxMessageAutoId, bool isForce = false);

        Task<List<ReadedRecorder>> GetReadedsAsync(long ownerId);
    }
}
