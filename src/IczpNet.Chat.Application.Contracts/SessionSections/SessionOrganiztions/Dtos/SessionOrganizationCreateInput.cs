﻿using System;

namespace IczpNet.Chat.SessionSections.SessionOrganiztions.Dtos;

/// <summary>
/// SessionOrganization CreateInput
/// </summary>
public class SessionOrganizationCreateInput : SessionOrganizationCreateBySessionUnitInput, ISessionId
{
    public virtual Guid? SessionId { get; set; }
}
