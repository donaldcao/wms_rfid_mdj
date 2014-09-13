using System;

namespace THOK.SMS.REST.Interfaces
{
    public interface ISupplyService
    {
        bool CreateSupplyTask(int position, int quantity, DateTime orderdate, int batchNo, out string error);

        bool AssignTaskOriginPositionAddress(out string error);

        bool CheckSupplyPositionStorage(out string error);
    }
}
