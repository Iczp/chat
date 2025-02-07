﻿using IczpNet.AbpCommons;
using IczpNet.Chat.SessionUnits;
using IczpNet.Chat.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;
using Volo.Abp.Settings;
using Volo.Abp.Uow;

namespace IczpNet.Chat.Follows;

public class FollowManager : DomainService, IFollowManager
{
    protected ISessionUnitManager SessionUnitManager { get; }

    protected IUnitOfWorkManager UnitOfWorkManager { get; }

    protected IRepository<Follow> Repository { get; }

    protected ISettingProvider SettingProvider { get; }

    public FollowManager(ISessionUnitManager sessionUnitManager,
        IUnitOfWorkManager unitOfWorkManager,
        IRepository<Follow> repository,
        ISettingProvider settingProvider)
    {
        SessionUnitManager = sessionUnitManager;
        UnitOfWorkManager = unitOfWorkManager;
        Repository = repository;
        SettingProvider = settingProvider;
    }

    public async Task<List<Follow>> GetFollowersAsync(Guid destinationSessionUnitId)
    {
        return (await Repository.GetQueryableAsync())
            .Where(x => x.DestinationId == destinationSessionUnitId)
            .ToList();
    }

    public async Task<List<Guid>> GetFollowerIdListAsync(Guid destinationSessionUnitId)
    {
        return (await Repository.GetQueryableAsync())
           .Where(x => x.DestinationId == destinationSessionUnitId)
           .Where(x => x.SessionUnitId != destinationSessionUnitId)
           .Select(x => x.SessionUnitId)
           .ToList();
    }

    public async Task<List<Guid>> GetFollowingIdListAsync(Guid sessionUnitId)
    {
        return (await Repository.GetQueryableAsync())
          .Where(x => x.SessionUnitId == sessionUnitId)
          .Where(x => x.DestinationId != sessionUnitId)
          .Select(x => x.DestinationId)
          .ToList();
    }

    public async Task<int> GetFollowingCountAsync(Guid ownerId)
    {
        return await Repository.CountAsync(x => x.SessionUnitId == ownerId);
    }

    public async Task<bool> CreateAsync(Guid sessionUnitId, List<Guid> idList)
    {
        var followingCount = await GetFollowingCountAsync(sessionUnitId);

        var maxFollowingCount = await SettingProvider.GetAsync<int>(ChatSettings.MaxFollowingCount);

        Assert.If(followingCount > maxFollowingCount, $"Max following count:{maxFollowingCount}");

        var owner = await SessionUnitManager.GetAsync(sessionUnitId);

        return await CreateAsync(owner, idList);
    }

    public async Task<bool> CreateAsync(SessionUnit owner, List<Guid> idList)
    {
        var destinationList = await SessionUnitManager.GetManyAsync(idList.Distinct().ToList());

        Assert.If(idList.Contains(owner.Id), $"Unable following oneself.");

        foreach (var item in destinationList)
        {
            Assert.If(owner.SessionId != item.SessionId, $"Not in the same session,id:{item.Id}");
        }

        var followedIdList = (await Repository.GetQueryableAsync())
             .Where(x => x.SessionUnitId == owner.Id)
             .Select(x => x.DestinationId)
             .ToList();

        var newList = idList.Except(followedIdList)
            .Where(X => X != owner.Id)
            .Select(x => new Follow(owner, x))
            .ToList();

        if (newList.Any())
        {
            await Repository.InsertManyAsync(newList, autoSave: true);
        }

        return true;
    }

    public async Task DeleteAsync(Guid sessionUnitId, List<Guid> idList)
    {
        await Repository.DeleteAsync(x => x.SessionUnitId == sessionUnitId && idList.Contains(x.DestinationId));
    }

    public async Task DeleteAsync(SessionUnit owner, List<Guid> idList)
    {
        await DeleteAsync(owner.Id, idList);
    }


}
