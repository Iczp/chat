﻿using System.ComponentModel.DataAnnotations.Schema;
using System;
using IczpNet.Chat.SessionSections.SessionUnits;
using IczpNet.Chat.BaseEntitys;
using IczpNet.Chat.SessionSections.SessionOrganizations;

namespace IczpNet.Chat.SessionSections.SessionUnitOrganizations
{

    public class SessionUnitOrganization : BaseEntity
    {
        public virtual Guid SessionUnitId { get; set; }

        [ForeignKey(nameof(SessionUnitId))]
        public virtual SessionUnit SessionUnit { get; set; }

        public virtual long SessionOrganizationId { get; set; }

        [ForeignKey(nameof(SessionOrganizationId))]
        public virtual SessionOrganization SessionOrganization { get; set; }

        protected SessionUnitOrganization() { }

        public SessionUnitOrganization(SessionOrganization sessionOrganization, SessionUnit sessionUnit)
        {
            SessionOrganization = sessionOrganization;
            SessionUnit = sessionUnit;
        }

        public override object[] GetKeys()
        {
            return new object[] { SessionUnitId, SessionOrganizationId };
        }
    }
}