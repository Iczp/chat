﻿using IczpNet.Chat.Enums;
using System;
using System.ComponentModel;
using Volo.Abp.Application.Dtos;

namespace IczpNet.Chat.SessionSections.SessionUnits.Dtos
{
    public class SessionUnitGetListInput : PagedAndSortedResultRequestDto
    {
        public virtual long? OwnerId { get; set; }

        public virtual long? DestinationId { get; set; }

        public virtual ChatObjectTypeEnums? DestinationObjectType { get; set; }

        public virtual bool? IsCreator { get; set; }

        [DefaultValue(null)]
        public virtual bool? IsTopping { get; set; }

        [DefaultValue(false)]
        public virtual bool IsRemind { get; set; }

        /// <summary>
        /// 是否保存通讯录(群)
        /// </summary>
        public virtual bool? IsCantacts { get; set; }

        /// <summary>
        /// 消息免打扰，默认为 false
        /// </summary>
        public virtual bool? IsImmersed { get; set; }

        [DefaultValue(false)]
        public virtual bool IsBadge { get; set; }

        public virtual bool IsOrderByBadge { get; set; }

        public virtual bool? IsKilled { get; set; }

        public virtual long? MinAutoId { get; set; }

        public virtual long? MaxAutoId { get; set; }

        //public virtual JoinWays? JoinWay { get; set; }

        //public virtual Guid? InviterId { get; set; }
    }
}