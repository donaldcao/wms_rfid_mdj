using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using THOK.SMS.SignalR.Connection;
using THOK.SMS.SignalR.Optimize.Interfaces;
using THOK.SMS.SignalR.Model;
using System.Threading;

namespace THOK.SMS.SignalR.Optimize.Service
{
    public class OptimizeSortOrderService : Notifier<OptimizeSortOrderConnection>, IOptimizeSortOrderService
    {
        public void Optimize(string connectionId, ProgressState ps, CancellationToken cancellationToken, string sortBatchId)
        {
            ps.State = StateType.Start;
            ps.Messages.Add("开始优化");
            NotifyConnection(ps.Clone());
        }

        void StateTypeForProcessing(ProgressState ps, string TotalName, int TotalValue, string CurrentName, int CurrentValue)
        {
            ps.State = StateType.Processing;
            ps.TotalProgressName = TotalName;
            ps.TotalProgressValue = TotalValue;
            ps.CurrentProgressName = CurrentName;
            ps.CurrentProgressValue = CurrentValue;
            NotifyConnection(ps.Clone());
        }
    }
}
