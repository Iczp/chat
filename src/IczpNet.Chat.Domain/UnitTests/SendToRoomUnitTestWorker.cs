﻿using AutoMapper.Internal;
using IczpNet.Chat.Follows;
using IczpNet.Chat.MessageSections;
using IczpNet.Chat.MessageSections.Messages;
using IczpNet.Chat.MessageSections.Templates;
using IczpNet.Chat.RoomSections.Rooms;
using IczpNet.Chat.SessionSections.SessionUnits;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Volo.Abp.BackgroundWorkers;
using Volo.Abp.Threading;
using Volo.Abp.Uow;

namespace IczpNet.Chat.UnitTests
{
    public class SendToRoomUnitTestWorker : AsyncPeriodicBackgroundWorkerBase
    {
        private static List<Guid> SessionUnitIdList;

        private static long? RoomId = 6019;

        private static int Index = 0;

        protected IMessageSender MessageSender { get; }

        protected IRoomManager RoomManager { get; }

        protected ISessionUnitManager SessionUnitManager { get; }

        protected ISessionUnitRepository SessionUnitRepository { get; }
        protected IFollowManager FollowManager { get; }


        public SendToRoomUnitTestWorker(AbpAsyncTimer timer,
            IServiceScopeFactory serviceScopeFactory,
            ISessionUnitRepository sessionUnitRepository,
            IMessageSender messageSender,
            IRoomManager roomManager,
            ISessionUnitManager sessionUnitManager,
            IFollowManager followManager) : base(timer, serviceScopeFactory)
        {
            Timer.Period = 500;
            SessionUnitRepository = sessionUnitRepository;
            MessageSender = messageSender;
            RoomManager = roomManager;
            SessionUnitManager = sessionUnitManager;
            FollowManager = followManager;
        }

        [UnitOfWork]
        protected override async Task DoWorkAsync(PeriodicBackgroundWorkerContext workerContext)
        {
            await Task.CompletedTask;

            var stopWatch = Stopwatch.StartNew();

            Logger.LogInformation($"SendToRoomWorker");

            RoomId ??= (await RoomManager.CreateByAllUsersAsync($"Auto-create:{DateTime.Now}")).Id;

            var items = await GetSessionIdListAsync(RoomId.Value);

            var sessionunitId = items[new Random().Next(0, items.Count - 1)];

            var sessionunit = await SessionUnitRepository.GetAsync(sessionunitId);

            Logger.LogInformation($"sessionunit: id:{sessionunit?.Id},name:{sessionunit?.Owner?.Name}");

            // Following
            if (sessionunit.OwnerFollowList.Count < 3)
            {
                var tagId = items[new Random().Next(0, items.Count - 1)];

                await FollowManager.CreateAsync(sessionunit, new List<Guid>() { tagId });

                Logger.LogInformation($"Follow: sessionunit: id:{sessionunit?.Id},tagId:{tagId}");
            }

            Index++;

            var remindList = new List<Guid>();

            for (int i = 0; i < new Random().Next(1, 10); i++)
            {
                remindList.TryAdd(items[new Random().Next(0, items.Count - 1)]);
            }

            var text = $"RoomId:{RoomId}, sessionunitId:{sessionunitId}, remindCount:{remindList.Count}";

            if (new Random().Next(3) % 3 == 1)
            {
                text = $"@陈忠培 {text}";
            }

            await MessageSender.SendTextAsync(sessionunit, new MessageSendInput<TextContentInfo>()
            {
                Content = new TextContentInfo()
                {
                    Text = text
                },
                RemindList = remindList
            });
            stopWatch.Stop();

            Logger.LogInformation($"SendText: {text},stopWatch:{stopWatch.ElapsedMilliseconds}");

        }

        protected async Task<List<Guid>> GetSessionIdListAsync(long roomId)
        {
            return SessionUnitIdList ??= (await SessionUnitRepository.GetQueryableAsync())
                .Where(x => x.DestinationId == roomId)
                .Select(x => x.Id)
                .ToList();
        }
    }
}
