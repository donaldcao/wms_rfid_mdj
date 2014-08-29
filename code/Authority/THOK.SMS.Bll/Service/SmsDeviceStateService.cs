using System;
using THOK.SMS.DbModel;
using THOK.SMS.Dal.Interfaces;
using THOK.SMS.Bll.Interfaces;
using System.Linq;
using Microsoft.Practices.Unity;
using THOK.Common.Entity;
using System.Data;
using THOK.SMS.Dal.EntityRepository;
using THOK.Wms.Dal.Interfaces;

namespace THOK.SMS.Bll.Service
{
    public class SmsDeviceStateService : ServiceBase<SmsDeviceState>, ISmsDeviceStateService
    {
        [Dependency]
        public ISmsDeviceStateRepository SmsDeviceStateRepository { get; set; }

        protected override Type LogPrefix
        {
            get { return this.GetType(); }
        }


        public object GetDetails(int page, int rows, string DeviceCode, string DeviceType, string StateCode, string BeginTime, string EndTime, string UseTime)
        {
            IQueryable<SmsDeviceState> smsDeviceStateQuery = SmsDeviceStateRepository.GetQueryable();

            IQueryable<SmsDeviceState> smsDeviceStateDetail1 = smsDeviceStateQuery;
            if (DeviceCode != "" )
            {
                smsDeviceStateDetail1 = smsDeviceStateQuery.Where(s => s.DeviceCode == DeviceCode);
            }
            IQueryable<SmsDeviceState> smsDeviceStateDetail2 = smsDeviceStateDetail1;
            if (!string.IsNullOrEmpty(EndTime))
            {
                DateTime endTime = Convert.ToDateTime(EndTime);
                smsDeviceStateDetail2 = smsDeviceStateDetail1.Where(s => s.EndTime.Equals(endTime));
            }
            IQueryable<SmsDeviceState> smsDeviceStateDetail3 = smsDeviceStateDetail2;
            if (DeviceType != "")
            {
                smsDeviceStateDetail3 = smsDeviceStateDetail2.Where(s => s.DeviceType.Contains(DeviceType));
            }
            IQueryable<SmsDeviceState> smsDeviceStateDetail4 = smsDeviceStateDetail3;
            if (StateCode != "")
            {
                smsDeviceStateDetail4 = smsDeviceStateDetail3.Where(s => s.StateCode.Contains(StateCode));
            }
            IQueryable<SmsDeviceState> smsDeviceStateDetail5 = smsDeviceStateDetail4;
            if (BeginTime != string.Empty && BeginTime != null)
            {
                DateTime beginTime = Convert.ToDateTime(BeginTime);
                smsDeviceStateDetail5 = smsDeviceStateDetail4.Where(s => s.BeginTime.Equals(beginTime));
            }
            IQueryable<SmsDeviceState> smsDeviceStateDetail6 = smsDeviceStateDetail5;
            if (UseTime != null && UseTime != "")
            {
                int useTime = Convert.ToInt32(UseTime);
                smsDeviceStateDetail6 = smsDeviceStateDetail5.Where(s => s.UseTime.Equals(useTime));
            }
            var v1 = smsDeviceStateDetail6.OrderBy(a => a.Id).Select(t => t);
            int total = v1.Count();
            v1 = v1.Skip((page - 1) * rows).Take(rows);

            var smsDeviceStatesArray = v1.ToArray().Select(s => new
                {
                    s.Id,
                    s.DeviceCode,
                    s.DeviceName,
                    s.DeviceType,
                    StateCode = s.StateCode == "1" ? "正常运行" : s.StateCode =="2" ? "正常停机" : s.StateCode == "3" ? "故障运行" : s.StateCode == "4" ? "故障停机":"异常",
                    BeginTime = s.BeginTime.ToString("yyyy-MM-dd"),
                    EndTime = s.EndTime.ToString("yyyy-MM-dd"),
                    s.UseTime
                });
            return new { total, rows = smsDeviceStatesArray };
        }

        public DataTable GetSmsDeviceState(int page, int rows, string DeviceCode, string DeviceType, string StateCode, string BeginTime, string EndTime, string UseTime)
        {
            IQueryable<SmsDeviceState> smsDeviceStateQuery = SmsDeviceStateRepository.GetQueryable();
            IQueryable<SmsDeviceState> smsDeviceStateDetail1 = smsDeviceStateQuery;
            if (DeviceCode != "")
            {
                smsDeviceStateDetail1 = smsDeviceStateQuery.Where(s => s.DeviceCode == DeviceCode);
            }
            IQueryable<SmsDeviceState> smsDeviceStateDetail2 = smsDeviceStateDetail1;
            if (!string.IsNullOrEmpty(EndTime))
            {
                DateTime endTime = Convert.ToDateTime(EndTime);
                smsDeviceStateDetail2 = smsDeviceStateDetail1.Where(s => s.EndTime.Equals(endTime));
            }
            IQueryable<SmsDeviceState> smsDeviceStateDetail3 = smsDeviceStateDetail2;
            if (DeviceType != "")
            {
                smsDeviceStateDetail3 = smsDeviceStateDetail2.Where(s => s.DeviceType.Contains(DeviceType));
            }
            IQueryable<SmsDeviceState> smsDeviceStateDetail4 = smsDeviceStateDetail3;
            if (StateCode != "")
            {
                smsDeviceStateDetail4 = smsDeviceStateDetail3.Where(s => s.StateCode.Contains(StateCode));
            }
            IQueryable<SmsDeviceState> smsDeviceStateDetail5 = smsDeviceStateDetail4;
            if (BeginTime != string.Empty && BeginTime != null)
            {
                DateTime beginTime = Convert.ToDateTime(BeginTime);
                smsDeviceStateDetail5 = smsDeviceStateDetail4.Where(s => s.BeginTime.Equals(beginTime));
            }
            IQueryable<SmsDeviceState> smsDeviceStateDetail6 = smsDeviceStateDetail5;
            if (UseTime != null && UseTime != "")
            {
                int useTime = Convert.ToInt32(UseTime);
                smsDeviceStateDetail6 = smsDeviceStateDetail5.Where(s => s.UseTime.Equals(useTime));
            }
            var v1 = smsDeviceStateDetail6.OrderBy(a => a.Id).Select(t => t);
            var smsDeviceStatesArray = v1.OrderBy(s => s.Id).AsEnumerable()
                .Select(s => new
                {
                    s.Id,
                    s.DeviceCode,
                    s.DeviceName,
                    s.DeviceType,
                    StateCode = s.StateCode == "1" ? "正常运行" : s.StateCode == "2" ? "正常停机" : s.StateCode == "3" ? "故障运行" : s.StateCode == "4" ? "故障停机" : "异常",
                    BeginTime = s.BeginTime.ToString("yyyy-MM-dd"),
                    EndTime = s.EndTime.ToString("yyyy-MM-dd"),
                    s.UseTime
                }).ToArray();
            DataTable dt = new DataTable();
            dt.Columns.Add("Id", typeof(int));
            dt.Columns.Add("设备代码", typeof(string));
            dt.Columns.Add("设备名称", typeof(string));
            dt.Columns.Add("设备类型", typeof(string));
            dt.Columns.Add("状态代码", typeof(string));
            dt.Columns.Add("开始时间", typeof(string));
            dt.Columns.Add("结束时间", typeof(string));
            dt.Columns.Add("合计用时", typeof(int));

            foreach (var item in smsDeviceStatesArray)
            {
                dt.Rows.Add(
                    item.Id,
                    item.DeviceCode,
                    item.DeviceName,
                    item.DeviceType,
                    item.StateCode,
                    item.BeginTime,
                    item.EndTime,
                    item.UseTime);
            }
            return dt;
        }
    }
}
