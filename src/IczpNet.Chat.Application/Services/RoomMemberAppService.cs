﻿using IczpNet.Chat.BaseAppServices;
using IczpNet.Chat.RoomSections.RoomMembers;
using IczpNet.Chat.RoomSections.RoomMembers.Dtos;
using System;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace IczpNet.Chat.Services
{
    public class RoomMemberAppService
        : CrudChatAppService<
            RoomMember,
            RoomMemberDetailDto,
            RoomMemberDto,
            Guid,
            RoomMemberGetListInput,
            RoomMemberCreateInput,
            RoomMemberUpdateInput>,
        IRoomMemberAppService
    {
        public RoomMemberAppService(IRepository<RoomMember, Guid> repository) : base(repository)
        {
        }

        protected override async Task<IQueryable<RoomMember>> CreateFilteredQueryAsync(RoomMemberGetListInput input)
        {
            return (await base.CreateFilteredQueryAsync(input))
                .WhereIf(input.OwnerId.HasValue, x => x.OwnerId == input.OwnerId)
                .WhereIf(input.RoomId.HasValue, x => x.RoomId == input.RoomId)
                .WhereIf(input.RoomRoleId.HasValue, x => x.MemberRoleList.Any(d => d.RoomRoleId == input.RoomRoleId))
                ;
        }
    }
}
