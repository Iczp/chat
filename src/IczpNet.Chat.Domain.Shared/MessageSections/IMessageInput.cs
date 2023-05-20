﻿using IczpNet.Chat.Enums;
using System;

namespace IczpNet.Chat.MessageSections
{

    public interface IMessageInput<T> : IMessageInput where T : class, IContentInfo
    {
        T Content { get; }
    }

    public interface IMessageInput
    {
        //IContentInfo Content { get; }
        /// <summary>
        /// 发送者
        /// </summary>
        long SenderId { get; set; }

        /// <summary>
        /// 接收者
        /// </summary>
        long ReceiverId { get; set; }

        ///// <summary>
        ///// 消息类型
        ///// </summary>
        //MessageTypes MessageType { get; set; }

        /// <summary>
        /// 扩展（键名）根据业务自义，如:"courseId"、"course-userId"、"erp-userId"
        /// </summary>
        string KeyName { get; set; }

        /// <summary>
        /// 扩展（键值）根据业务自义,如："123456789"、"02b7d668-02ca-428f-b88c-b8adac2c5044"、"admin"
        /// </summary>
        string KeyValue { get; set; }

        long? QuoteMessageId { get; set; }
    }

   
}