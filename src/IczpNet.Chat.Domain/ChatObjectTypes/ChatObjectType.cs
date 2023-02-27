﻿using IczpNet.AbpCommons.DataFilters;
using IczpNet.Chat.BaseEntitys;
using IczpNet.Chat.ChatObjectCategorys;
using IczpNet.Chat.ChatObjects;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace IczpNet.Chat.ChatObjectTypes
{
    public class ChatObjectType : BaseEntity<string>, IName, IIsStatic
    {
        [StringLength(50)]
        [Required]
        public virtual string Name { get; set; }

        [StringLength(500)]
        public virtual string Description { get; set; }

        public virtual int MaxDepth { get; set; }

        public virtual bool IsHasChild { get; set; }

        public virtual bool IsStatic { get; internal protected set; }

        public virtual IList<ChatObject> ChatObjectList { get; set; } = new List<ChatObject>();

        public virtual IList<ChatObjectCategory> ChatObjectCategoryList { get; set; } = new List<ChatObjectCategory>();

        protected ChatObjectType() { }

        public ChatObjectType(string id) : base(id) { }

        public int GetChatObjectCount()
        {
            return ChatObjectList.Count;
        }
    }
}
