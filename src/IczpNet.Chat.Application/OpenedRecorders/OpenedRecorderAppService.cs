﻿using IczpNet.Chat.BaseAppServices;
using IczpNet.Chat.ChatObjects;
using IczpNet.Chat.Mottos.Dtos;
using IczpNet.Chat.OpenedRecorders.Dtos;
using IczpNet.Chat.SessionSections.SessionUnits;
using IczpNet.Chat.SessionSections.SessionUnits.Dtos;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Repositories;

namespace IczpNet.Chat.OpenedRecorders
{
    public class OpenedRecorderAppService : ChatAppService, IOpenedRecorderAppService
    {
        protected virtual string SetReadedPolicyName { get; set; }

        protected IRepository<OpenedRecorder> Repository { get; }

        protected IOpenedRecorderManager OpenedRecorderManager { get; }

        public OpenedRecorderAppService(
            IOpenedRecorderManager openedRecorderManager, 
            IRepository<OpenedRecorder> repository)
        {
            OpenedRecorderManager = openedRecorderManager;
            Repository = repository;
        }

        [HttpGet]
        public Task<Dictionary<long, int>> GetCountsAsync(List<long> messageIdList)
        {
            return OpenedRecorderManager.GetCountsAsync(messageIdList);
        }

        [HttpGet]
        public async Task<PagedResultDto<SessionUnitDestinationDto>> GetListByMessageIdAsync(long messageId, GetListByMessageIdInput input)
        {
            var query = input.IsReaded
                ? await OpenedRecorderManager.QueryRecordedAsync(messageId)
                : await OpenedRecorderManager.QueryUnrecordedAsync(messageId);

            query = query.WhereIf(!input.Keyword.IsNullOrWhiteSpace(), new KeywordOwnerSessionUnitSpecification(input.Keyword, await ChatObjectManager.QueryByKeywordAsync(input.Keyword)));

            return await GetPagedListAsync<SessionUnit, SessionUnitDestinationDto>(query, input);
        }

        [HttpPost]
        public virtual async Task<OpenedRecorderDto> SetOpenedAsync(OpenedRecorderInput input)
        {
            await CheckPolicyAsync(SetReadedPolicyName);

            var sessionUnit = await SessionUnitManager.GetAsync(input.SessionUnitId);

            var entity = await OpenedRecorderManager.CreateIfNotContainsAsync(sessionUnit, input.MessageId, input.DeviceId);

            return await MapToDtoAsync(entity);
        }

        protected virtual Task<OpenedRecorderDto> MapToDtoAsync(OpenedRecorder entity)
        {
            return Task.FromResult(MapToDto(entity));
        }

        protected virtual OpenedRecorderDto MapToDto(OpenedRecorder entity)
        {
            return ObjectMapper.Map<OpenedRecorder, OpenedRecorderDto>(entity);
        }
    }
}
