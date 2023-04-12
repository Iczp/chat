﻿using IczpNet.Chat.ChatObjects;
using IczpNet.Chat.Enums;
using IczpNet.Chat.SessionSections.Friendships;
using IczpNet.Chat.SessionSections.OpenedRecorders;
using IczpNet.Chat.SessionSections.SessionRoles;
using IczpNet.Chat.SessionSections.SessionTags;
using IczpNet.Chat.SessionSections.SessionUnits;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IczpNet.Chat.SessionSections.Sessions
{
    public interface ISessionManager
    {
        Task<Session> GetAsync(Guid sessionId);

        Task<Session> GetByOwnerIdAsync(long roomId);

        Task<Session> GetByKeyAsync(string sessionKey);

        Task<bool> IsFriendshipAsync(long ownerId, long destinationId);

        Task<IQueryable<Session>> InSameAsync(long sourceChatObjectId, long destinationChatObjectId, ChatObjectTypeEnums? chatObjectType = null);

        Task<Friendship> CreateFriendshipAsync(long ownerId, long destinationId, bool isPassive, Guid? friendshipRequestId);

        Task<Friendship> CreateFriendshipAsync(IChatObject owner, IChatObject destination, bool isPassive, Guid? friendshipRequestId);

        Task<DateTime> DeleteFriendshipAsync(long ownerId, long destinationId);

        Task<DateTime?> HandleRequestAsync(Guid friendshipRequestId, bool isAgreed, string handlMessage);

        Task<OpenedRecorder> SetOpenedAsync(long ownerId, long destinationId, long messageId, string deviceId);

        Task<SessionTag> AddTagAsync(Session entity, SessionTag sessionTag);

        Task RemoveTagAsync(Guid tagId);

        Task<SessionTag> AddTagMembersAsync(Guid tagId, List<Guid> sessionUnitIdList);

        Task<SessionTag> RemoveTagMembersAsync(Guid tagId, List<Guid> sessionUnitIdList);

        Task<SessionRole> AddRoleAsync(Session entity, SessionRole sessionRole);

        Task RemoveRoleAsync(Guid roleId);

        Task<SessionRole> AddRoleMembersAsync(Guid roleId, List<Guid> sessionUnitIdList);

        Task<SessionRole> RemoveRoleMembersAsync(Guid roleId, List<Guid> sessionUnitIdList);
        
    }
}
