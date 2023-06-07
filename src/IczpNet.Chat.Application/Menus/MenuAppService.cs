﻿using IczpNet.AbpCommons;
using IczpNet.AbpTrees;
using IczpNet.Chat.BaseAppServices;
using IczpNet.Chat.ChatObjects;
using IczpNet.Chat.Menus.Dtos;
using IczpNet.Chat.SessionSections.SessionPermissions;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace IczpNet.Chat.Menus
{
    public class MenuAppService
        : CrudTreeChatAppService<
            Menu,
            Guid,
            MenuDto,
            MenuDto,
            MenuGetListInput,
            MenuCreateInput,
            MenuUpdateInput,
            MenuInfo>,
        IMenuAppService
    {
        protected ISessionPermissionChecker SessionPermissionChecker { get; }
        protected IChatObjectRepository ChatObjectRepository { get; }
        protected new IMenuManager TreeManager { get; }

        public MenuAppService(
            IRepository<Menu, Guid> repository,
            IMenuManager menuManager,
            ISessionPermissionChecker sessionPermissionChecker,
            IChatObjectRepository chatObjectRepository) : base(repository, menuManager)
        {

            SessionPermissionChecker = sessionPermissionChecker;
            ChatObjectRepository = chatObjectRepository;
            TreeManager = menuManager;
        }

        protected override async Task<IQueryable<Menu>> CreateFilteredQueryAsync(MenuGetListInput input)
        {
         
            return (await base.CreateFilteredQueryAsync(input))
                .WhereIf(!input.OwnerId.HasValue, x => x.OwnerId == input.OwnerId)
              
                .WhereIf(!input.Keyword.IsNullOrWhiteSpace(), x => x.Name.Contains(input.Keyword))
                ;
        }

        [HttpPost]
        public override async Task<MenuDto> CreateAsync(MenuCreateInput input)
        {
            Assert.If(!await ChatObjectRepository.AnyAsync(x => x.Id == input.OwnerId), $"No such entity of OwnerId:{input.OwnerId}");

            Assert.If(input.ParentId.HasValue && !await Repository.AnyAsync(x => x.Id == input.ParentId && x.OwnerId == input.OwnerId), $"No such entity of ParentId:{input.ParentId}");

            return await base.CreateAsync(input);
        }

        [HttpPost]
        public override async Task<MenuDto> UpdateAsync(Guid id, MenuUpdateInput input)
        {
            if (input.ParentId.HasValue)
            {
                var perent = await Repository.GetAsync(input.ParentId.Value);

                var entity = await Repository.GetAsync(id);

                Assert.If(perent.OwnerId != entity.OwnerId, $"Parent owner is different,ParentId:{input.ParentId}");
            }
            return await base.UpdateAsync(id, input);
        }

        [HttpGet]
        //[UnitOfWork(true, IsolationLevel.ReadUncommitted)]
        public async Task<string> TriggerAsync(Guid id)
        {
            return await TreeManager.TriggerAsync(id);
        }
    }
}
