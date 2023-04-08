﻿using IczpNet.Chat.BaseAppServices;
using IczpNet.Chat.ChatObjectCategorys;
using IczpNet.Chat.ChatObjects;
using IczpNet.Chat.ChatObjects.Dtos;
using IczpNet.Chat.ChatObjectTypes;
using IczpNet.Chat.Enums;
using IczpNet.Chat.OfficialSections.Officials.Dtos;
using IczpNet.Chat.SessionSections.Sessions;
using IczpNet.Chat.SessionSections.SessionUnits;
using IczpNet.Chat.SessionSections.SessionUnits.Dtos;
using Microsoft.AspNetCore.Mvc;
using Pipelines.Sockets.Unofficial.Buffers;
using System;
using System.Threading.Tasks;

namespace IczpNet.Chat.OfficialSections.Officials;

public class OfficialAppService : ChatAppService, IOfficialAppService
{
    public virtual string InvitePolicyName { get; set; }
    public virtual string CreateOfficialPolicyName { get; set; }
    protected IOfficialManager OfficialManager { get; }
    protected IChatObjectTypeManager ChatObjectTypeManager { get; }
    protected ISessionUnitManager SessionUnitManager { get; }
    protected IChatObjectCategoryManager ChatObjectCategoryManager { get; }

    public OfficialAppService(IOfficialManager roomManager,
        IChatObjectCategoryManager chatObjectCategoryManager,
        IChatObjectTypeManager chatObjectTypeManager,
        ISessionUnitManager sessionUnitManager)
    {
        OfficialManager = roomManager;
        ChatObjectCategoryManager = chatObjectCategoryManager;
        ChatObjectTypeManager = chatObjectTypeManager;
        SessionUnitManager = sessionUnitManager;
    }


    [HttpPost]
    public virtual async Task<ChatObjectDto> CreateAsync(OfficialCreateInput input)
    {
        await CheckPolicyAsync(CreateOfficialPolicyName);

        var chatObjectType = await ChatObjectTypeManager.GetAsync(ChatObjectTypeEnums.Official);

        var official = await OfficialManager.CreateAsync(new ChatObject(input.Name, chatObjectType, null));

        return ObjectMapper.Map<ChatObject, ChatObjectDto>(official);
    }

    [HttpPost]
    public async Task<SessionUnitOwnerDto> EnableAsync(long ownerId, long destinationId)
    {

        var sessionUnit = await OfficialManager.EnableAsync(ownerId, destinationId);

        return ObjectMapper.Map<SessionUnit, SessionUnitOwnerDto>(sessionUnit);
    }

    [HttpPost]
    public async Task<SessionUnitOwnerDto> DisableAsync(Guid sessionUnitId)
    {
        var sessionUnit = await OfficialManager.DisableAsync(sessionUnitId);

        return ObjectMapper.Map<SessionUnit, SessionUnitOwnerDto>(sessionUnit);
    }

    [HttpPost]
    public async Task<SessionUnitOwnerDto> EnableByIdAsync(Guid sessionUnitId)
    {
        var sessionUnit = await OfficialManager.EnableAsync(sessionUnitId);

        return ObjectMapper.Map<SessionUnit, SessionUnitOwnerDto>(sessionUnit);
    }
}
