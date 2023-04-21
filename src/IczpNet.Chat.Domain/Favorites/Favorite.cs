﻿using IczpNet.Chat.BaseEntitys;
using IczpNet.Chat.ChatObjects;
using IczpNet.Chat.DataFilters;
using IczpNet.Chat.FavoriteMessages;
using System;
using System.Collections.Generic;

namespace IczpNet.Chat.Favorites
{
    public class Favorite : BaseEntity<Guid>, IChatOwner<long>
    {
        public virtual long OwnerId { get; set; }

        public virtual ChatObject Owner { get; set; }

        /// <summary>
        /// 收藏的消息
        /// </summary>
        public virtual IList<FavoriteMessage> FavoriteMessageList { get; set; }
    }
}
