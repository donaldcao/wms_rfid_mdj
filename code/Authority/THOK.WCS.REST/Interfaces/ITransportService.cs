using System;

namespace THOK.WCS.REST.Interfaces
{
    public interface ITransportService
    {
        bool Arrive(int taskid, string positionName, out string error);
        bool GetSrmTask(string srmName, int travelPos, int liftPos, out string error);
        bool GetForkliftTask(string norkliftName, string[] tasktypes, int mode, out string error);
        bool ApplyTask(int taskid,out string error);
        bool CancelTask(int taskid,out string error);
        bool FinishTask(int taskid,string operatorName, out string error);
    }
}
