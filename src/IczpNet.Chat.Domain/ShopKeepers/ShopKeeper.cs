﻿using IczpNet.Chat.ChatObjects;
using System;
using IczpNet.Chat.Enums;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using IczpNet.Chat.DataFilters;

namespace IczpNet.Chat.Robots
{
    public class ShopKeeper : ChatObject, IOwner<Guid?>
    {
        public const ChatObjectTypeEnum ChatObjectTypeValue = ChatObjectTypeEnum.ShopKeeper;

        public virtual Guid? OwnerId { get; set; }

        [ForeignKey(nameof(OwnerId))]
        public virtual ChatObject Owner { get; set; }

        [InverseProperty(nameof(ShopWaiter.ShopKeeper))]
        public virtual IList<ShopWaiter> ShopWaiterList { get; set; }

        protected ShopKeeper()
        {
            ChatObjectType = ChatObjectTypeValue;
        }
        protected ShopKeeper(Guid id) : base(id, ChatObjectTypeValue)
        {

        }
    }
}
