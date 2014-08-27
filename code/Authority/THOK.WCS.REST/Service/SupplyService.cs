using THOK.WCS.REST.Interfaces;

namespace THOK.WCS.REST.Service
{
    public class SupplyService :ISupplyService
    {
        public bool CreateSupplyTask(int position, int quantity, System.DateTime orderdate, int batchNo, out string error)
        {
            throw new System.NotImplementedException();
        }

        public bool AssignTaskOriginPositionAddress(out string error)
        {
            throw new System.NotImplementedException();
        }
    }
}
