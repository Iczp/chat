﻿using IczpNet.Chat.Enums;
using IczpNet.Chat.Management.BaseDtos;
using System;

namespace IczpNet.Chat.Management.Articles.Dtos;

public class ArticleGetListInput : BaseGetListInput
{
    /// <summary>
    /// 群主
    /// </summary>
    public virtual Guid? OwnerId { get; set; }

    public virtual ArticleTypes? Type { get; set; }

    public virtual int? MinCount { get; set; }

    public virtual int? MaxCount { get; set; }

    /// <summary>
    /// 是否全体禁言
    /// </summary>
    public virtual bool? IsForbiddenAll { get; set; }

    /// <summary>
    /// 成员所在的群(我加入的群)
    /// </summary>
    public virtual Guid? MemberOwnerId { get; set; }

    /// <summary>
    /// 成员被禁言的群
    /// </summary>
    public virtual Guid? ForbiddenMemberOwnerId { get; set; }
    


}
