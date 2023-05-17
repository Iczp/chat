﻿using IczpNet.Chat.Attributes;
using IczpNet.Chat.Enums;
using System.ComponentModel.DataAnnotations;
using System.Drawing;

namespace IczpNet.Chat.MessageSections.Templates
{
    [MessageTemplate(MessageTypes.File)]
    [ContentOuput(typeof(FileContentInfo))]
    public class FileContent : MessageContentEntityBase
    {
        public override long GetSize() => ContentLength ?? 0;
        /// <summary>
        /// 文件地址
        /// </summary>
        //[Required(ErrorMessage = "文件地址(必填)")]
        [StringLength(500)]
        public virtual string Url { get; set; }

        /// <summary>
        /// FileName
        /// </summary>
        [StringLength(256)]
        //[Index]
        public virtual string FileName { get; set; }

        /// <summary>
        /// 文件地址
        /// </summary>
        //[Required(ErrorMessage = "文件控制器地址")]
        [StringLength(500)]
        public virtual string ActionUrl { get; set; }

        /// <summary>
        /// ContentType
        /// </summary>
        [StringLength(100)]
        //[Index]
        public virtual string ContentType { get; set; }

        /// <summary>
        /// 文件后缀名
        /// </summary>
        [StringLength(10)]
        //[Index]
        public virtual string Suffix { get; set; }

        /// <summary>
        /// 大小 ContentLength(Size)
        /// </summary>
        public virtual long? ContentLength { get; set; }
    }
}
