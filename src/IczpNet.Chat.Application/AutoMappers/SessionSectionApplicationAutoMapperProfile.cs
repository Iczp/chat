﻿using AutoMapper;
using IczpNet.Chat.FriendshipTagSections.FriendshipTags.Dtos;
using IczpNet.Chat.SessionSections.FriendshipRequests;
using IczpNet.Chat.SessionSections.FriendshipRequests.Dtos;
using IczpNet.Chat.SessionSections.Friendships;
using IczpNet.Chat.SessionSections.Friendships.Dtos;
using IczpNet.Chat.SessionSections.OpenedRecorders;
using IczpNet.Chat.SessionSections.OpenedRecordes.Dtos;
using IczpNet.Chat.SessionSections.Sessions;
using IczpNet.Chat.SessionSections.SessionUnits;
using IczpNet.Chat.SessionSections.SessionUnits.Dtos;

namespace IczpNet.Chat.AutoMappers;

public class SessionSectionApplicationAutoMapperProfile : Profile
{
    public SessionSectionApplicationAutoMapperProfile()
    {
        /* You can configure your AutoMapper mapping configuration here.
         * Alternatively, you can split your mapping configurations
         * into multiple profile classes for a better organization. */


        //Session
        CreateMap<Session, SessionDto>();

        //SessionUnit
        CreateMap<SessionUnit, SessionUnitDto>();
        CreateMap<SessionUnit, SessionUnitDetailDto>();

        //Friendship
        CreateMap<Friendship, FriendshipDto>();
        CreateMap<Friendship, FriendshipDetailDto>();
        CreateMap<FriendshipCreateInput, Friendship>(MemberList.Source).IgnoreAllPropertiesWithAnInaccessibleSetter();
        CreateMap<FriendshipUpdateInput, Friendship>(MemberList.Source).IgnoreAllPropertiesWithAnInaccessibleSetter();

        //FriendshipRequest
        CreateMap<FriendshipRequest, FriendshipRequestDto>();
        CreateMap<FriendshipRequest, FriendshipRequestDetailDto>().ForMember(x => x.FriendshipIdList, o => o.MapFrom(x => x.GetFriendshipIdList()));
        CreateMap<FriendshipRequestCreateInput, FriendshipRequest>(MemberList.Source).IgnoreAllPropertiesWithAnInaccessibleSetter();
        CreateMap<FriendshipRequestUpdateInput, FriendshipRequest>(MemberList.Source).IgnoreAllPropertiesWithAnInaccessibleSetter();

        //FriendshipTag
        CreateMap<FriendshipTag, FriendshipTagDto>();
        CreateMap<FriendshipTag, FriendshipTagDetailDto>();
        CreateMap<FriendshipTag, FriendshipTagSimpleDto>();
        CreateMap<FriendshipTagCreateInput, FriendshipTag>(MemberList.Source).IgnoreAllPropertiesWithAnInaccessibleSetter();
        CreateMap<FriendshipTagUpdateInput, FriendshipTag>(MemberList.Source).IgnoreAllPropertiesWithAnInaccessibleSetter();


        //OpenedRecorder
        CreateMap<OpenedRecorder, OpenedRecorderDto>();
    }
}
