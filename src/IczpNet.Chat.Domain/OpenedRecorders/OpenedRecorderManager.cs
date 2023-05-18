﻿using IczpNet.Chat.Bases;
using IczpNet.Chat.MessageSections.Messages;
using IczpNet.Chat.SessionSections.SessionUnits;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace IczpNet.Chat.OpenedRecorders
{
    public class OpenedRecorderManager : RecorderManager<OpenedRecorder>, IOpenedRecorderManager
    {
        public OpenedRecorderManager(IRepository<OpenedRecorder> repository) : base(repository)
        {

        }

        protected override OpenedRecorder CreateEntity(SessionUnit entity, Message message, string deviceId)
        {
            return new OpenedRecorder(entity, message.Id, deviceId);
        }

        protected override OpenedRecorder CreateEntity(Guid sessionUnitId, long messageId)
        {
            return new OpenedRecorder(sessionUnitId, messageId);
        }

        protected override async Task ChangeMessageIfNotContainsAsync(SessionUnit sessionUnit, Message message)
        {
            message.FavoritedCount++;

            await Task.CompletedTask;
        }

        protected override async Task ChangeMessagesIfNotContainsAsync(SessionUnit sessionUnit, List<Message> changeMessages)
        {
            foreach (Message message in changeMessages)
            {
                message.FavoritedCount++;
            }
            await Task.CompletedTask;
        }
    }
}
