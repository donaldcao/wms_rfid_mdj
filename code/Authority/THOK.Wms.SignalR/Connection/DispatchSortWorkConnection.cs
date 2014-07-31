using Microsoft.Practices.Unity;
using THOK.Wms.SignalR.Dispatch.Interfaces;
using System.Threading;
using THOK.Common.SignalR.Connection;
using THOK.Common.SignalR.Model;

namespace THOK.Wms.SignalR.Connection
{
    public class DispatchSortWorkConnection : ConnectionBase
    {
        class ActionData
        {
            public string ActionType { get; set; }
            public string workDispatchId { get; set; }
        }

        [Dependency]
        public ISortOrderWorkDispatchService SortOrderWorkDispatchService { get; set; }

        protected override void Execute(string connectionId, string data, ProgressState ps, CancellationToken cancellationToken,string userName)
        {            
            ActionData ad = jns.Parse<ActionData>(data);
            SortOrderWorkDispatchService.Dispatch(connectionId, ps, cancellationToken, ad.workDispatchId,userName);
        }      
    }
}
