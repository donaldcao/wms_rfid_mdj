using System;
using THOK.WCS.REST.Models;

namespace THOK.WCS.REST.Interfaces
{
    public interface ITransportService
    {
        bool Arrive(string positionName, string barcode, out string error);
        bool Arrive(string positionName, int taskid, out string error);

        SRMTask GetSrmTask(string srmName, int travelPos, int liftPos, out string error);
        bool CancelTask(int taskid,out string error);
        bool FinishTask(int taskid,string operatorName, out string error);
    }
}
