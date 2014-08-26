using System.Threading;
using THOK.Common.SignalR.Model;

namespace THOK.SMS.SignalR.Optimize.Interfaces
{
    public interface IOptimizeService
    {
        void Optimize(int sortBatchID);
    }
}
