using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using THOK.WCS.DbModel;

namespace THOK.WCS.Bll.Interfaces
{
    public interface IWcsDeviceStateService : IService<WcsDeviceState>
    {
        object GetDetails(int page, int rows, string DeviceCode, string DeviceType, string StateCode, string BeginTime, string EndTime, string UseTime);

        System.Data.DataTable GetWcsDeviceState(int page, int rows, WcsDeviceState wds);
    }
}
