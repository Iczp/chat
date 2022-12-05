﻿using IczpNet.Chat.BaseEntitys;
using IczpNet.Chat.Enums;
using IczpNet.Chat.Messages;
using IczpNet.Chat.OfficialSections.OfficialExcludedMembers;
using IczpNet.Chat.OfficialSections.OfficialGroupMembers;
using IczpNet.Chat.OfficialSections.OfficialMembers;
using IczpNet.Chat.Robots;
using IczpNet.Chat.RoomSections.RoomForbiddenMembers;
using IczpNet.Chat.RoomSections.RoomMembers;
using IczpNet.Chat.SessionSections.Friendships;
using IczpNet.Chat.SquareSections.SquareMembers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Volo.Abp.SimpleStateChecking;

namespace IczpNet.Chat.ChatObjects
{
    //[Index]
    public class ChatObject : BaseEntity<Guid>, IChatObject, IHasSimpleStateCheckers<ChatObject>
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public virtual long AutoId { get; set; }

        [StringLength(50)]
        [Required]
        public virtual string Name { get; set; }

        [StringLength(50)]
        public virtual string Code { get; set; }

        /// <summary>
        /// 头像
        /// </summary>
        [StringLength(300)]
        [MaxLength(300)]
        public virtual string Portrait { get; protected set; }

        public virtual Guid? AppUserId { get; protected set; }

        public virtual ChatObjectTypeEnum? ObjectType { get; protected set; }

        [StringLength(500)]
        public virtual string Description { get; set; }

        #region Message

        [InverseProperty(nameof(Message.Sender))]
        public virtual IList<Message> SenderMessageList { get; set; }

        [InverseProperty(nameof(Message.Receiver))]
        public virtual IList<Message> ReceiverMessageList { get; set; }

        #endregion

        #region Shop
        /// <summary>
        /// 兼职店小二
        /// </summary>
        [InverseProperty(nameof(ShopWaiter.Owner))]
        public virtual IList<ShopWaiter> ProxyShopWaiterList { get; set; }

        /// <summary>
        /// 兼职掌柜
        /// </summary>
        [InverseProperty(nameof(ShopKeeper.Owner))]
        public virtual IList<ShopKeeper> ProxyShopKeeperList { get; set; }

        #endregion

        #region Room

        [InverseProperty(nameof(RoomMember.Owner))]
        public virtual IList<RoomMember> InRoomMemberList { get; set; }

        [InverseProperty(nameof(RoomForbiddenMember.Owner))]
        public virtual IList<RoomForbiddenMember> InRoomForbiddenMemberList { get; set; }

        [InverseProperty(nameof(RoomMember.Inviter))]
        public virtual IList<RoomMember> InInviterList { get; set; }

        #endregion

        #region Official

        [InverseProperty(nameof(OfficialGroupMember.Owner))]
        public virtual IList<OfficialGroupMember> InOfficialGroupMemberList { get; set; }

        [InverseProperty(nameof(OfficialMember.Owner))]
        public virtual IList<OfficialMember> InOfficialMemberList { get; set; }

        [InverseProperty(nameof(OfficalExcludedMember.Owner))]
        public virtual IList<OfficalExcludedMember> InOfficalExcludedMemberList { get; set; }

        #endregion

        #region Square

        [InverseProperty(nameof(SquareMember.Owner))]
        public virtual IList<SquareMember> InSquareMemberList { get; set; }

        #endregion

        #region Friendship

        [InverseProperty(nameof(Friendship.Owner))]
        public virtual IList<Friendship> FriendList { get; set; }

        [InverseProperty(nameof(Friendship.Destination))]
        public virtual IList<Friendship> InFriendList { get; set; }

        #endregion

        #region Session

        //[InverseProperty(nameof(SessionSetting.Owner))]
        //public virtual IList<SessionSetting> OwnerSessionSettingList { get; set; }

        //[InverseProperty(nameof(SessionSetting.Destination))]
        //public virtual IList<SessionSetting> DestinationSessionSettingList { get; set; }

        #endregion

        public List<ISimpleStateChecker<ChatObject>> StateCheckers { get; }

        protected ChatObject()
        {
            StateCheckers = new List<ISimpleStateChecker<ChatObject>>();
        }

        protected ChatObject(Guid id, ChatObjectTypeEnum chatObjectType) : base(id)
        {
            ObjectType = chatObjectType;
            StateCheckers = new List<ISimpleStateChecker<ChatObject>>();
        }
    }
}
