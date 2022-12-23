﻿using System;
using Volo.Abp.Application.Dtos;

namespace IczpNet.Chat.SessionSections.SessionUnits.Dtos
{
    public class SessionUnitGetListInput : PagedAndSortedResultRequestDto
    {
        public virtual Guid? OwnerId { get; set; }

        public virtual Guid? DestinationId { get;  set; }

        public virtual bool? IsKilled { get; set; }

        //public virtual JoinWays? JoinWay { get; set; }

        //public virtual Guid? InviterId { get; set; }

        public virtual bool IsOrderByBadge { get; set; }
    }
}