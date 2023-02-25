﻿using System;
using System.Collections.Generic;
using System.Linq;
using Volo.Abp.Users;

namespace IczpNet.Chat.ChatObjects
{
    public static class CurrentUserExtensions
    {
        public static List<Guid> GetChatObjectIdList(this ICurrentUser currentUser)
        {
            return currentUser.FindClaims(ChatObjectClaims.Id)
                .Where(x => !string.IsNullOrWhiteSpace(x.Value))
                .Select(x => Guid.Parse(x.Value))
                .ToList();
        }

        public static int GetChatObjectCount(this ICurrentUser currentUser)
        {
            return int.TryParse(currentUser.FindClaimValue(ChatObjectClaims.Count), out var count) ? count : 0;
        }
    }
}