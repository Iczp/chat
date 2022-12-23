﻿using IczpNet.AbpCommons.DataFilters;
using IczpNet.Chat.BaseEntitys;
using IczpNet.Chat.ChatObjects;
using IczpNet.Chat.DataFilters;
using IczpNet.Chat.Enums;
using IczpNet.Chat.MessageSections.Messages;
using IczpNet.Chat.SessionSections.MessageReminders;
using IczpNet.Chat.SessionSections.SessionRoles;
using IczpNet.Chat.SessionSections.Sessions;
using IczpNet.Chat.SessionSections.SessionTags;
using IczpNet.Chat.SessionSections.SessionUnitRoles;
using IczpNet.Chat.SessionSections.SessionUnitTags;
using IczpNet.Chat.Specifications;
using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace IczpNet.Chat.SessionSections.SessionUnits
{
    public class SessionUnit : BaseSessionEntity, IChatOwner<Guid>, ISorting
    {
        public virtual Guid SessionId { get; protected set; }

        [ForeignKey(nameof(SessionId))]
        public virtual Session Session { get; protected set; }

        public virtual long ReadedMessageAutoId { get; protected set; }

        /// <summary>
        /// 为null时，
        /// </summary>
        public virtual DateTime? HistoryFristTime { get; protected set; }

        public virtual DateTime? HistoryLastTime { get; protected set; }

        /// <summary>
        /// KillSession  退出群，不删除会话
        /// </summary>
        public virtual bool IsKilled { get; protected set; }

        public virtual DateTime? ClearTime { get; protected set; }

        /// <summary>
        /// 删除消息会话,不退群
        /// </summary>
        public virtual DateTime? RemoveTime { get; protected set; }

        public virtual string Name { get; set; }

        /// <summary>
        /// 加入方式
        /// </summary>
        public virtual JoinWays? JoinWay { get; set; }

        /// <summary>
        /// 邀请人
        /// </summary>
        public virtual Guid? InviterId { get; set; }

        [ForeignKey(nameof(InviterId))]
        public virtual ChatObject Inviter { get; set; }

        public virtual double Sorting { get; private set; }

        public virtual IList<MessageReminder> ReminderList { get; protected set; } = new List<MessageReminder>();

        public virtual List<SessionUnitTag> SessionUnitTagList { get; protected set; } = new List<SessionUnitTag>();

        public virtual List<SessionUnitRole> SessionUnitRoleList { get; protected set; } = new List<SessionUnitRole>();

        [NotMapped]
        public virtual int ReminderAllCount => GetRemindAllCount();

        [NotMapped]
        public virtual int ReminderMeCount => GetRemindMeCount();

        [NotMapped]
        public virtual int Badge => GetBadge();

        [NotMapped]
        public virtual int ReminderCount => GetReminderCount();

        [NotMapped]
        public virtual Message LastMessage => Session.LastMessage;//GetLastMessage();

        [NotMapped]
        public virtual List<SessionTag> TagList => GetTagList();

        [NotMapped]
        public virtual List<Guid> TagIdList => GetTagIdList();

        [NotMapped]
        public virtual List<SessionRole> RoleList => GetRoleList();

        [NotMapped]
        public virtual List<Guid> RoleIdList => GetRoleIdList();

        protected SessionUnit() { }

        internal SessionUnit(Guid id, [NotNull] Guid sessionId, [NotNull] Guid ownerId, [NotNull] Guid destinationId) : base(id)
        {
            SessionId = sessionId;
            OwnerId = ownerId;
            DestinationId = destinationId;
        }

        internal SessionUnit(Guid id, [NotNull] Session session, [NotNull] ChatObject owner, [NotNull] ChatObject destination) : base(id)
        {
            Session = session;
            Owner = owner;
            Destination = destination;
        }

        internal void SetReaded(long messageAutoId, bool isForce = false)
        {
            if (isForce || messageAutoId > ReadedMessageAutoId)
            {
                ReadedMessageAutoId = messageAutoId;
            }
        }

        internal void SetHistoryFristTime(DateTime historyFristTime)
        {
            HistoryFristTime = historyFristTime;
        }

        /// <summary>
        /// removeSession 删除消息会话,不退群
        /// </summary>
        /// <param name="removeTime"></param>
        internal void RemoveSession(DateTime removeTime)
        {
            RemoveTime = removeTime;
        }

        /// <summary>
        /// 退群，但不删除会话（用于查看历史I）
        /// </summary>
        /// <param name="removeTime"></param>
        internal void KillSession(DateTime killTime)
        {
            IsKilled = true;
            //KillTime = killTime;
            HistoryLastTime = killTime;
        }

        /// <summary>
        /// 清空消息，不退群 
        /// </summary>
        /// <param name="clearTime"></param>
        internal void ClearMessage(DateTime? clearTime)
        {
            ClearTime = clearTime;
        }

        public override object[] GetKeys()
        {
            return new object[] { SessionId, OwnerId, DestinationId };
        }

        protected virtual int GetBadge()
        {
            return Session.MessageList.AsQueryable().Count(new SessionUnitMessageSpecification(this).ToExpression());
        }

        protected virtual Message GetLastMessage()
        {
            return Session.MessageList.AsQueryable().OrderByDescending(x => x.AutoId).FirstOrDefault(new SessionUnitMessageSpecification(this).ToExpression());
        }

        private int GetReminderCount()
        {
            return GetRemindMeCount() + GetRemindAllCount();
        }

        /// <summary>
        /// @me
        /// </summary>
        /// <returns></returns>
        protected int GetRemindMeCount()
        {
            return ReminderList.AsQueryable().Select(x => x.Message).Where(x => !x.IsRollbacked).Count(new SessionUnitMessageSpecification(this).ToExpression());
        }

        /// <summary>
        /// @everyone
        /// </summary>
        /// <returns></returns>
        protected int GetRemindAllCount()
        {
            return Session.MessageList.AsQueryable().Where(x => x.IsRemindAll && !x.IsRollbacked).Count(new SessionUnitMessageSpecification(this).ToExpression());
        }

        internal void SetTopping(bool isTopping)
        {
            Sorting = isTopping ? DateTime.Now.Ticks : 0;
        }

        private List<SessionTag> GetTagList()
        {
            return SessionUnitTagList.Select(x => x.SessionTag).ToList();
        }

        private List<Guid> GetTagIdList()
        {
            return SessionUnitTagList.Select(x => x.SessionTagId).ToList();
        }

        private List<SessionRole> GetRoleList()
        {
            return SessionUnitRoleList.Select(x => x.SessionRole).ToList();
        }

        private List<Guid> GetRoleIdList()
        {
            return SessionUnitRoleList.Select(x => x.SessionRoleId).ToList();
        }
    }
}
