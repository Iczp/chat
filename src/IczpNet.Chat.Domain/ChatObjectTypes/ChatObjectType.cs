﻿using IczpNet.AbpCommons.DataFilters;
using IczpNet.Chat.BaseEntitys;
using IczpNet.Chat.ChatObjects;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace IczpNet.Chat.ChatObjectTypes
{
    //[Index]
    public class ChatObjectType : BaseEntity<string>, IName
    {
        [StringLength(50)]
        [Required]
        public virtual string Name { get; set; }

        public virtual int MaxDepth { get; set; }

        public virtual bool IsHasChild { get; set; }

        public virtual IList<ChatObject> ChatObjectList { get; set; }

    }
}
