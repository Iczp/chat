﻿using IczpNet.Chat.BaseAppServices;
using IczpNet.Chat.ChatObjects;
using IczpNet.Chat.Follows.Dtos;
using IczpNet.Chat.SessionSections.SessionUnits;
using IczpNet.Chat.SessionSections.SessionUnits.Dtos;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Repositories;

namespace IczpNet.Chat.Follows
{
    public class FollowAppService : ChatAppService, IFollowAppService
    {
        protected IFollowManager FollowManager { get; set; }
        protected ISessionUnitRepository SessionUnitRepository { get; set; }
        protected IRepository<Follow> Repository { get; set; }

        public FollowAppService(
            IFollowManager followManager,
            ISessionUnitRepository sessionUnitRepository,
            IRepository<Follow> repository)
        {
            FollowManager = followManager;
            SessionUnitRepository = sessionUnitRepository;
            Repository = repository;
        }

        /// <inheritdoc/>
        [HttpGet]
        public async Task<PagedResultDto<SessionUnitDestinationDto>> GetListFollowingAsync(FollowingGetListInput input)
        {
            var ownerownerSessionUnit = await SessionUnitManager.GetAsync(input.SessionUnitId);

            var ownerSessionUnitIdList = (await Repository.GetQueryableAsync())
                .Where(x => x.OwnerId == ownerownerSessionUnit.Id)
                .Select(x => x.DestinationId)
                ;

            var query = (await SessionUnitRepository.GetQueryableAsync())
                .Where(x => ownerSessionUnitIdList.Contains(x.Id))
                .WhereIf(!input.Keyword.IsNullOrWhiteSpace(), new KeywordOwnerSessionUnitSpecification(input.Keyword, await ChatObjectManager.QueryByKeywordAsync(input.Keyword)))
                ;

            return await GetPagedListAsync<SessionUnit, SessionUnitDestinationDto>(query);
        }

        /// <inheritdoc/>
        [HttpGet]
        public async Task<PagedResultDto<SessionUnitDestinationDto>> GetListFollowerAsync(FollowerGetListInput input)
        {
            var query = (await Repository.GetQueryableAsync())
                .Where(x => x.DestinationId == input.SessionUnitId)
                .Select(x => x.Owner)
                //.WhereIf(!input.Keyword.IsNullOrWhiteSpace(), new KeywordDestinationSessionUnitSpecification(input.Keyword).ToExpression())
                .WhereIf(!input.Keyword.IsNullOrWhiteSpace(), new KeywordDestinationSessionUnitSpecification(input.Keyword, await ChatObjectManager.QueryByKeywordAsync(input.Keyword)))
                ;

            return await GetPagedListAsync<SessionUnit, SessionUnitDestinationDto>(query);
        }

        /// <inheritdoc/>
        [HttpPost]
        public async Task<bool> CreateAsync([FromQuery] FollowCreateInput input)
        {
            var owner = await SessionUnitManager.GetAsync(input.OwnerId);

            //check owner

            return await FollowManager.CreateAsync(owner, input.IdList);
        }

        /// <inheritdoc/>
        [HttpPost]
        public async Task DeleteAsync([FromQuery] FollowDeleteInput input)
        {
            var owner = await SessionUnitManager.GetAsync(input.OwnerId);
            //check owner
            await FollowManager.DeleteAsync(input.OwnerId, input.IdList);
        }
        

    }
}
