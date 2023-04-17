﻿using IczpNet.AbpCommons;
using IczpNet.Chat.BaseAppServices;
using IczpNet.Chat.SessionSections.SessionRequests;
using IczpNet.Chat.SessionSections.SessionRequests.Dtos;
using IczpNet.Chat.SessionSections.Sessions;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;

namespace IczpNet.Chat.SessionServices
{
    public class SessionRequestAppService
        : CrudChatAppService<
            SessionRequest,
            SessionRequestDetailDto,
            SessionRequestDto,
            Guid,
            SessionRequestGetListInput,
            SessionRequestCreateInput,
            SessionRequestUpdateInput>,
        ISessionRequestAppService
    {

        protected ISessionManager SessionManager { get; }
        protected ISessionUnitManager SessionUnitManager { get; }
        protected ISessionRequestManager SessionRequestManager { get; }
        public SessionRequestAppService(
            IRepository<SessionRequest, Guid> repository,
            ISessionManager sessionManager,
            ISessionUnitManager sessionUnitManager,
            ISessionRequestManager sessionRequestManager) : base(repository)
        {
            SessionManager = sessionManager;
            SessionUnitManager = sessionUnitManager;
            SessionRequestManager = sessionRequestManager;
        }

        protected override async Task<IQueryable<SessionRequest>> CreateFilteredQueryAsync(SessionRequestGetListInput input)
        {
            return (await base.CreateFilteredQueryAsync(input))
                .WhereIf(input.OwnerId.HasValue, x => x.OwnerId == input.OwnerId)
                .WhereIf(input.DestinationId.HasValue, x => x.DestinationId == input.DestinationId)
                .WhereIf(input.IsHandled.HasValue, x => x.IsHandled == input.IsHandled)
                .WhereIf(input.IsAgreed.HasValue, x => x.IsAgreed == input.IsAgreed)
                .WhereIf(input.StartCreationTime.HasValue, x => x.CreationTime >= input.StartCreationTime)
                .WhereIf(input.StartCreationTime.HasValue, x => x.CreationTime < input.EndCreationTime)
                .WhereIf(input.StartHandleTime.HasValue, x => x.HandleTime >= input.StartHandleTime)
                .WhereIf(input.StartHandleTime.HasValue, x => x.HandleTime < input.EndHandleTime)
                .WhereIf(!input.Keyword.IsNullOrWhiteSpace(), x => x.RequestMessage.Contains(input.Keyword) || x.HandleMessage.Contains(input.Keyword))
                ;
        }

        protected override SessionRequest MapToEntity(SessionRequestCreateInput createInput)
        {
            return new SessionRequest(createInput.OwnerId, createInput.DestinationId, createInput.RequestMessage);
        }

        protected override async Task CheckCreateAsync(SessionRequestCreateInput input)
        {
            Assert.NotNull(await SessionUnitManager.FindAsync(input.OwnerId, input.DestinationId), "Already a friend");
        }

        [HttpPost]
        public override Task<SessionRequestDetailDto> CreateAsync(SessionRequestCreateInput input)
        {
            return base.CreateAsync(input);
        }

        [HttpPost]
        [RemoteService(false)]
        public override Task<SessionRequestDetailDto> UpdateAsync(Guid id, SessionRequestUpdateInput input)
        {
            return base.UpdateAsync(id, input);
        }

        [RemoteService(false)]
        public override Task DeleteAsync(Guid id)
        {
            return base.DeleteAsync(id);
        }

        [RemoteService(false)]
        public override Task DeleteManyAsync(List<Guid> idList)
        {
            return base.DeleteManyAsync(idList);
        }

        [HttpPost]
        public async Task<DateTime?> HandleRequestAsync(SessionRequestHandleInput input)
        {
            var sessionRequest = await SessionRequestManager.HandleRequestAsync(input.SessionRequestId, input.IsAgreed, input.HandleMessage, null);

            return sessionRequest.HandleTime;
        }
    }
}
