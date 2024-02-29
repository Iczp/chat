﻿using IczpNet.AbpCommons;
using IczpNet.Chat.Blobs.Dtos;
using IczpNet.Chat.RoomSections.Rooms;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Volo.Abp;

namespace IczpNet.Chat.Controllers
{
    [Area(ChatRemoteServiceConsts.ModuleName)]
    [RemoteService(Name = ChatRemoteServiceConsts.RemoteServiceName)]
    [Route($"/api/{ChatRemoteServiceConsts.ModuleName}/room")]
    public class RoomController : ChatController
    {
        protected IRoomAppService RoomAppService { get; }

        public RoomController(IRoomAppService roomAppService)
        {
            RoomAppService = roomAppService;
        }

        /// <summary>
        /// 上传群头像
        /// </summary>
        /// <param name="sessionUnitId">主建Id</param>
        /// <param name="file"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("upload-portrait/{sessionUnitId}")]
        public async Task<BlobDto> UploadPortraitAsync(Guid sessionUnitId, IFormFile file)
        {
            Assert.If(file == null, "No file found!");

            var blobId = GuidGenerator.Create();

            var chatObjectDto = await RoomAppService.UpdatePortraitAsync(sessionUnitId, $"/file?id={blobId}");

            var blob = await UploadFileAsync(blobId, file, PortraitsContainer, $"{chatObjectDto.Id}", true);

            return blob;
        }
    }
}
