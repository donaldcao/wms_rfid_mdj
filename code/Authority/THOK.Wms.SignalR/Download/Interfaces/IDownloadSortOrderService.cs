using System.Threading;
using THOK.Common.SignalR.Model;

namespace THOK.Wms.SignalR.Download.Interfaces
{
    public interface IDownloadSortOrderService
    {
        void Download(string connectionId, ProgressState ps, CancellationToken cancellationToken, string beginDate, string endDate, string sortLineCode, bool isSortDown, string batch);
    }
}
