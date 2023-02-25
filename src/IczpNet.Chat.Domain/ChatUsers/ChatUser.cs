﻿using IczpNet.Chat.ChatObjects;
using IczpNet.Chat.Enums;
using System;

namespace IczpNet.Chat.ChatUsers
{
    public class ChatUser : ChatObject
    {
        public const ChatObjectTypeEnums ChatObjectTypeValue = ChatObjectTypeEnums.Personal;

        public override Guid? AppUserId { get; protected set; }
        protected ChatUser()
        {
            ObjectType = ChatObjectTypeValue;
        }

        protected ChatUser(Guid id, string name, Guid? parnetId) : base(id, name, ChatObjectTypeValue, parnetId) { }
    }
}
