﻿using IczpNet.AbpCommons;
using IczpNet.Chat.ChatObjects;
using IczpNet.Chat.DataFilters;
using IczpNet.Chat.Enums;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IczpNet.Chat.Robots
{
    public class ShopWaiter : ChatObject, IChatOwner<Guid?>
    {
        public const ChatObjectTypeEnums ChatObjectTypeValue = ChatObjectTypeEnums.ShopWaiter;

        [StringLength(20)]
        public virtual string NickName { get; set; }

        public virtual Guid ShopKeeperId { get; set; }

        public virtual Guid? OwnerId { get; set; }

        [ForeignKey(nameof(ShopKeeperId))]
        public virtual ShopKeeper ShopKeeper { get; set; }

        [ForeignKey(nameof(OwnerId))]
        public virtual ChatObject Owner { get; set; }

        protected ShopWaiter()
        {
            ObjectType = ChatObjectTypeValue;
        }

        public ShopWaiter(Guid id, string name, Guid? parnetId, ShopKeeper shopKeeper, ChatObject chatObject) : base(id, name, ChatObjectTypeValue, parnetId) 
        {
            Assert.If(chatObject.ObjectType == ChatObjectTypeEnums.ShopWaiter, "");
            ShopKeeper = shopKeeper;
            Owner = chatObject;
        }
    }
}
