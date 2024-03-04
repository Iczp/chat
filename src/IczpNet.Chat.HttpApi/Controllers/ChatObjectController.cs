﻿using IczpNet.AbpCommons;
using IczpNet.Chat.Blobs.Dtos;
using IczpNet.Chat.ChatObjects;
using IczpNet.Chat.ChatObjects.Dtos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp;

namespace IczpNet.Chat.Controllers
{
    [Area(ChatRemoteServiceConsts.ModuleName)]
    [RemoteService(Name = ChatRemoteServiceConsts.RemoteServiceName)]
    [Route($"/api/{ChatRemoteServiceConsts.ModuleName}/chat-object")]
    public class ChatObjectController : ChatController
    {
        protected IChatObjectAppService ChatObjectAppService { get; }

        public ChatObjectController(IChatObjectAppService chatObjectAppService)
        {
            ChatObjectAppService = chatObjectAppService;
        }

        /// <summary>
        /// 上传头像
        /// </summary>
        /// <param name="id">主建Id</param>
        /// <param name="file"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("{id}/upload-portrait")]
        public async Task<ChatObjectDto> UpdatePortraitAsync(long id, IFormFile file)
        {
            await CheckImageAsync(file);

            var bigImgBlobId = GuidGenerator.Create();

            var thumbnailBlobId = GuidGenerator.Create();

            var chatObjectDto = await ChatObjectAppService.UpdatePortraitAsync(id, $"/file?id={thumbnailBlobId}", $"/file?id={bigImgBlobId}");

            await SavePortraitAsync(file, id, thumbnailBlobId, bigImgBlobId);

            return chatObjectDto;
        }

    }
}
