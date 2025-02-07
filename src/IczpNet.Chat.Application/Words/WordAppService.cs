﻿using IczpNet.AbpCommons;
using IczpNet.Chat.BaseAppServices;
using IczpNet.Chat.Permissions;
using IczpNet.Chat.Words.Dtos;
using System;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace IczpNet.Chat.Words;

/// <summary>
/// 关键字
/// </summary>
public class WordAppService
    : CrudChatAppService<
        Word,
        WordDetailDto,
        WordDto,
        Guid,
        WordGetListInput,
        WordCreateInput,
        WordUpdateInput>,
    IWordAppService
{
    protected override string GetPolicyName { get; set; } = ChatPermissions.WordPermission.Default;
    protected override string GetListPolicyName { get; set; } = ChatPermissions.WordPermission.Default;
    protected override string CreatePolicyName { get; set; } = ChatPermissions.WordPermission.Create;
    protected override string UpdatePolicyName { get; set; } = ChatPermissions.WordPermission.Update;
    protected override string DeletePolicyName { get; set; } = ChatPermissions.WordPermission.Delete;

    public WordAppService(IRepository<Word, Guid> repository) : base(repository)
    {
    }

    protected override async Task<IQueryable<Word>> CreateFilteredQueryAsync(WordGetListInput input)
    {
        return (await ReadOnlyRepository.GetQueryableAsync())
            .WhereIf(!input.Keyword.IsNullOrEmpty(), x => x.Value.Contains(input.Keyword));
    }


    protected override async Task CheckCreateAsync(WordCreateInput input)
    {
        Assert.If(await Repository.AnyAsync(x => x.Value.Equals(input.Value)), $"Already exists [{input.Value}] ");
        await base.CheckCreateAsync(input);
    }

    protected override async Task CheckUpdateAsync(Guid id, Word entity, WordUpdateInput input)
    {
        //Assert.If(await Repository.AnyAsync(x => x.Id.Equals(id) && x.Id.Equals(id)), $"Already exists [{input.Value}] name:{id}");
        await base.CheckUpdateAsync(id, entity, input);
    }
}
