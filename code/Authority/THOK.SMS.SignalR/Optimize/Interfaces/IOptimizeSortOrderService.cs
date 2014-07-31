using System.Threading;
using THOK.Common.SignalR.Model;

namespace THOK.SMS.SignalR.Optimize.Interfaces
{
    public interface IOptimizeSortOrderService
    {
        void Optimize(string connectionId, ProgressState ps, CancellationToken cancellationToken, string id);
    }
}
