﻿using IczpNet.AbpTrees;
using IczpNet.Chat.ChatObjectTypes;
using IczpNet.Chat.Enums;
using IczpNet.Chat.MessageSections;
using IczpNet.Chat.MessageSections.Messages;
using IczpNet.Chat.MessageSections.Templates;
using IczpNet.Chat.SessionSections.Sessions;
using IczpNet.Chat.SessionSections.SessionUnits;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Uow;

namespace IczpNet.Chat.ChatObjects
{
    public class ChatObjectManager : TreeManager<ChatObject, long, ChatObjectInfo>, IChatObjectManager
    {
        protected IChatObjectTypeManager ChatObjectTypeManager { get; }
        protected IMessageSender MessageSender => LazyServiceProvider.LazyGetRequiredService<IMessageSender>();
        protected IUnitOfWorkManager UnitOfWorkManager => LazyServiceProvider.LazyGetRequiredService<IUnitOfWorkManager>();
        protected IUnitOfWork CurrentUnitOfWork => UnitOfWorkManager?.Current;
        protected ISessionGenerator SessionGenerator => LazyServiceProvider.LazyGetRequiredService<ISessionGenerator>();

        public ChatObjectManager(
            IChatObjectRepository repository,
            IChatObjectTypeManager chatObjectTypeManager) : base(repository)
        {
            ChatObjectTypeManager = chatObjectTypeManager;
        }

        public virtual async Task<List<ChatObject>> GetListByUserId(Guid userId)
        {
            return await Repository.GetListAsync(x => x.AppUserId == userId);
        }

        public virtual async Task<List<long>> GetIdListByUserId(Guid userId)
        {
            return (await Repository.GetQueryableAsync()).Where(x => x.AppUserId == userId).Select(x => x.Id).ToList();
        }

        public virtual Task<bool> IsAllowJoinRoomAsync(ChatObjectTypeEnums? objectType)
        {
            return Task.FromResult(ChatConsts.AllowJoinRoomObjectTypes.Any(x => x.Equals(objectType)));
        }

        public virtual async Task<List<long>> GetIdListByNameAsync(List<string> nameList)
        {
            var query = (await Repository.GetQueryableAsync())
                .Where(x => nameList.Contains(x.Name))
                .Select(x => x.Id)
                ;
            return await AsyncExecuter.ToListAsync(query);
        }

        public virtual async Task<List<ChatObject>> GetAllListAsync(ChatObjectTypeEnums objectType)
        {
            var query = (await Repository.GetQueryableAsync())
                .Where(x => x.ObjectType == objectType)
                ;
            return await AsyncExecuter.ToListAsync(query);
        }

        public virtual async Task<ChatObject> CreateRoomAsync(string name, List<long> memberIdList, long? ownerId)
        {
            var chatObjectType = await ChatObjectTypeManager.GetAsync(ChatObjectTypeEnums.Room);

            var room = await base.CreateAsync(new ChatObject(name, chatObjectType, null), isUnique: false);

            var roomInfo = await base.MapToOuputAsync(room);

            var session = await SessionGenerator.MakeAsync(roomInfo);

            session.SetOwner(room);

            foreach (var memberId in memberIdList)
            {
                session.AddSessionUnit(new SessionUnit(GuidGenerator.Create(), session, memberId, room));
            }

            room.OwnerSessionList.Add(session);

            await CurrentUnitOfWork.SaveChangesAsync();

            var roomOwner = ownerId.HasValue ? await GetItemByCacheAsync(ownerId.Value) : null;

            var members = await GetManyByCacheAsync(memberIdList.Take(3).ToList());

            await MessageSender.SendCmdMessageAsync(new MessageInput<CmdContentInfo>()
            {
                SenderId = room.Id,
                ReceiverId = room.Id,
                Content = new CmdContentInfo()
                {
                    Text = $"{roomOwner?.Name}创建群聊'{room.Name}',{members.Select(x => x.Name).JoinAsString("、")}等 {memberIdList.Count} 人加入群聊。",
                }
            });

            return room;
        }

        public virtual async Task<ChatObject> CreateRoomByAllUsersAsync(string name)
        {
            var query = (await Repository.GetQueryableAsync())
                .Where(x => x.ObjectType == ChatObjectTypeEnums.Personal)
                .Select(x => x.Id)
                ;
            var idList = await AsyncExecuter.ToListAsync(query);

            return await CreateRoomAsync(name, idList, null);
        }

        public virtual Task<ChatObject> CreateShopKeeperAsync(string name)
        {
            throw new NotImplementedException();
        }

        public virtual Task<ChatObject> CreateShopWaiterAsync(string name)
        {
            throw new NotImplementedException();
        }

        public virtual Task<ChatObject> CreateRobotAsync(string name)
        {
            throw new NotImplementedException();
        }

        public virtual Task<ChatObject> CreateSquareAsync(string name)
        {
            throw new NotImplementedException();
        }

        public virtual Task<ChatObject> CreateSubscriptionAsync(string name)
        {
            throw new NotImplementedException();
        }

        public virtual Task<ChatObject> CreateOfficialAsync(string name)
        {
            throw new NotImplementedException();
        }

        public virtual Task<ChatObject> CreateAnonymousAsync(string name)
        {
            throw new NotImplementedException();
        }
    }
}
