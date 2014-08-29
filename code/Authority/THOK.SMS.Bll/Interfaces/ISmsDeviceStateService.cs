using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using THOK.SMS.DbModel;

namespace THOK.SMS.Bll.Interfaces
{
    public interface ISmsDeviceStateService : IService<SmsDeviceState>
    {
        //object GetDetails(int page, int rows, SmsDeviceState smsDeviceState);

        object GetDetails(int page, int rows, string DeviceCode, string DeviceType, string StateCode, string BeginTime, string EndTime, string UseTime);

        System.Data.DataTable GetSmsDeviceState(int page, int rows, string DeviceCode, string DeviceType, string StateCode, string BeginTime, string EndTime, string UseTime);
    }
}
