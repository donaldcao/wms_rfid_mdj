using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.Unity;
using THOK.Wms.SignalR.Download.Interfaces;
using THOK.Wms.SignalR.Model;
using System.Threading;

namespace THOK.Wms.SignalR.Connection
{
    public class DownloadSortOrderConnection : ConnectionBase
    {
        class ActionData
        {
            public string BeginDate { get; set; }
            public string EndDate { get; set; }
            public string SortLineCode { get; set; }
            public bool IsSortDown { get; set; }
            public string Batch { get; set; }
        }

        [Dependency]
        public IDownloadSortOrderService DownloadSortOrderService { get; set; }

        protected override void Execute(string connectionId, string data, ProgressState ps, CancellationToken cancellationToken, string userName)
        {
            ActionData ad = jns.Parse<ActionData>(data);
            DownloadSortOrderService.Download(connectionId, ps, cancellationToken, ad.BeginDate, ad.EndDate, ad.SortLineCode, ad.IsSortDown, ad.Batch);
        }     
    }
}
