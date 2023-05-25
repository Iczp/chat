﻿using IczpNet.AbpCommons.Extensions;
using IczpNet.Chat.EntityFrameworkCore;
using IczpNet.Chat.SessionSections.SessionUnits;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Volo.Abp.EntityFrameworkCore;

namespace IczpNet.Chat.Repositories
{
    public class SessionUnitRepository : ChatRepositoryBase<SessionUnit, Guid>, ISessionUnitRepository
    {
        public SessionUnitRepository(IDbContextProvider<ChatDbContext> dbContextProvider) : base(dbContextProvider)
        {
        }

        protected virtual Task<string> GetTableNameForEntityAsync(ChatDbContext context, Type typeofEntity)
        {
            var entityType = context.Model.FindEntityType(typeof(SessionUnit));
            var schema = entityType.GetSchema();
            var tableName = entityType.GetTableName();
            if (string.IsNullOrWhiteSpace(tableName))
            {
                tableName = "dbo";
            }
            var table = $"{schema}.{tableName}";
            return Task.FromResult(table);
        }

        private static Expression<Func<SessionUnit, bool>> GetSessionUnitPredicate(DateTime messageCreationTime)
        {
            return SessionUnit.GetActivePredicate(messageCreationTime);
        }

        private async Task<IQueryable<SessionUnit>> GetQueryableAsync(DateTime messageCreationTime)
        {
            var context = await GetDbContextAsync();

            var predicate = GetSessionUnitPredicate(messageCreationTime);

            return context.SessionUnit.Where(predicate);
        }

        protected virtual Task<int> BatchUpdateLastMessageIdAsync(Guid sessionId, long lastMessageId, List<Guid> sessionUnitIdList = null)
        {
            return BatchUpdateLastMessageIdByEf7Async(sessionId, lastMessageId);
        }

        protected virtual async Task<int> BatchUpdateLastMessageIdBySqlAsync(Guid sessionId, long lastMessageId, List<Guid> sessionUnitIdList = null)
        {
            var context = await GetDbContextAsync();

            var table = await GetTableNameForEntityAsync(context, typeof(SessionUnit));

            var sql = @$"Update {table} set {nameof(SessionUnit.LastMessageId)}=@LastMessageId where {nameof(SessionUnit.SessionId)}=@SessionId and [{nameof(SessionUnit.IsDeleted)}]=@IsDeleted and {nameof(SessionUnit.LastMessageId)}<@LastMessageId";

            var parameters = new List<SqlParameter>()
            {
                new SqlParameter("@LastMessageId", lastMessageId),
                new SqlParameter("@SessionId", sessionId),
                new SqlParameter("@IsDeleted", false),
            };

            if (sessionUnitIdList.IsAny())
            {
                sql += " and id in(@SessionUnitIdList)";
                parameters.Add(new SqlParameter("@SessionUnitIdList", sessionUnitIdList));
            }

            return await context.Database.ExecuteSqlRawAsync(sql, parameters);
        }

        protected virtual async Task<int> BatchUpdateLastMessageIdByEf7Async(Guid sessionId, long lastMessageId, List<Guid> sessionUnitIdList = null)
        {
            var context = await GetDbContextAsync();

            ////EF7.0  https://learn.microsoft.com/en-us/ef/core/what-is-new/ef-core-7.0/whatsnew
            return await context.SessionUnit
                .Where(x => x.SessionId == sessionId && !x.IsDeleted)
                .WhereIf(sessionUnitIdList.IsAny(), x => sessionUnitIdList.Contains(x.Id))
                .ExecuteUpdateAsync(s => s
                    .SetProperty(b => b.LastMessageId, b => lastMessageId)
                //.SetProperty(b => b.LastModificationTime, b => DateTime.Now)
                );
        }

        public virtual async Task<int> BatchUpdateLastMessageIdAndPublicBadgeAndRemindAllCountAsync(Guid sessionId, long lastMessageId, DateTime messageCreationTime, Guid ignoreSessionUnitId, bool isRemindAll)
        {
            var query = (await GetQueryableAsync(messageCreationTime))
                .Where(x => x.SessionId == sessionId)
                .Where(x => x.Id != ignoreSessionUnitId)
                .Where(x => x.LastMessageId != lastMessageId);

            if (isRemindAll)
            {
                return await query.ExecuteUpdateAsync(s => s
                    .SetProperty(b => b.PublicBadge, b => b.PublicBadge + 1)
                    .SetProperty(b => b.LastMessageId, b => lastMessageId)
                    .SetProperty(b => b.RemindAllCount, b => b.RemindAllCount + 1)
                );
            }

            return await query.ExecuteUpdateAsync(s => s
                    .SetProperty(b => b.PublicBadge, b => b.PublicBadge + 1)
                    .SetProperty(b => b.LastMessageId, b => lastMessageId)
                //.SetPropertyIf(isRemindAll, b => b.RemindAllCount, b => b.RemindAllCount + 1)
                );
        }

        protected virtual async Task<int> IncrementPublicBadgeAsync(Guid sessionId, DateTime messageCreationTime, Guid ignoreSessionUnitId)
        {
            var query = await GetQueryableAsync(messageCreationTime);

            return await query
                .Where(x => x.SessionId == sessionId)
                .Where(x => x.Id != ignoreSessionUnitId)
                .ExecuteUpdateAsync(s => s
                    .SetProperty(b => b.PublicBadge, b => b.PublicBadge + 1)
                );
        }

        protected virtual async Task<int> IncrementRemindAllCountAsync(Guid sessionId, DateTime messageCreationTime, Guid ignoreSessionUnitId)
        {
            var query = await GetQueryableAsync(messageCreationTime);

            return await query
                .Where(x => x.SessionId == sessionId)
                .Where(x => x.Id != ignoreSessionUnitId)
                .ExecuteUpdateAsync(s => s
                    .SetProperty(b => b.RemindAllCount, b => b.RemindAllCount + 1)
                );
        }



        public virtual async Task<int> IncrementRemindMeCountAsync(DateTime messageCreationTime, List<Guid> sessionUnitIdList)
        {
            var query = await GetQueryableAsync(messageCreationTime);

            return await query
                .Where(x => sessionUnitIdList.Contains(x.Id))
                .ExecuteUpdateAsync(s => s
                    .SetProperty(b => b.RemindMeCount, b => b.RemindMeCount + 1)
                );
        }


        public virtual async Task<int> IncrementFollowingCountAsync(Guid sessionId, DateTime messageCreationTime, List<Guid> destinationSessionUnitIdList)
        {
            var query = await GetQueryableAsync(messageCreationTime);

            return await query
                .Where(x => x.SessionId == sessionId)
                .Where(x => destinationSessionUnitIdList.Contains(x.Id))
                .ExecuteUpdateAsync(s => s
                    .SetProperty(b => b.FollowingCount, b => b.FollowingCount + 1)
                );
        }

        public virtual async Task<int> BatchUpdateNameAsync(long chatObjectId, string name, string nameSpelling, string nameSpellingAbbreviation)
        {
            var query = await GetQueryableAsync(DateTime.Now);

            var a = await query
                 .Where(x => x.OwnerId == chatObjectId)
                 .ExecuteUpdateAsync(s => s
                     .SetProperty(b => b.OwnerName, b => name)
                     .SetProperty(b => b.OwnerNameSpellingAbbreviation, b => nameSpellingAbbreviation)
                 );
            var b = await query
                 .Where(x => x.DestinationId == chatObjectId)
                 .ExecuteUpdateAsync(s => s
                     .SetProperty(b => b.DestinationName, b => name)
                     .SetProperty(b => b.DestinationNameSpellingAbbreviation, b => nameSpellingAbbreviation)
                 );
            return a + b;
        }

        public async Task<int> BatchUpdateAppUserIdAsync(long chatObjectId, Guid appUserId)
        {
            var query = await GetQueryableAsync(DateTime.Now);

            return await query
                .Where(x => x.OwnerId == chatObjectId)
                .ExecuteUpdateAsync(s => s
                     .SetProperty(b => b.AppUserId, b => appUserId)
                 );
        }
    }
}
