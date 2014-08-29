using THOK.WCS.REST.Interfaces;

namespace THOK.WCS.REST.Service
{
    public class TransportService :ITransportService
    {
        public bool Arrive(int taskid, string positionName, out string error)
        {
            throw new System.NotImplementedException();
        }

        public bool GetSrmTask(string srmName, int travelPos, int liftPos, out string error)
        {
            throw new System.NotImplementedException();
        }

        public bool GetForkliftTask(string norkliftName, string[] tasktypes, int mode, out string error)
        {
            throw new System.NotImplementedException();
        }

        public bool ApplyTask(int taskid, out string error)
        {
            throw new System.NotImplementedException();
        }

        public bool CancelTask(int taskid, out string error)
        {
            throw new System.NotImplementedException();
        }

        public bool FinishTask(int taskid, string operatorName, out string error)
        {
            throw new System.NotImplementedException();
        }
    }
}
