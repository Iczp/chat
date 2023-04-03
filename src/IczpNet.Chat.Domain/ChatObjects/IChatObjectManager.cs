﻿using IczpNet.AbpTrees;
using IczpNet.Chat.Enums;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IczpNet.Chat.ChatObjects
{
    public interface IChatObjectManager : ITreeManager<ChatObject, long, ChatObjectInfo>
    {
        //ChatObject GroupAssistant { get; }

        Task<List<ChatObject>> GetAllListAsync(ChatObjectTypeEnums objectType);

        Task<List<ChatObject>> GetListByUserId(Guid userId);

        Task<List<long>> GetIdListByUserId(Guid userId);

        //Task<ChatObject> GetAsync(Guid chatObjectId);

        //Task<ChatObjectInfo> GetItemByCacheAsync(Guid chatObjectId);

        //Task<List<ChatObjectInfo>> GetManyByCacheAsync(List<Guid> chatObjectIdList);

        //Task<List<ChatObject>> GetManyAsync(List<Guid> chatObjectIdList);

        Task<bool> IsAllowJoinRoomAsync(ChatObjectTypeEnums? objectType);

        Task<List<long>> GetIdListByNameAsync(List<string> nameList); //, List<ChatObjectTypes> objectTypes

        Task<ChatObject> CreateRoomAsync(string name, List<long> memberIdList, long? ownerId);

        Task<ChatObject> CreateRoomByAllUsersAsync(string name);

        Task<ChatObject> CreateShopKeeperAsync(string name);

        Task<ChatObject> CreateShopWaiterAsync(long shopKeeperId ,string name);

        Task<ChatObject> CreateRobotAsync(string name);

        Task<ChatObject> CreateSquareAsync(string name);

        Task<ChatObject> CreateSubscriptionAsync(string name);

        Task<ChatObject> CreateOfficialAsync(string name);

        Task<ChatObject> CreateAnonymousAsync(string name);

        //Task<ChatObjectInfo> GetGroupAssistantAsync();

    }
}
