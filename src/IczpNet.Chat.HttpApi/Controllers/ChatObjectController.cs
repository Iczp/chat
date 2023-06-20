﻿using IczpNet.Chat.ChatObjects;
using IczpNet.Chat.ChatObjects.Dtos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Volo.Abp;

namespace IczpNet.Chat.Controllers
{
    [Area(ChatRemoteServiceConsts.ModuleName)]
    [RemoteService(Name = ChatRemoteServiceConsts.RemoteServiceName)]
    //[Route($"Api/{ChatRemoteServiceConsts.ModuleName}[Controller]/[Action]")]
    public class ChatObjectController : ChatController
    {
        protected IChatObjectAppService ChatObjectAppService { get; }

        public ChatObjectController(IChatObjectAppService chatObjectAppService)
        {
            ChatObjectAppService = chatObjectAppService;
        }

        /// <summary>
        /// 更新头像
        /// </summary>
        /// <param name="id"></param>
        /// <param name="file"></param>
        /// <returns></returns>
        [HttpPost]
        public Task<ChatObjectDto> UpdatePortraitAsync(long id, IFormFile file)
        {
            return ChatObjectAppService.UpdatePortraitAsync(id, "");
        }
    }
}
