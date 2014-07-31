using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using THOK.SMS.SignalR.Model;
using System.Threading;

namespace THOK.SMS.SignalR.Optimize.Interfaces
{
    public interface IOptimizeSortOrderService
    {
        void Optimize(string connectionId, ProgressState ps, CancellationToken cancellationToken, string id);
    }
}
