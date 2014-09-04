using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using THOK.SMS.DbModel;

namespace THOK.SMS.Bll.Interfaces
{
    public interface ISmsAlarmInfoService : IService<SmsAlarmInfo>
    {
        object GetDetails(int page, int rows, string AlarmCode, string Description);

        bool Add(SmsAlarmInfo alarmInfo, out string strResult);

        bool Save(SmsAlarmInfo alarmInfoCode, out string strResult);

        System.Data.DataTable GetAlarmInfo(int page, int rows, SmsAlarmInfo alarmInfo);

        bool Delete(string code, out string strResult);
    }
}
