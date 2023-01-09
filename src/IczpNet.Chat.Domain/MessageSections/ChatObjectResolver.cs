﻿using IczpNet.AbpCommons;
using IczpNet.Chat.ChatObjects;
using IczpNet.Chat.MessageSections.Messages;
using IczpNet.Chat.RoomSections.Rooms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Caching;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;

namespace IczpNet.Chat.MessageSections
{
    public class ChatObjectResolver : DomainService, IChatObjectResolver
    {
        protected IRepository<Room, Guid> RoomRepository { get; }

        protected IDistributedCache<List<Guid>, Guid> SessionUnitIdListCache { get; }

        public ChatObjectResolver(
            IRepository<Room, Guid> roomRepository,
            IDistributedCache<List<Guid>, Guid> sessionUnitIdListCache)
        {
            RoomRepository = roomRepository;
            SessionUnitIdListCache = sessionUnitIdListCache;
        }
        public virtual async Task<List<ChatObject>> GetListAsync(Message message)
        {
            var result = new List<ChatObject>();

            switch (message.Channel)
            {
                case Enums.Channels.PrivateChannel:
                    return new List<ChatObject>() { message.Sender, message.Receiver };
                case Enums.Channels.RoomChannel:
                    await RoomRepository.GetAsync(message.ReceiverId.Value);

                    break;
                case Enums.Channels.SubscriptionChannel:
                    break;
                case Enums.Channels.ServiceChannel:
                    break;
                case Enums.Channels.SquareChannel:
                    break;
                case Enums.Channels.RobotChannel:
                    break;
                case Enums.Channels.ElectronicCommerceChannel:
                    break;
                default:
                    break;
            }
            return result;
        }

        public virtual Task<List<Guid>> GetIdListAsync(Message message)
        {
            Assert.NotNull(message.SessionId, "Message.Session is not null.");

            return SessionUnitIdListCache.GetOrAddAsync(message.SessionId.Value, () =>
            {
                return Task.FromResult(message.Session.UnitList.Select(x => x.OwnerId).ToList());
            });
        }
    }
}
