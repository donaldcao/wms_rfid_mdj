using System.Threading;
using THOK.Common.SignalR.Model;

namespace THOK.Wms.SignalR.Dispatch.Interfaces
{
    public interface ISortOrderWorkDispatchService
    {
        void Dispatch(string connectionId, ProgressState ps, CancellationToken cancellationToken, string workDispatchId, string userName);

        bool LowerLimitMoveLibrary(string userName, bool isEnableStocking, out string errorInfo);
    }
}
