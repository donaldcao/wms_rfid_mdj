using System.Threading;
using Microsoft.Practices.Unity;
using THOK.SMS.SignalR.Optimize.Interfaces;
using THOK.Common.SignalR.Model;
using THOK.Common.SignalR.Connection;
using THOK.Common.SignalR;

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
            ((Notifier<OptimizeSortOrderConnection>)OptimizeSortOrderService).ConnectionId = connectionId;
            ((Notifier<OptimizeSortOrderConnection>)OptimizeSortOrderService).ProgressState = ps;
            ((Notifier<OptimizeSortOrderConnection>)OptimizeSortOrderService).CancellationToken = cancellationToken;
            OptimizeSortOrderService.Optimize(connectionId, ps, cancellationToken, ad.sortBatchId);
        }     
    }
}
