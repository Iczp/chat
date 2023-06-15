﻿using Castle.Core.Internal;
using IczpNet.AbpCommons;
using IczpNet.AbpCommons.Extensions;
using IczpNet.Chat.BaseAppServices;
using IczpNet.Chat.Enums.Dtos;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;

namespace IczpNet.Chat.Enums
{
    public class EnumAppService : ChatAppService, IEnumAppService
    {
        protected static List<EnumTypeDto> EnumItems;

        public async Task<PagedResultDto<EnumTypeDto>> GetListAsync(EnumGetListInput input)
        {
            EnumItems ??= typeof(ChatDomainSharedModule).Assembly.GetExportedTypes()
                .Where(x => x.IsEnum)
                .Select(x => new EnumTypeDto()
                {
                    Type = x.FullName,
                    Description = x.GetTypeAttribute<DescriptionAttribute>()?.Description,
                    Names = Enum.GetNames(x)
                })
                .ToList();
            var query = EnumItems.AsQueryable()
                .WhereIf(!input.Keyword.IsNullOrWhiteSpace(), x => x.Description.Contains(input.Keyword) || x.Names.Contains(input.Keyword));

            return await GetPagedListAsync<EnumTypeDto, EnumTypeDto>(query, input);
        }

        public async Task<List<EnumDto>> GetItemsAsync(string type)
        {
            await Task.Yield();

            var _type = typeof(ChatDomainSharedModule).Assembly.GetType(type);

            Assert.NotNull(_type, $"Not found:{type}");

            return Enum.GetNames(_type)
               .Select(x => new EnumDto()
               {
                   Name = x,
                   Value = (int)Enum.Parse(_type, x),
                   Description = ((Enum)Enum.Parse(_type, x)).GetDescription()
               })
               .ToList();
        }
    }
}
