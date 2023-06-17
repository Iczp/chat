﻿using IczpNet.AbpCommons;
using IczpNet.Chat.BaseEntities;
using IczpNet.Chat.ChatObjects;
using IczpNet.Chat.DataFilters;
using IczpNet.Chat.Enums;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IczpNet.Chat.Wallets
{
    public class Wallet : BaseEntity<Guid>, IChatOwner<long>
    {
        public virtual Guid? AppUserId { get; protected set; }

        public virtual long OwnerId { get; protected set; }

        [ForeignKey(nameof(OwnerId))]
        public virtual ChatObject Owner { get; protected set; }

        [Column(TypeName = "decimal(18,2)")]
        [Range(0.0, (double)decimal.MaxValue)]
        [Comment("可用金额")]
        public virtual decimal AvailableAmount { get; protected set; }


        [Column(TypeName = "decimal(18,2)")]
        [Range(0.0, (double)decimal.MaxValue)]
        [Comment("冻结金额")]
        public virtual decimal LockAmount { get; protected set; }

        [Column(TypeName = "decimal(18,2)")]
        [Comment("总金额=可用金额+冻结金额")]
        public virtual decimal TotalAmount { get; protected set; }

        [StringLength(100)]
        [Comment("密码")]
        public virtual string Password { get; protected set; }

        public virtual List<WalletRecorder> WalletRecorderList { get; protected set; } = new List<WalletRecorder>();

        protected Wallet()
        {

        }

        private void UpdateTotalAmount()
        {
            TotalAmount = LockAmount + AvailableAmount;
        }

        public Wallet(Guid id, long ownerId ) : base(id)
        {
            OwnerId = ownerId;
        }

        /// <summary>
        /// 支出
        /// </summary>
        /// <param name="amount"></param>
        /// <param name="walletRecorder"></param>
        internal void Expenditure(decimal amount, WalletRecorder walletRecorder)
        {
            Assert.If(walletRecorder.WalletBusinessType != WalletBusinessTypes.Expenditure, "");
            SetOriginalAvailableAmount();
            AvailableAmount -= amount;
            UpdateTotalAmount();
            WalletRecorderList.Add(walletRecorder);
        }

        private void SetOriginalAvailableAmount()
        {

        }

        /// <summary>
        /// 收入
        /// </summary>
        /// <param name="amount"></param>
        /// <param name="walletRecorder"></param>
        internal void Income(decimal amount, WalletRecorder walletRecorder)
        {
            Assert.If(walletRecorder.WalletBusinessType != WalletBusinessTypes.Income, "");
            SetOriginalAvailableAmount();
            AvailableAmount += amount;
            UpdateTotalAmount();
            WalletRecorderList.Add(walletRecorder);
        }


        ///// <summary>
        ///// 支出
        ///// </summary>
        ///// <param name="amount"></param>
        ///// <param name="description"></param>
        //public virtual void ExpenditureAsync(Guid recordId, decimal amount, string description)
        //{
        //    WalletRecorderList.Add(new WalletRecorder(recordId, this, Owner, amount, description));
        //    AvailableAmount -= amount;
        //}

        ///// <summary>
        ///// 收入
        ///// </summary>
        ///// <param name="amount"></param>
        ///// <param name="description"></param>
        //public virtual void IncomeAsync(Guid recordId, decimal amount, string description)
        //{
        //    WalletRecorderList.Add(new WalletRecorder(recordId, this, Owner, amount, description));
        //    AvailableAmount += amount;
        //}
    }
}
