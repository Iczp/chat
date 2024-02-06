﻿using IczpNet.Chat.Enums;
using System;

namespace IczpNet.Chat.ServiceStates;

public class ServiceStatusCacheItem
{
    public ServiceStatus? Status { get; set; }

    public DateTime? ActiveTime { get; set; }

    public long? ChatObjectId { get; set; }

    public string DeviceId { get; set; }

    public ServiceStatusCacheItem() { }

    public ServiceStatusCacheItem(long chatObjectId, string deviceId, ServiceStatus status)
    {
        ChatObjectId = chatObjectId;
        Status = status;
        DeviceId = deviceId;
        ActiveTime = DateTime.Now;
    }

    public override string ToString()
    {
        return $"ChatObjectId:{ChatObjectId},Status:{Status},ActiveTime:{ActiveTime}";
    }
}
