﻿using System.ComponentModel;

namespace IczpNet.Chat.Enums
{
    /// <summary>
    /// 头像的显示方式
    /// </summary>
    public enum PortraitDisplayModeEnum
    {
        /// <summary>
        /// 自定义
        /// </summary>
        [Description("自定义")]
        Custom = 0,
        /// <summary>
        /// 9宫格
        /// </summary>
        [Description("九宫格")]
        Grid = 1,
    }
}
