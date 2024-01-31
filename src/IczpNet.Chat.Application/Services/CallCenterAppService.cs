﻿using IczpNet.AbpCommons.Extensions;
using IczpNet.Chat.BaseAppServices;
using IczpNet.Chat.CallCenters;
using IczpNet.Chat.Enums;
using IczpNet.Chat.SessionUnits;
using IczpNet.Chat.SessionUnits.Dtos;
using Microsoft.AspNetCore.Mvc;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Volo.Abp.ObjectMapping;

namespace IczpNet.Chat.Services
{
    /// <summary>
    /// 呼叫中心
    /// </summary>
    public class CallCenterAppService : ChatAppService, ICallCenterAppService
    {
        protected ICallCenterManager CallCenterManager { get; }

        public CallCenterAppService(ICallCenterManager callCenterManager)
        {
            CallCenterManager = callCenterManager;
        }

        /// <summary>
        /// 转接
        /// </summary>
        /// <param name="sessionUnitId">当前会话单元Id</param>
        /// <param name="destinationId">目标会话单元Id</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<SessionUnitOwnerDetailDto> TransferToAsync([Required] Guid sessionUnitId, [Required] long destinationId)
        {
            var entity = await CallCenterManager.TransferToAsync(sessionUnitId, destinationId, isNotice: true);

            return ObjectMapper.Map<SessionUnit, SessionUnitOwnerDetailDto>(entity);
        }
    }
}
