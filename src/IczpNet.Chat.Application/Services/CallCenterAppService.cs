﻿using IczpNet.Chat.BaseAppServices;
using IczpNet.Chat.CallCenters;
using IczpNet.Chat.ChatObjects;
using IczpNet.Chat.SessionSections.Sessions;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace IczpNet.Chat.Services
{
    public class CallCenterAppService : ChatAppService, ICallCenterAppService
    {

        protected IChatObjectManager ChatObjectManager { get; }
        protected ISessionUnitManager SessionUnitManager { get; }

        public CallCenterAppService(IChatObjectManager chatObjectManager, ISessionUnitManager sessionUnitManager)
        {
            ChatObjectManager = chatObjectManager;
            SessionUnitManager = sessionUnitManager;
        }

        [HttpPost]
        public Task TransferToAsync(Guid sessionId, long destinationId)
        {
            throw new NotImplementedException();
        }
    }
}