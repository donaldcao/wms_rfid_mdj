using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using THOK.WCS.DbModel;

namespace THOK.WCS.Bll.Interfaces
{
    public interface IWcsDeviceFaultService : IService<WcsDeviceFault>
    {
        object GetDetails(int page, int rows, string DeviceCode, string DeviceType, string FaultCode, string BeginTime, string EndTime, string UseTime);

        System.Data.DataTable GetWcsDeviceFault(int page, int rows, string DeviceCode, string DeviceType, string FaultCode, string BeginTime, string EndTime, string UseTime);
    }
}
