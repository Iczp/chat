﻿using IczpNet.Chat.BaseEntitys;
using IczpNet.Chat.ChatObjects;
using IczpNet.Chat.Enums;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IczpNet.Chat.Messages
{
    public partial class Message : BaseEntity<Guid>
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public virtual long AutoId { get; set; }

        [StringLength(100)]
        public virtual string SessionId { get; protected set; }

        /// <summary>
        /// 发送者
        /// </summary>
        public virtual Guid? SenderId { get; set; }

        /// <summary>
        /// 接收者
        /// </summary>
        public virtual Guid? ReceiverId { get; set; }

        /// <summary>
        /// 消息通道
        /// </summary>
        public virtual MessageChannel MessageChannel { get; set; }

        /// <summary>
        /// 消息类型
        /// </summary>
        public virtual MessageType MessageType { get; set; }

        [StringLength(5000)]
        public virtual string Content { get; protected set; }

        /// <summary>
        /// 扩展（键名）根据业务自义，如:"courseId"、"course-userId"、"erp-userId"
        /// </summary>
        [StringLength(100)]
        public virtual string KeyName { get; set; }

        /// <summary>
        /// 扩展（键值）根据业务自义,如："123456789"、"02b7d668-02ca-428f-b88c-b8adac2c5044"、"admin"
        /// </summary>
        [StringLength(5000)]
        public virtual string KeyValue { get; set; }

        /// <summary>
        /// 撤回消息时间
        /// </summary>
        public virtual DateTime? RollbackTime { get; set; }

        [ForeignKey(nameof(SenderId))]
        public virtual ChatObject Sender { get; set; }

        [ForeignKey(nameof(ReceiverId))]
        public virtual ChatObject Receiver { get; set; }

    }
}
