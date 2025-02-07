﻿using IczpNet.Chat.Enums;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace IczpNet.Chat.ServiceStates;

public interface IServiceStateManager : ITransientDependency
{
    Task<List<ServiceStatusCacheItem>> GetAsync(long chatObjectId);

    Task<ServiceStatus?> GetStatusAsync(long chatObjectId);

    Task<List<ServiceStatusCacheItem>> SetAsync(long chatObjectId, string deviceId, ServiceStatus status);

    Task RemoveDeviceAsync(long chatObjectId, string deviceId);

    Task<Dictionary<long, List<ServiceStatusCacheItem>>> SetAppUserIdAsync(Guid appUserId, string deviceId, ServiceStatus status);

    Task RemoveAppUserIdAsync(Guid appUserId, string deviceId);

    /// <summary>
    /// 是否空闲
    /// </summary>
    /// <param name="chatObjectId"></param>
    /// <returns></returns>
    Task<bool> IsIdledAsync(long chatObjectId);

    /// <summary>
    /// 是否在线
    /// </summary>
    /// <param name="chatObjectId"></param>
    /// <returns></returns>
    Task<bool> IsOnlineAsync(long chatObjectId);
}
