﻿using AutoMapper;
using IczpNet.Chat.Articles;
using IczpNet.Chat.Articles.Dtos;
using IczpNet.Chat.ChatObjects;
using IczpNet.Chat.ChatObjects.Dtos;
using IczpNet.Chat.Connections;
using IczpNet.Chat.Connections.Dtos;
using IczpNet.Chat.SquareSections.SquareCategorys.Dtos;
using IczpNet.Chat.SquareSections.SquareCategorys;
using IczpNet.Chat.ChatObjectCategorys;
using IczpNet.Chat.ChatObjectCategorys.Dtos;
using IczpNet.Chat.SquareSections.Squares.Dtos;
using IczpNet.Chat.SquareSections.Squares;
using IczpNet.Chat.ChatObjectTypes;
using IczpNet.Chat.ChatObjectTypes.Dtos;

namespace IczpNet.Chat.AutoMappers;

public class ChatApplicationAutoMapperProfile : Profile
{
    public ChatApplicationAutoMapperProfile()
    {
        /* You can configure your AutoMapper mapping configuration here.
         * Alternatively, you can split your mapping configurations
         * into multiple profile classes for a better organization. */

        //ChatObject
        CreateMap<ChatObject, ChatObjectDto>();
        CreateMap<ChatObject, ChatObjectSimpleDto>();
        CreateMap<ChatObject, ChatObjectDetailDto>()
            .ForMember(x => x.SenderMessageCount, o => o.MapFrom(x => x.SenderMessageList.Count))
            .ForMember(x => x.ReceiverMessageCount, o => o.MapFrom(x => x.ReceiverMessageList.Count))
            .ForMember(x => x.InSquareMemberCount, o => o.MapFrom(x => x.InSquareMemberList.Count))
            .ForMember(x => x.InRoomMemberCount, o => o.MapFrom(x => x.InRoomMemberList.Count))
            .ForMember(x => x.InRoomForbiddenMemberCount, o => o.MapFrom(x => x.InRoomForbiddenMemberList.Count))
            .ForMember(x => x.InOfficialMemberCount, o => o.MapFrom(x => x.InOfficialMemberList.Count))
            .ForMember(x => x.InOfficialGroupMemberCount, o => o.MapFrom(x => x.InOfficialGroupMemberList.Count))
            .ForMember(x => x.InOfficalExcludedMemberCount, o => o.MapFrom(x => x.InOfficalExcludedMemberList.Count))
            .ForMember(x => x.FriendCount, o => o.MapFrom(x => x.OwnerFriendshipList.Count))
            .ForMember(x => x.InFriendCount, o => o.MapFrom(x => x.DestinationFriendshipList.Count))
            .ForMember(x => x.ProxyShopKeeperCount, o => o.MapFrom(x => x.ProxyShopKeeperList.Count))
            .ForMember(x => x.ProxyShopWaiterCount, o => o.MapFrom(x => x.ProxyShopWaiterList.Count))
            ;
        CreateMap<ChatObjectCreateInput, ChatObject>(MemberList.Source).IgnoreAllSourcePropertiesWithAnInaccessibleSetter();
        CreateMap<ChatObjectUpdateInput, ChatObject>(MemberList.Source).IgnoreAllSourcePropertiesWithAnInaccessibleSetter();


        //ChatObjectCategory
        CreateMap<ChatObjectCategory, ChatObjectCategoryDto>();
        CreateMap<ChatObjectCategory, ChatObjectCategoryDetailDto>();
        CreateMap<ChatObjectCategoryCreateInput, ChatObjectCategory>(MemberList.Source).IgnoreAllSourcePropertiesWithAnInaccessibleSetter();
        CreateMap<ChatObjectCategoryUpdateInput, ChatObjectCategory>(MemberList.Source).IgnoreAllSourcePropertiesWithAnInaccessibleSetter();
        CreateMap<ChatObjectCategory, ChatObjectCategoryInfo>();


        //ChatObjectType
        CreateMap<ChatObjectType, ChatObjectTypeDto>().ForMember(x => x.ChatObjectCount, o => o.MapFrom(x => x.GetChatObjectCount()));
        CreateMap<ChatObjectType, ChatObjectTypeDetailDto>().ForMember(x => x.ChatObjectCount, o => o.MapFrom(x => x.GetChatObjectCount()));
        CreateMap<ChatObjectTypeCreateInput, ChatObjectType>(MemberList.Source).IgnoreAllSourcePropertiesWithAnInaccessibleSetter();
        CreateMap<ChatObjectTypeUpdateInput, ChatObjectType>(MemberList.Source).IgnoreAllSourcePropertiesWithAnInaccessibleSetter();

        //Article
        CreateMap<Article, ArticleDto>();
        CreateMap<Article, ArticleDetailDto>();
        CreateMap<ArticleCreateInput, Article>(MemberList.None).IgnoreAllSourcePropertiesWithAnInaccessibleSetter();
        CreateMap<ArticleUpdateInput, Article>(MemberList.Source).IgnoreAllSourcePropertiesWithAnInaccessibleSetter();

        //Connection
        CreateMap<Connection, ConnectionDto>();
        CreateMap<Connection, ConnectionDetailDto>();
        CreateMap<ConnectionCreateInput, Connection>(MemberList.None).IgnoreAllSourcePropertiesWithAnInaccessibleSetter();
        //CreateMap<ConnectionUpdateInput, Connection>(MemberList.Source).IgnoreAllPropertiesWithAnInaccessibleSetter();



    }
}
