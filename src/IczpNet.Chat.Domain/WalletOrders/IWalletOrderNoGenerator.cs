﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IczpNet.Chat.WalletOrders
{
    public interface IWalletOrderNoGenerator
    {
        Task<string> MakeAsync();
    }
}
