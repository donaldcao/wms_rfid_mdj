using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using THOK.Wms.SignalR.Model;

namespace THOK.SMS.SignalR.Optimize.Interfaces
{
    public interface IOptimizeSortOrderService
    {
        void Optimize(string connectionId, ProgressState ps, CancellationToken cancellationToken, string id);
    }
}
