﻿using IczpNet.Chat.Enums;
using System;

namespace IczpNet.Chat.ServiceStates;

public class ServiceStatusCacheItem
{
    public ServiceStatus? Status { get; protected set; } = null;

    public DateTime? ActiveTime { get; protected set; }

    public long? ChatObjectId { get; protected set; }

    public ServiceStatusCacheItem() { }

    public ServiceStatusCacheItem(long chatObjectId, ServiceStatus status)
    {
        ChatObjectId = chatObjectId;
        Status = status;
        ActiveTime = DateTime.Now;
    }

    public override string ToString()
    {
        return $"ChatObjectId:{ChatObjectId},Status:{Status},ActiveTime:{ActiveTime}";
    }
}
