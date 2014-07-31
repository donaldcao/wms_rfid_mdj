﻿using System.Threading;
using THOK.Common.SignalR.Model;

namespace THOK.Wms.SignalR.Allot.Interfaces
{
    public interface IOutBillAllotService
    {
        void Allot(string connectionId, ProgressState ps, CancellationToken cancellationToken, string billNo, string[] areaCodes);
    }
}
