﻿using IczpNet.Chat.Attributes;
using IczpNet.Chat.Enums;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IczpNet.Chat.MessageSections.Templates
{
    [MessageTemplate(MessageTypes.History)]
    public class HistoryContent : MessageContentEntityBase
    {
        /// <summary>
        /// 文本内容
        /// </summary>
        [Required(ErrorMessage = "标题内容[Title]必填！")]
        [StringLength(256)]
        //[Index]
        public virtual string Title { get; protected set; }

        /// <summary>
        /// 简要说明
        /// </summary>
        [StringLength(500)]
        //[Index]
        public virtual string Description { get; protected set; }

        [InverseProperty(nameof(HistoryMessage.HistoryContent))]
        public virtual List<HistoryMessage> HistoryMessageList { set; protected get; }
    }
}
