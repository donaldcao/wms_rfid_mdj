using System;
using THOK.WCS.REST.Models;

namespace THOK.WCS.REST.Interfaces
{
    public interface ITransportService
    {
        SRMTask GetSrmTask(string srmName, int travelPos, int liftPos, out string error);
        bool CancelTask(int taskid,out string error);
        bool FinishTask(int taskid,string operatorName, out string error);
    }
}
