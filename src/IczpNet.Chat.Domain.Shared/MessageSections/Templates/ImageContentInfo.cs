﻿namespace IczpNet.Chat.MessageSections.Templates
{
    /// <summary>
    /// 图片消息
    /// </summary>
    public class ImageContentInfo : BaseMessageContentInfo, IMessageContentInfo
    {
        /// <summary>
        /// 图片地址
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// MinIO控制器URL
        /// </summary>
        public string ActionUrl { get; set; }

        /// <summary>
        /// 缩略图地址
        /// </summary>
        public string ThumbnailUrl { get; set; }

        /// <summary>
        /// 缩略图MinIO控制器URL
        /// </summary>
        public string ThumbnailActionUrl { get; set; }

        /// <summary>
        /// 拍照时设备方向信息 http://www.html5plus.org/doc/zh_cn/io.html#plus.io.ImageInfo
        /// </summary>
        public string Orientation { get; set; }

        /// <summary>
        /// Width
        /// </summary>
        public int Width { get; set; }

        /// <summary>
        /// Height
        /// </summary>
        public int Height { get; set; }

        /// <summary>
        /// Size
        /// </summary>
        public int Size { get; set; }

        /// <summary>
        /// 二维码信息
        /// </summary>
        public string Qrcode { get; set; }

    }
}