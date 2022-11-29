﻿using IczpNet.Chat.BaseDtos;
using IczpNet.Chat.Enums;

namespace IczpNet.Chat.ChatObjects.Dtos
{
    public class ChatObjectDto : BaseDto
    {
        public virtual string Name { get; set; }

        public virtual string Code { get; set; }

        public virtual ChatObjectTypeEnum ChatObjectType { get; set; }
    }
}
