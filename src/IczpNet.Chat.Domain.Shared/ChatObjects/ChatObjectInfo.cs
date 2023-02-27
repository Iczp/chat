﻿using IczpNet.AbpTrees;
using IczpNet.Chat.Enums;
using System;

namespace IczpNet.Chat.ChatObjects
{
    public class ChatObjectInfo : TreeInfo<Guid>
    {
        public virtual string Code { get; set; }

        public virtual string Portrait { get; set; }

        public virtual Guid? AppUserId { get; set; }

        public virtual ChatObjectTypeEnums? ObjectType { get; set; }
    }
}
