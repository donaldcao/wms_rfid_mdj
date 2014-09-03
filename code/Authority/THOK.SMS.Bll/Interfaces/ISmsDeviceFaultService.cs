using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using THOK.SMS.DbModel;

namespace THOK.SMS.Bll.Interfaces
{
    public interface ISmsDeviceFaultService : IService<SmsDeviceFault>
    {
        object GetDetails(int page, int rows, string DeviceCode, string DeviceType, string FaultCode, string BeginTime, string EndTime, string UseTime);

        System.Data.DataTable GetSmsDeviceFault(int page, int rows, SmsDeviceFault sdf);
    }
}
