﻿using IczpNet.Chat.ChatObjects.Dtos;
using IczpNet.Chat.RoomSections.Rooms.Dtos;
using IczpNet.Chat.SessionSections.SessionUnits;
using IczpNet.Chat.SessionSections.SessionUnits.Dtos;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace IczpNet.Chat.RoomSections.Rooms
{
    public interface IRoomAppService : IApplicationService
    {
        Task<ChatObjectDto> CreateAsync(RoomCreateInput input);

        Task<ChatObjectDto> CreateByAllUsersAsync(string name);

        Task<ChatObjectDto> CreateByAllUsersWithManyAsync(string name);

        Task<List<SessionUnitSenderInfo>> InviteAsync(InviteInput input);

        Task<PagedResultDto<SessionUnitDto>> GetSameAsync(long sourceChatObjectId, long targetChatObjectId, int maxResultCount = 10,
        int skipCount = 0, string sorting = null);

        Task<int> GetSameCountAsync(long sourceChatObjectId, long targetChatObjectId);

        Task<ChatObjectDto> UpdateNameAsync(long id, string name);

        Task<ChatObjectDto> UpdatePortraitAsync(long id, string portrait);
    }
}
