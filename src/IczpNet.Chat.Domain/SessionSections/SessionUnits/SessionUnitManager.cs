﻿using IczpNet.Chat.Enums;
using IczpNet.Chat.MessageSections.Messages;
using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Caching;
using Volo.Abp.Domain.Services;
using IczpNet.AbpCommons.Extensions;
using IczpNet.AbpCommons;
using IczpNet.Chat.Follows;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using IczpNet.Chat.ChatObjects;
using IczpNet.Chat.SessionSections.SessionUnitCounters;

namespace IczpNet.Chat.SessionSections.SessionUnits;

public class SessionUnitManager : DomainService, ISessionUnitManager
{
    protected ISessionUnitRepository Repository { get; }
    protected IMessageRepository MessageRepository { get; }
    protected IDistributedCache<List<SessionUnitCacheItem>, string> UnitListCache { get; }
    protected IDistributedCache<string, Guid> UnitCountCache { get; }
    protected IFollowManager FollowManager => LazyServiceProvider.LazyGetRequiredService<IFollowManager>();
    protected IChatObjectRepository ChatObjectRepository { get; }

    public SessionUnitManager(
        ISessionUnitRepository repository,
        IMessageRepository messageRepository,
        IDistributedCache<List<SessionUnitCacheItem>, string> unitListCache,
        IDistributedCache<string, Guid> unitCountCache,
        IChatObjectRepository chatObjectRepository)
    {
        Repository = repository;
        MessageRepository = messageRepository;
        UnitListCache = unitListCache;
        UnitCountCache = unitCountCache;
        ChatObjectRepository = chatObjectRepository;
    }

    protected virtual async Task<SessionUnit> SetEntityAsync(SessionUnit entity, Action<SessionUnit> action = null, bool autoSave = false)
    {
        action?.Invoke(entity);

        return await Repository.UpdateAsync(entity, autoSave: autoSave);
    }

    public virtual async Task<Guid?> FindIdAsync(Expression<Func<SessionUnit, bool>> predicate)
    {
        return (await Repository.GetQueryableAsync())
            .Where(predicate)
            .Select(x => (Guid?)x.Id)
            .FirstOrDefault();
        ;
    }
    public virtual Task<Guid?> FindIdAsync(long ownerId, long destinactionId)
    {
        return FindIdAsync(x => x.OwnerId == ownerId && x.DestinationId == destinactionId);
    }

    public virtual Task<SessionUnit> FindAsync(long ownerId, long destinactionId)
    {
        return FindAsync(x => x.OwnerId == ownerId && x.DestinationId == destinactionId);
    }

    public virtual Task<SessionUnit> FindAsync(Expression<Func<SessionUnit, bool>> predicate)
    {
        return Repository.FindAsync(predicate);
    }

    public virtual Task<SessionUnit> FindBySessionIdAsync(Guid sessionId, long ownerId)
    {
        return FindAsync(x => x.SessionId == sessionId && x.OwnerId == ownerId);
    }

    public virtual Task<SessionUnit> GetAsync(Guid id)
    {
        return Repository.GetAsync(id);
    }

    public virtual async Task<List<SessionUnit>> GetManyAsync(List<Guid> idList)
    {
        var result = new List<SessionUnit>();

        foreach (var id in idList)
        {
            result.Add(await GetAsync(id));
        }
        return result;
    }

    public virtual async Task<SessionUnit> CreateIfNotContainsAsync(SessionUnit sessionUnit)
    {
        var entity = await FindAsync(sessionUnit.OwnerId, sessionUnit.DestinationId.Value);

        entity ??= await Repository.InsertAsync(sessionUnit, autoSave: true);

        return entity;
    }

    public Task<SessionUnit> SetMemberNameAsync(SessionUnit entity, string memberName)
    {
        return SetEntityAsync(entity, x => x.SetMemberName(memberName));
    }

    public Task<SessionUnit> SetRenameAsync(SessionUnit entity, string rename)
    {
        return SetEntityAsync(entity, x => x.SetRename(rename));
    }

    public virtual Task<SessionUnit> SetToppingAsync(SessionUnit entity, bool isTopping)
    {
        return SetEntityAsync(entity, x => x.SetTopping(isTopping));
    }

    public virtual async Task<SessionUnit> SetReadedMessageIdAsync(SessionUnit entity, bool isForce = false, long? messageId = null)
    {
        var isNullOrZero = messageId == null || messageId == 0;

        var lastMessageId = isNullOrZero ? entity.LastMessageId.Value : messageId.Value;

        if (!isNullOrZero)
        {
            var message = await MessageRepository.GetAsync(lastMessageId);

            Assert.If(entity.SessionId != message.SessionId, $"Not in same session,messageId:{messageId}");
        }

        await SetEntityAsync(entity, x => x.SetReadedMessageId(lastMessageId, isForce = false));

        await UpdateCacheItemsAsync(entity, items =>
        {
            var item = items.FirstOrDefault(x => x.Id != entity.Id);

            if (item == null)
            {
                return false;
            }

            if (isForce || lastMessageId > item.ReadedMessageId.GetValueOrDefault())
            {
                item.ReadedMessageId = lastMessageId;
            }

            item.PublicBadge = 0;
            item.PrivateBadge = 0;
            item.RemindAllCount = 0;
            item.FollowingCount = 0;
            //item.LastMessageId = messageId;

            return true;
        });

        return entity;
    }

    public virtual Task<SessionUnit> SetImmersedAsync(SessionUnit entity, bool isImmersed)
    {
        return SetEntityAsync(entity, x => x.SetImmersed(isImmersed));
    }

    public virtual Task<SessionUnit> RemoveAsync(SessionUnit entity)
    {
        return SetEntityAsync(entity, x => x.Remove(Clock.Now));
    }

    public virtual Task<SessionUnit> KillAsync(SessionUnit entity)
    {
        return SetEntityAsync(entity, x => x.Kill(Clock.Now));
    }

    public virtual Task<SessionUnit> ClearMessageAsync(SessionUnit entity)
    {
        return SetEntityAsync(entity, x => x.ClearMessage(Clock.Now));
    }

    public virtual Task<SessionUnit> DeleteMessageAsync(SessionUnit entity, long messageId)
    {
        throw new NotImplementedException();
    }

    protected virtual async Task<int> GetBadgeAsync(Func<IQueryable<SessionUnit>, IQueryable<SessionUnit>> queryAction)
    {
        var query = queryAction.Invoke(await Repository.GetQueryableAsync());

        var badge = query.Select(x => new
        {
            PublicBadge = x.Session.MessageList.Count(d =>
                !d.IsPrivate &&
                //!x.IsRollbacked &&
                d.SenderId != x.OwnerId &&
                (x.ReadedMessageId == null || d.Id > x.ReadedMessageId) &&
                (!x.HistoryFristTime.HasValue || d.CreationTime > x.HistoryFristTime) &&
                (!x.HistoryLastTime.HasValue || d.CreationTime < x.HistoryLastTime) &&
                (!x.ClearTime.HasValue || d.CreationTime > x.ClearTime)),
            PrivateBadge = x.Session.MessageList.Count(d =>
                d.IsPrivate && d.ReceiverId == x.OwnerId &&
                //!x.IsRollbacked &&
                d.SenderId != x.OwnerId &&
                (x.ReadedMessageId == null || d.Id > x.ReadedMessageId) &&
                (!x.HistoryFristTime.HasValue || d.CreationTime > x.HistoryFristTime) &&
                (!x.HistoryLastTime.HasValue || d.CreationTime < x.HistoryLastTime) &&
                (!x.ClearTime.HasValue || d.CreationTime > x.ClearTime)),
        })
        .Select(x => x.PublicBadge + x.PrivateBadge)
        .Where(x => x > 0)
        .ToList()
        .Sum();

        return badge;
    }

    public virtual Task<int> GetBadgeByOwnerIdAsync(long ownerId, bool? isImmersed = null)
    {
        return GetBadgeAsync(q =>
            q.Where(x => x.OwnerId == ownerId)
            .WhereIf(isImmersed.HasValue, x => x.IsImmersed == isImmersed));
    }

    public virtual Task<int> GetBadgeByIdAsync(Guid sessionUnitId, bool? isImmersed = null)
    {
        return GetBadgeAsync(q =>
            q.Where(x => x.Id == sessionUnitId)
            .WhereIf(isImmersed.HasValue, x => x.IsImmersed == isImmersed));
    }

    public virtual async Task<Dictionary<Guid, int>> GetBadgeByIdAsync(List<Guid> sessionUnitIdList, long minMessageId = 0, bool? isImmersed = null)
    {
        var badges = (await Repository.GetQueryableAsync())
            .Where(x => sessionUnitIdList.Contains(x.Id))
            .WhereIf(isImmersed.HasValue, x => x.IsImmersed == isImmersed)
            .Select(x => new
            {
                x.Id,
                x.OwnerId,
                Messages = x.Session.MessageList.Where(d =>
                    d.Id > minMessageId &&
                    d.SenderId != x.OwnerId &&
                    (x.ReadedMessageId == null || d.Id > x.ReadedMessageId) &&
                    (!x.HistoryFristTime.HasValue || d.CreationTime > x.HistoryFristTime) &&
                    (!x.HistoryLastTime.HasValue || d.CreationTime < x.HistoryLastTime) &&
                    (!x.ClearTime.HasValue || d.CreationTime > x.ClearTime))
            })
            .Select(x => new
            {
                x.Id,
                PublicBadge = x.Messages.Count(d => !d.IsPrivate),
                PrivateBadge = x.Messages.Count(d => d.IsPrivate && d.ReceiverId == x.OwnerId),
            })
            //.GroupBy(x => x.Id).ToDictionary(x => x.Key, x => x.Sum(d => d.PublicBadge + d.PrivateBadge))
            .ToDictionary(x => x.Id, x => x.PrivateBadge + x.PublicBadge)
            ;

        return badges;
    }

    public virtual async Task<Dictionary<Guid, SessionUnitStatModel>> GetStatsAsync(List<Guid> sessionUnitIdList, long minMessageId = 0, bool? isImmersed = null)
    {
        return await GetStatsByLinqAsync(sessionUnitIdList, minMessageId, isImmersed);
    }

    protected virtual async Task<Dictionary<Guid, SessionUnitStatModel>> GetStatsByLinqAsync(List<Guid> sessionUnitIdList, long minMessageId = 0, bool? isImmersed = null)
    {
        return (await Repository.GetQueryableAsync())
            .Where(x => sessionUnitIdList.Contains(x.Id))
            .WhereIf(isImmersed.HasValue, x => x.IsImmersed == isImmersed)
            .Select(x => new
            {
                x.Id,
                x.OwnerId,
                x.OwnerFollowList,
                Messages = x.Session.MessageList.Where(d =>
                    d.Id > minMessageId &&
                    d.SenderId != x.OwnerId &&
                    (x.ReadedMessageId == null || d.Id > x.ReadedMessageId) &&
                    (x.HistoryFristTime == null || d.CreationTime > x.HistoryFristTime) &&
                    (x.HistoryLastTime == null || d.CreationTime < x.HistoryLastTime) &&
                    (x.ClearTime == null || d.CreationTime > x.ClearTime))
            })
            .Select(x => new SessionUnitStatModel
            {
                Id = x.Id,
                PublicBadge = x.Messages.Count(d => !d.IsPrivate),
                PrivateBadge = x.Messages.Count(d => d.IsPrivate && d.ReceiverId == x.OwnerId),
                FollowingCount = x.Messages.Count(d => x.OwnerFollowList.Any(d => d.DestinationId == x.Id)),
                RemindAllCount = x.Messages.Count(d => d.IsRemindAll && !d.IsRollbacked),
                RemindMeCount = x.Messages.Count(d => d.MessageReminderList.Any(g => g.SessionUnitId == x.Id))
            })
            //.GroupBy(x => x.Id).ToDictionary(x => x.Key, x => x.Sum(d => d.PublicBadge + d.PrivateBadge))
            .ToDictionary(x => x.Id)
            ;
    }

    public virtual async Task<Dictionary<Guid, int>> GetReminderCountByIdAsync(List<Guid> sessionUnitIdList, long minMessageId = 0, bool? isImmersed = null)
    {
        var query = (await Repository.GetQueryableAsync())
            .Where(x => sessionUnitIdList.Contains(x.Id))
            .WhereIf(isImmersed.HasValue, x => x.IsImmersed == isImmersed);
        //return ReminderList.AsQueryable().Select(x => x.Message).Where(x => !x.IsRollbacked).Count(new SessionUnitMessageSpecification(this).ToExpression());
        var reminds = query.Select(x => new
        {
            x.Id,
            RemindMeCount = x.ReminderList.Select(x => x.Message)
                .Where(d => d.Id > minMessageId)
                .Where(d =>
                    d.SenderId != x.OwnerId &&
                    (x.ReadedMessageId == null || d.Id > x.ReadedMessageId) &&
                    (!x.HistoryFristTime.HasValue || d.CreationTime > x.HistoryFristTime) &&
                    (!x.HistoryLastTime.HasValue || d.CreationTime < x.HistoryLastTime) &&
                    (!x.ClearTime.HasValue || d.CreationTime > x.ClearTime)
                )
                .Count(),
            RemindAllCount = x.Session.MessageList
                .Where(d => d.IsRemindAll && !d.IsRollbacked)
                .Where(d => d.Id > minMessageId)
                .Where(d =>
                    d.SenderId != x.OwnerId &&
                    (x.ReadedMessageId == null || d.Id > x.ReadedMessageId) &&
                    (!x.HistoryFristTime.HasValue || d.CreationTime > x.HistoryFristTime) &&
                    (!x.HistoryLastTime.HasValue || d.CreationTime < x.HistoryLastTime) &&
                    (!x.ClearTime.HasValue || d.CreationTime > x.ClearTime)
                )
                .Count(),
        })
            //.GroupBy(x => x.Id).ToDictionary(x => x.Key, x => x.Sum(d => d.RemindMeCount + d.RemindAllCount))
            .ToDictionary(x => x.Id, x => x.RemindMeCount + x.RemindAllCount)
            ;

        return reminds;
    }

    public virtual async Task<Dictionary<Guid, int>> GetFollowingCountByIdAsync(List<Guid> sessionUnitIdList, long minMessageId = 0, bool? isImmersed = null)
    {
        var query = (await Repository.GetQueryableAsync())
            .Where(x => sessionUnitIdList.Contains(x.Id))
            .WhereIf(isImmersed.HasValue, x => x.IsImmersed == isImmersed);

        var follows = query.Select(x => new
        {
            x.Id,
            FollowingCount = x.Session.MessageList
                .Where(d => x.OwnerFollowList.Any(d => d.DestinationId == x.Id))
                .Where(d => d.Id > minMessageId)
                .Where(d =>
                    d.SenderId != x.OwnerId &&
                    (x.ReadedMessageId == null || d.Id > x.ReadedMessageId) &&
                    (!x.HistoryFristTime.HasValue || d.CreationTime > x.HistoryFristTime) &&
                    (!x.HistoryLastTime.HasValue || d.CreationTime < x.HistoryLastTime) &&
                    (!x.ClearTime.HasValue || d.CreationTime > x.ClearTime)
                )
                .Count(),
        })
            .ToDictionary(x => x.Id, x => x.FollowingCount)
            //.GroupBy(x => x.Id).ToDictionary(x => x.Key, x => x.Sum(d => d.FollowingCount))
            ;

        return follows;
    }

    public virtual async Task<int> GetCountAsync(Guid sessionId)
    {
        var value = await UnitCountCache.GetOrAddAsync(sessionId, async () =>
        {
            var count = (await Repository.GetQueryableAsync())
                .Where(x => x.SessionId == sessionId)
                .Where(SessionUnit.GetActivePredicate(Clock.Now))
                .Count();
            return count.ToString();
        });
        return int.Parse(value);
    }

    public virtual Task<List<SessionUnitCacheItem>> GetCacheListAsync(string sessionUnitCachKey)
    {
        return UnitListCache.GetAsync(sessionUnitCachKey);
    }

    public virtual Task<List<SessionUnitCacheItem>> GetOrAddCacheListAsync(Guid sessionId)
    {
        return UnitListCache.GetOrAddAsync($"{new SessionUnitCacheKey(sessionId)}", () => GetListBySessionIdAsync(sessionId));
    }

    public virtual async Task SetCacheListBySessionIdAsync(Guid sessionId, List<SessionUnitCacheItem> sessionUnitList)
    {
        //var sessionUnitInfoList = await GetListBySessionIdAsync(sessionId);

        await SetCacheListAsync($"{new SessionUnitCacheKey(sessionId)}", sessionUnitList);
    }

    public virtual async Task SetCacheListAsync(string cacheKey, List<SessionUnitCacheItem> sessionUnitList, DistributedCacheEntryOptions options = null, bool? hideErrors = null, bool considerUow = false, CancellationToken token = default)
    {
        await UnitListCache.SetAsync(cacheKey, sessionUnitList, options, hideErrors, considerUow, token);
    }

    public virtual async Task<List<SessionUnitCacheItem>> GetListBySessionIdAsync(Guid sessionId)
    {
        var list = (await Repository.GetQueryableAsync())
            .Where(SessionUnit.GetActivePredicate(Clock.Now))
            .Where(x => x.SessionId == sessionId)
            .Select(x => new SessionUnitCacheItem()
            {
                Id = x.Id,
                SessionId = x.SessionId,
                OwnerId = x.OwnerId,
                DestinationId = x.DestinationId,
                //DestinationObjectType = x.DestinationObjectType,
                IsPublic = x.IsPublic,
                ReadedMessageId = x.ReadedMessageId,
                PublicBadge = x.Counter == null ? 0 : x.Counter.PublicBadge,
                PrivateBadge = x.Counter == null ? 0 : x.Counter.PrivateBadge,
                RemindAllCount = x.Counter == null ? 0 : x.Counter.RemindAllCount,
                RemindMeCount = x.Counter == null ? 0 : x.Counter.RemindMeCount,
                FollowingCount = x.Counter == null ? 0 : x.Counter.FollowingCount,
                LastMessageId = x.Counter == null ? null : x.Counter.LastMessageId,
            })
            .ToList();

        await UnitCountCache.SetAsync(sessionId, list.Where(x => x.IsPublic).Count().ToString());

        return list;
    }

    public virtual async Task RemoveCacheListBySessionIdAsync(Guid sessionId)
    {
        await UnitListCache.RemoveAsync($"{new SessionUnitCacheKey(sessionId)}");
    }

    private async Task<IQueryable<SessionUnit>> GetOwnerQueryableAsync(long ownerId, List<ChatObjectTypeEnums> destinationObjectTypeList = null)
    {
        return (await Repository.GetQueryableAsync())
              .Where(x => x.OwnerId.Equals(ownerId) && !x.IsKilled && x.IsEnabled)
              .WhereIf(destinationObjectTypeList.IsAny(), x => destinationObjectTypeList.Contains(x.DestinationObjectType.Value));

    }

    public virtual async Task<IQueryable<SessionUnit>> GetSameSessionQeuryableAsync(long sourceChatObjectId, long targetChatObjectId, List<ChatObjectTypeEnums> chatObjectTypeList = null)
    {
        var targetSessionIdList = (await GetOwnerQueryableAsync(targetChatObjectId, chatObjectTypeList))
            .Select(x => x.SessionId);

        var sourceQuery = (await GetOwnerQueryableAsync(sourceChatObjectId, chatObjectTypeList))
            .Where(x => targetSessionIdList.Contains(x.SessionId))
            ;

        return sourceQuery;
    }

    public virtual async Task<int> GetSameSessionCountAsync(long sourceChatObjectId, long targetChatObjectId, List<ChatObjectTypeEnums> chatObjectTypeList = null)
    {
        var query = await GetSameSessionQeuryableAsync(sourceChatObjectId, targetChatObjectId, chatObjectTypeList);

        return await AsyncExecuter.CountAsync(query);
    }

    public virtual async Task<IQueryable<SessionUnit>> GetSameDestinationQeuryableAsync(long sourceChatObjectId, long targetChatObjectId, List<ChatObjectTypeEnums> chatObjectTypeList = null)
    {
        var destinationIdList = (await GetOwnerQueryableAsync(targetChatObjectId, chatObjectTypeList))
            .Where(x => x.DestinationId.HasValue)
            .Select(x => x.DestinationId.Value);

        var sourceQuery = (await GetOwnerQueryableAsync(sourceChatObjectId, chatObjectTypeList))
            .Where(x => x.DestinationId.HasValue)
            .Where(x => destinationIdList.Contains(x.DestinationId.Value))
            ;

        return sourceQuery;
    }

    public virtual async Task<int> GetSameDestinationCountAsync(long sourceChatObjectId, long targetChatObjectId, List<ChatObjectTypeEnums> chatObjectTypeList = null)
    {
        var query = await GetSameDestinationQeuryableAsync(sourceChatObjectId, targetChatObjectId, chatObjectTypeList);

        return await AsyncExecuter.CountAsync(query);
    }


    public virtual async Task<int> IncrementFollowingCountAsync(SessionUnit senderSessionUnit, Message message)
    {
        var ownerSessionUnitIdList = await FollowManager.GetFollowerIdListAsync(senderSessionUnit.Id);

        if (ownerSessionUnitIdList.Any())
        {
            ownerSessionUnitIdList.Remove(senderSessionUnit.Id);

            return await Repository.IncrementFollowingCountAsync(senderSessionUnit.SessionId.Value, message.CreationTime, destinationSessionUnitIdList: ownerSessionUnitIdList);
        }
        return 0;
    }

    protected virtual async Task UpdateCacheItemsAsync(SessionUnit senderSessionUnit, Func<List<SessionUnitCacheItem>, bool> action)
    {
        var stopwatch = Stopwatch.StartNew();

        var sessionUnitList = await GetOrAddCacheListAsync(senderSessionUnit.SessionId.Value);

        if (action.Invoke(sessionUnitList))
        {
            await SetCacheListBySessionIdAsync(senderSessionUnit.SessionId.Value, sessionUnitList);
        }

        stopwatch.Stop();

        Logger.LogInformation($"UpdateCacheItems stopwatch: {stopwatch.ElapsedMilliseconds}ms.");
    }

    public virtual async Task<int> UpdateCachesAsync(SessionUnit senderSessionUnit, Message message)
    {
        int count = 0;

        await UpdateCacheItemsAsync(senderSessionUnit, items =>
        {
            var self = items.FirstOrDefault(x => x.Id == senderSessionUnit.Id);

            if (self != null)
            {
                self.LastMessageId = message.Id;
            }

            var others = items.Where(x => x.Id != senderSessionUnit.Id).ToList();

            foreach (var item in others)
            {
                //item.RemindMeCount++;
                //item.FollowingCount++;
                item.PublicBadge++;
                item.RemindAllCount++;
                item.LastMessageId = message.Id;
            }
            count = others.Count;

            return true;
        });

        Logger.LogInformation($"BatchUpdateCacheAsync:{count}");

        return count;
    }

    public virtual async Task<int> BatchUpdateAsync(SessionUnit senderSessionUnit, Message message)
    {
        var stopwatch = Stopwatch.StartNew();

        var result = await Repository.IncrementPublicBadgeAndRemindAllCountAndUpdateLastMessageIdAsync(
               sessionId: senderSessionUnit.SessionId.Value,
               lastMessageId: message.Id,
               messageCreationTime: message.CreationTime,
               senderSessionUnit.Id,
               isRemindAll: message.IsRemindAll
               );

        stopwatch.Stop();

        Logger.LogInformation($"BatchUpdateLastMessageIdAndPublicBadgeAndRemindAllCount:{result}, stopwatch: {stopwatch.ElapsedMilliseconds}ms.");

        return result;
    }

    public async Task<List<Guid>> GetIdListByNameAsync(Guid sessionId, List<string> nameList)
    {
        var chatObjectIds = (await ChatObjectRepository.GetQueryableAsync())
            .Where(x => nameList.Contains(x.Name))
            .Select(x => x.Id)
            ;

        return (await Repository.GetQueryableAsync())
            .Where(x => x.SessionId == sessionId)
            .Where(x => nameList.Contains(x.MemberName) || chatObjectIds.Contains(x.OwnerId))
            .Where(SessionUnit.GetActivePredicate(null))
            .Select(x => x.Id)
            .ToList();
    }

    public async Task<Dictionary<Guid, string>> GetIdListByName_BackAsync(Guid sessionId, List<string> nameList)
    {
        //var chatObjectDicts = (await ChatObjectRepository.GetQueryableAsync())
        //    .Where(x => nameList.Contains(x.Name))
        //    .Select(x => new ChatObjectIdName()
        //    {
        //        Id = x.Id,
        //        Name = x.Name
        //    })
        //    .ToDictionary(x => x.Id);
        //;

        //var chatObjectIds = chatObjectDicts.Keys.ToList();

        //var dicts = (await Repository.GetQueryableAsync())
        //    .Where(x => x.SessionId == sessionId)
        //    .Where(x => nameList.Contains(x.MemberName) || chatObjectIds.Contains(x.OwnerId))
        //    .Where(SessionUnit.GetActivePredicate(null))
        //    .Select(x => new { x.Id, x.OwnerId })
        //    .ToDictionary(x => x.Id, x => chatObjectDicts[x.OwnerId]);

        //return dicts;

        return (await Repository.GetQueryableAsync())
            .Where(x => x.SessionId == sessionId)
            .Where(x => nameList.Contains(x.MemberName) || nameList.Contains(x.OwnerName))
            .Where(SessionUnit.GetActivePredicate(null))
            .ToDictionary(x => x.Id, x => !string.IsNullOrEmpty(x.MemberName) ? x.MemberName : x.OwnerName);
    }

    /// <inheritdoc/>
    public async Task<int> IncremenetAsync(SessionUnitIncrementArgs args)
    {
        Logger.LogInformation($"Incremenet args:{args},starting.....................................");

        var stopwatch = Stopwatch.StartNew();

        var counter = new List<int>();

        if (args.IsPrivate)
        {
            var count = await Repository.IncrementPrivateBadgeAndUpdateLastMessageIdAsync(
                sessionId: args.SessionId,
                lastMessageId: args.LastMessageId,
                messageCreationTime: args.MessageCreationTime,
                senderSessionUnitId: args.SenderSessionUnitId,
                destinationSessionUnitIdList: args.PrivateBadgeSessionUnitIdList);

            Logger.LogInformation($"IncrementPrivateBadgeAndUpdateLastMessageId count:{count}");

            counter.Add(count);
        }
        else
        {
            var count = await Repository.IncrementPublicBadgeAndRemindAllCountAndUpdateLastMessageIdAsync(
                sessionId: args.SessionId,
                lastMessageId: args.LastMessageId,
                messageCreationTime: args.MessageCreationTime,
                senderSessionUnitId: args.SenderSessionUnitId,
                isRemindAll: args.IsRemindAll);

            Logger.LogInformation($"IncrementPublicBadgeAndRemindAllCountAndUpdateLastMessageIdAsync count:{count}");

            counter.Add(count);
        }

        if (args.RemindSessionUnitIdList.IsAny())
        {
            var count = await Repository.IncrementRemindMeCountAsync(
                sessionId: args.SessionId,
                messageCreationTime: args.MessageCreationTime,
                destinationSessionUnitIdList: args.RemindSessionUnitIdList);

            Logger.LogInformation($"IncrementRemindMeCountAsync count:{count}");

            counter.Add(count);
        }

        if (args.FollowingSessionUnitIdList.IsAny())
        {
            var count = await Repository.IncrementFollowingCountAsync(
                sessionId: args.SessionId,
                messageCreationTime: args.MessageCreationTime,
                destinationSessionUnitIdList: args.FollowingSessionUnitIdList);

            Logger.LogInformation($"IncrementFollowingCountAsync count:{count}");

            counter.Add(count);
        }

        stopwatch.Stop();

        var totalCount = counter.Sum();

        Logger.LogInformation($"Incremenet totalCount:{totalCount}, stopwatch: {stopwatch.ElapsedMilliseconds}ms.");

        return totalCount;
    }
}
