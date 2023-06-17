﻿using AutoMapper;
using IczpNet.Chat.Wallets;
using IczpNet.Chat.Wallets.Dtos;
using IczpNet.Chat.Words.Dtos;
using IczpNet.Chat.Words;
using IczpNet.Chat.WalletOrders;
using IczpNet.Chat.WalletOrders.Dtos;

namespace IczpNet.Chat.AutoMappers;

public class WalletSectionApplicationAutoMapperProfile : Profile
{
    public WalletSectionApplicationAutoMapperProfile()
    {
        /* You can configure your AutoMapper mapping configuration here.
         * Alternatively, you can split your mapping configurations
         * into multiple profile classes for a better organization. */

        //Wallet
        CreateMap<Wallet, WalletDto>();
        CreateMap<Wallet, WalletDetailDto>();
        //CreateMap<WalletCreateInput, Wallet>(MemberList.Source).IgnoreAllSourcePropertiesWithAnInaccessibleSetter();
        //CreateMap<WalletUpdateInput, Wallet>(MemberList.Source).IgnoreAllSourcePropertiesWithAnInaccessibleSetter();

        //WalletOrder
        CreateMap<WalletOrder, WalletOrderDto>();
        CreateMap<WalletOrder, WalletOrderDetailDto>();
        CreateMap<WalletOrderCreateInput, WalletOrder>(MemberList.Source).IgnoreAllSourcePropertiesWithAnInaccessibleSetter();
        CreateMap<WalletOrderUpdateInput, WalletOrder>(MemberList.Source).IgnoreAllSourcePropertiesWithAnInaccessibleSetter();
    }
}
