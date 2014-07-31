using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Microsoft.Practices.Unity;
using THOK.SMS.SignalR.Model;
using THOK.SMS.SignalR.Optimize.Interfaces;

namespace THOK.SMS.SignalR.Connection
{
    public class OptimizeSortOrderConnection : ConnectionBase
    {
        class ActionData
        {
            public string sortBatchId { get; set; }
        }

        [Dependency]
        public IOptimizeSortOrderService OptimizeSortOrderService { get; set; }

        protected override void Execute(string connectionId, string data, ProgressState ps, CancellationToken cancellationToken, string userName)
        {
            ActionData ad = jns.Parse<ActionData>(data);
            OptimizeSortOrderService.Optimize(connectionId, ps, cancellationToken, ad.sortBatchId);
        }     
    }
}
