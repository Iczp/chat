﻿using IczpNet.Chat.ChatObjects.Dtos;
using System;

namespace IczpNet.Chat.SessionSections.SessionRequests.Dtos;

public class SessionRequestDetailDto : SessionRequestDto
{
    public virtual ChatObjectSimpleDto Owner { get; set; }
    public virtual string HandleMessage { get; set; }

    public virtual DateTime? HandleTime { get; set; }
}
