﻿using IczpNet.Chat.BaseEntitys;
using IczpNet.Chat.SessionSections.SessionUnits;

namespace IczpNet.Chat.OpenedRecorders
{
    public class OpenedRecorder : BaseRecorder
    {
        protected OpenedRecorder() { }

        public OpenedRecorder(SessionUnit sessionUnit, long messageId, string deviceId) : base(sessionUnit, messageId, deviceId) { }

    }
}
