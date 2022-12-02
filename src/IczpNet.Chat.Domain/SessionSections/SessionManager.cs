﻿using IczpNet.AbpCommons;
using IczpNet.Chat.ChatObjects;
using IczpNet.Chat.DataFilters;
using IczpNet.Chat.SessionSections.FriendshipRequests;
using IczpNet.Chat.SessionSections.Friendships;
using System;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;

namespace IczpNet.Chat.SessionSections
{
    public class SessionManager : DomainService, ISessionManager
    {
        protected IRepository<Friendship, Guid> FriendshipRepository { get; }
        protected IRepository<FriendshipRequest, Guid> FriendshipRequestRepository { get; }
        protected IChatObjectManager ChatObjectManager { get; }

        public SessionManager(
            IRepository<Friendship, Guid> friendshipRepository,
            IChatObjectManager chatObjectManager,
            IRepository<FriendshipRequest, Guid> friendshipRequestRepository)
        {
            FriendshipRepository = friendshipRepository;
            ChatObjectManager = chatObjectManager;
            FriendshipRequestRepository = friendshipRequestRepository;
        }

        public Task<bool> IsFriendshipAsync(Guid ownerId, Guid friendId)
        {
            return FriendshipRepository.AnyAsync(x => x.OwnerId == ownerId && x.FriendId == friendId);
        }

        public async Task<Friendship> CreateFriendshipAsync(Guid ownerId, Guid friendId)
        {
            var owner = await ChatObjectManager.GetAsync(ownerId);

            var friend = await ChatObjectManager.GetAsync(friendId);

            return await CreateFriendshipAsync(owner, friend);
        }

        public async Task<Friendship> CreateFriendshipAsync(ChatObject owner, ChatObject friend)
        {
            Assert.NotNull(owner, nameof(owner));

            Assert.NotNull(friend, nameof(friend));

            var entity = await FriendshipRepository.FindAsync(x => x.OwnerId == owner.Id && x.FriendId == friend.Id);

            entity ??= await FriendshipRepository.InsertAsync(new Friendship(owner, friend), autoSave: true);

            return entity;
        }

        public Task<DateTime> DeleteFriendshipAsync(Guid ownerId, Guid friendId)
        {
            throw new NotImplementedException();
        }

        public Task DeleteFriendshipRequestAsync(Guid ownerId, Guid destinationId)
        {
            return FriendshipRequestRepository.DeleteAsync(x => x.OwnerId == ownerId && x.DestinationId == destinationId && !x.IsHandled);
        }

        public async Task<DateTime?> HandlRequestAsync(Guid friendshipRequestId, bool isAgreed, string handlMessage)
        {
            var friendshipRequest = await FriendshipRequestRepository.GetAsync(friendshipRequestId);

            Assert.If(friendshipRequest.IsHandled, $"Already been handled:IsAgreed={friendshipRequest.IsAgreed}");

            if (isAgreed)
            {
                var friendship = await CreateFriendshipAsync(friendshipRequest.Owner, friendshipRequest.Destination);

                friendshipRequest.AgreeRequest(friendship, handlMessage);
            }
            else
            {
                friendshipRequest.DisagreeRequest(handlMessage);
            }

            await DeleteFriendshipRequestAsync(friendshipRequest.OwnerId, friendshipRequest.DestinationId.Value);

            return friendshipRequest.HandlTime;
        }


    }
}
