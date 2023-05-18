﻿using IczpNet.AbpCommons;
using IczpNet.Chat.Bases;
using IczpNet.Chat.MessageSections.Messages;
using IczpNet.Chat.SessionSections.SessionUnits;
using IczpNet.Chat.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Settings;

namespace IczpNet.Chat.Favorites
{
    public class FavoriteManager : RecorderManager<Favorite>, IFavoriteManager
    {
        protected ISettingProvider SettingProvider { get; }
        public FavoriteManager(IRepository<Favorite> repository, ISettingProvider settingProvider) : base(repository)
        {
            SettingProvider = settingProvider;
        }
        protected override Favorite CreateEntity(SessionUnit sessionUnit, Message message, string deviceId)
        {
            return new Favorite(sessionUnit, message, deviceId);
        }
        protected override Favorite CreateEntity(Guid sessionUnitId, long messageId)
        {
            return new Favorite(sessionUnitId, messageId);
        }

        public override async Task<Favorite> CreateIfNotContainsAsync(SessionUnit sessionUnit, long messageId, string deviceId)
        {
            //check favorite size
            var maxFavoriteSize = await SettingProvider.GetAsync<long>(ChatSettings.MaxFavoriteSize);

            var size = await GetSizeByOwnerIdAsync(sessionUnit.OwnerId);

            Assert.If(size > maxFavoriteSize, $"MaxFavoriteSize:${maxFavoriteSize}");

            //check favorite count
            var count = await GetCountByOwnerIdAsync(sessionUnit.OwnerId);

            var maxFavoriteCount = await SettingProvider.GetAsync<long>(ChatSettings.MaxFavoriteCount);

            Assert.If(count > maxFavoriteCount, $"MaxFavoriteCount:${maxFavoriteCount}");

            return await base.CreateIfNotContainsAsync(sessionUnit, messageId, deviceId);
        }

        public virtual Task DeleteAsync(Guid sessionUnitId, long messageId)
        {
            return Repository.DeleteAsync(x => x.SessionUnitId == sessionUnitId && x.MessageId == messageId);
        }

        public virtual async Task<long> GetSizeByOwnerIdAsync(long ownerId)
        {
            return (await Repository.GetQueryableAsync())
                .Where(x => x.OwnerId == ownerId)
                .Sum(x => x.Size);
        }

        public virtual async Task<int> GetCountByOwnerIdAsync(long ownerId)
        {
            return (await Repository.GetQueryableAsync())
                .Where(x => x.OwnerId == ownerId)
                .Count();
        }

        protected override async Task ChangeMessageIfNotContainsAsync(SessionUnit sessionUnit, Message message)
        {
            message.FavoritedCount++;
            await Task.CompletedTask;
        }

        protected override async Task ChangeMessagesIfNotContainsAsync(SessionUnit sessionUnit, List<Message> changeMessages)
        {
            foreach (Message message in changeMessages)
            {
                message.FavoritedCount++;
            }
            await Task.CompletedTask;
        }
    }
}
