﻿using AutoMapper;
using IczpNet.Chat.ChatObjects.Dtos;
using IczpNet.Chat.Rooms;
using IczpNet.Chat.Rooms.Dtos;

namespace IczpNet.Chat.AutoMappers;

public class ChatApplicationAutoMapperProfile : Profile
{
    public ChatApplicationAutoMapperProfile()
    {
        /* You can configure your AutoMapper mapping configuration here.
         * Alternatively, you can split your mapping configurations
         * into multiple profile classes for a better organization. */

        //Room
        CreateMap<Room, RoomDto>();
        CreateMap<Room, RoomDetailDto>();
        CreateMap<RoomCreateInput, Room>(MemberList.Source).IgnoreAllPropertiesWithAnInaccessibleSetter();
        CreateMap<RoomUpdateInput, Room>(MemberList.Source).IgnoreAllPropertiesWithAnInaccessibleSetter();
    }
}
