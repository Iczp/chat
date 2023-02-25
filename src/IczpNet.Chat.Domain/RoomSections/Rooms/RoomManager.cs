﻿using IczpNet.AbpCommons;
using IczpNet.AbpCommons.Extensions;
using IczpNet.Chat.ChatObjects;
using IczpNet.Chat.Enums;
using IczpNet.Chat.MessageSections;
using IczpNet.Chat.MessageSections.Messages;
using IczpNet.Chat.MessageSections.Templates;
using IczpNet.Chat.Options;
using IczpNet.Chat.SessionSections.Sessions;
using IczpNet.Chat.SessionSections.SessionUnits;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Caching;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;

namespace IczpNet.Chat.RoomSections.Rooms
{
    public class RoomManager : DomainService, IRoomManager
    {
        protected RoomOptions Config { get; }
        protected IRepository<Room, Guid> Repository { get; }
        protected IRepository<Session, Guid> SessionRepository { get; }
        protected ISessionGenerator SessionGenerator { get; }
        protected IChatObjectManager ChatObjectManager { get; }
        protected ISessionUnitManager SessionUnitManager { get; }

        protected IDistributedCache<List<Guid>, Guid> SessionUnitIdListCache { get; }

        protected IMessageSender ChatSender { get; }

        public RoomManager(
            IOptions<RoomOptions> options,
            IRepository<Room, Guid> repository,
            IRepository<Session, Guid> sessionRepository,
            ISessionGenerator sessionGenerator,
            IChatObjectManager chatObjectManager,
            IMessageSender chatSender,
            IDistributedCache<List<Guid>, Guid> sessionUnitIdListCache,
            ISessionUnitManager sessionUnitManager)
        {
            Config = options.Value;
            Repository = repository;
            SessionRepository = sessionRepository;
            SessionGenerator = sessionGenerator;
            ChatObjectManager = chatObjectManager;
            ChatSender = chatSender;
            SessionUnitIdListCache = sessionUnitIdListCache;
            SessionUnitManager = sessionUnitManager;
        }

        public virtual Task<bool> IsAllowJoinRoomAsync(ChatObjectTypeEnums? objectType)
        {
            return Task.FromResult(Config.AllowJoinRoomObjectTypes.Any(x => x.Equals(objectType)));
        }

        public virtual Task<bool> IsAllowCreateRoomAsync(ChatObjectTypeEnums? objectType)
        {
            return Task.FromResult(Config.AllowCreateRoomObjectTypes.Any(x => x.Equals(objectType)));
        }

        public virtual async Task<Room> CreateRoomAsync(Room room, List<ChatObject> members)
        {
            if (room.OwnerId.HasValue)
            {
                room.SetOwner(await ChatObjectManager.GetAsync(room.OwnerId.Value));
                Assert.If(!await IsAllowCreateRoomAsync(room.Owner.ObjectType), $"Not allowed to create the room,ObjectType:{room.Owner.ObjectType},Id:{room.Owner.Id}");
            }

            foreach (ChatObject member in members)
            {
                Assert.If(!await IsAllowJoinRoomAsync(member.ObjectType), $"Not allowed to join the room,ObjectType:{member.ObjectType},Id:{member.Id}");
            }
            var chatObject = await ChatObjectManager.GetItemByCacheAsync(room.Id);
            var session = await SessionGenerator.MakeAsync(chatObject, chatObject);

            session.SetUnitList(members.Select(x =>
                    new SessionUnit(GuidGenerator.Create(), session, x.Id, room.Id, room.ObjectType)
                    {
                        InviterId = room.OwnerId,
                        JoinWay = JoinWays.Creator,
                    }
                ).ToList());

            session.SetOwner(room);

            room.SetSession(session);

            room.SetMemberCount(members.Count);

            await Repository.InsertAsync(room, autoSave: true);

            await ChatSender.SendCmdMessageAsync(new MessageInput<CmdContentInfo>()
            {
                SenderId = room.Id,
                ReceiverId = room.Id,
                Content = new CmdContentInfo()
                {
                    Text = $"{room.Owner?.Name}创建群,{members.Take(3).Select(x => x.Name).JoinAsString("、")}等 {members.Count} 人加入群聊。",
                }
            });

            return room;
        }

        public virtual async Task<int> JoinRoomAsync(Room room, List<ChatObject> members, ChatObject inviter, JoinWays joinWay)
        {
            Assert.If(room.IsInRoom(inviter), $"The inviter[{inviter}] is no in room[{room}]");

            var addCount = 0;

            foreach (var member in members)
            {
                if (room.IsInRoom(member))
                {
                    Logger.LogWarning($"The member[{member}] is in room[{room}]");
                    continue;
                }
                room.Session.AddSessionUnit(new SessionUnit(GuidGenerator.Create(), room.Session, member.Id, room.Id, room.ObjectType)
                {
                    JoinWay = joinWay,
                    Inviter = inviter
                });

                await SessionUnitIdListCache.RemoveAsync(room.Session.Id);

                addCount++;
            }

            if (addCount == 0)
            {
                return 0;
            }

            room.SetMemberCount(await GetMemberCountAsync(room));

            await Repository.UpdateAsync(room, autoSave: true);

            await ChatSender.SendCmdMessageAsync(new MessageInput<CmdContentInfo>()
            {
                SenderId = room.Id,
                ReceiverId = room.Id,
                Content = new CmdContentInfo()
                {
                    Text = $"{members.Select(x => x.Name).JoinAsString("、")}等 {members.Count} 人通过'{joinWay.GetDescription()}'加入群聊。",
                }
            });
            return addCount;
        }

        public virtual Task<bool> IsInRoomAsync(Room room, ChatObject member)
        {
            return Task.FromResult(room.IsInRoom(member));
        }

        public async Task<Room> UpdateRoomAsync(Room room)
        {
            room.SetName(room.Name);

            room.SetMemberCount(await GetMemberCountAsync(room));

            return await Repository.UpdateAsync(room, autoSave: true);
        }

        public async Task<int> GetMemberCountAsync(Room room)
        {
            if (room.SessionId.HasValue)
            {
                return await SessionUnitManager.GetCountAsync(room.SessionId.Value);
            }
            return 0;
        }
    }
}
