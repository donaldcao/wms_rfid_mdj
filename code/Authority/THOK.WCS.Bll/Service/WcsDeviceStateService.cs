using System;
using System.Data;
using System.Linq;
using Microsoft.Practices.Unity;
using THOK.Common.Entity;
using THOK.WCS.Bll.Interfaces;
using THOK.WCS.Dal.EntityRepository;
using THOK.WCS.Dal.Interfaces;
using THOK.WCS.DbModel;
using THOK.Wms.Dal.Interfaces;

namespace THOK.WCS.Bll.Service
{
    public class WcsDeviceStateService : ServiceBase<WcsDeviceState>, IWcsDeviceStateService
    {
        [Dependency]
        public IWcsDeviceStateRepository WcsDeviceStateRepository { get; set; }

        protected override Type LogPrefix
        {
            get { return this.GetType(); }
        }


        public object GetDetails(int page, int rows, string DeviceCode, string DeviceType, string StateCode, string BeginTime, string EndTime, string UseTime)
        {
            IQueryable<WcsDeviceState> wcsDeviceStateQuery = WcsDeviceStateRepository.GetQueryable();

            IQueryable<WcsDeviceState> wcsDeviceStateDetail1 = wcsDeviceStateQuery;
            if (DeviceCode != "" )
            {
                wcsDeviceStateDetail1 = wcsDeviceStateQuery.Where(s => s.DeviceCode == DeviceCode);
            }
            IQueryable<WcsDeviceState> wcsDeviceStateDetail2 = wcsDeviceStateDetail1;
            if (!string.IsNullOrEmpty(EndTime))
            {
                DateTime endTime = Convert.ToDateTime(EndTime);
                wcsDeviceStateDetail2 = wcsDeviceStateDetail1.Where(s => s.EndTime.Equals(endTime));
            }
            IQueryable<WcsDeviceState> wcsDeviceStateDetail3 = wcsDeviceStateDetail2;
            if (DeviceType != "")
            {
                wcsDeviceStateDetail3 = wcsDeviceStateDetail2.Where(s => s.DeviceType.Contains(DeviceType));
            }
            IQueryable<WcsDeviceState> wcsDeviceStateDetail4 = wcsDeviceStateDetail3;
            if (StateCode != "")
            {
                wcsDeviceStateDetail4 = wcsDeviceStateDetail3.Where(s => s.StateCode.Contains(StateCode));
            }
            IQueryable<WcsDeviceState> wcsDeviceStateDetail5 = wcsDeviceStateDetail4;
            if (BeginTime != string.Empty && BeginTime != null)
            {
                DateTime beginTime = Convert.ToDateTime(BeginTime);
                wcsDeviceStateDetail5 = wcsDeviceStateDetail4.Where(s => s.BeginTime.Equals(beginTime));
            }
            IQueryable<WcsDeviceState> wcsDeviceStateDetail6 = wcsDeviceStateDetail5;
            if (UseTime != null && UseTime != "")
            {
                int useTime = Convert.ToInt32(UseTime);
                wcsDeviceStateDetail6 = wcsDeviceStateDetail5.Where(s => s.UseTime.Equals(useTime));
            }

            var wcsDeviceStatesArray = wcsDeviceStateDetail6.OrderBy(s => s.Id).AsEnumerable()
                .Select(s => new
            {
                s.Id,
                s.DeviceCode,
                s.DeviceName,
                s.DeviceType,
                StateCode = s.StateCode == "1" ? "正常运行" : s.StateCode == "2" ? "正常停机" : s.StateCode == "3" ? "故障运行" : s.StateCode == "4" ? "故障停机" : "异常",
                BeginTime = s.BeginTime.ToString("yyyy-MM-dd HH:mm:ss"),
                EndTime = s.EndTime.ToString("yyyy-MM-dd HH:mm:ss"),
                s.UseTime
            });
            int total = wcsDeviceStatesArray.Count();
            wcsDeviceStatesArray = wcsDeviceStatesArray.Skip((page - 1) * rows).Take(rows);
            return new { total, rows = wcsDeviceStatesArray.ToArray() };
        }

        public DataTable GetWcsDeviceState(int page, int rows, WcsDeviceState wds)
        {
            IQueryable<WcsDeviceState> wcsDeviceStateQuery = WcsDeviceStateRepository.GetQueryable();

            var wcsDeviceStatesArray = wcsDeviceStateQuery.OrderBy(s => s.Id).AsEnumerable()
                .Select(s => new
                {
                    s.Id,
                    s.DeviceCode,
                    s.DeviceName,
                    s.DeviceType,
                    StateCode = s.StateCode == "1" ? "正常运行" : s.StateCode == "2" ? "正常停机" : s.StateCode == "3" ? "故障运行" : s.StateCode == "4" ? "故障停机" : "异常",
                    BeginTime = s.BeginTime.ToString("yyyy-MM-dd HH:mm:ss"),
                    EndTime = s.EndTime.ToString("yyyy-MM-dd HH:mm:ss"),
                    s.UseTime
                });
            System.Data.DataTable dt = new System.Data.DataTable();
            dt.Columns.Add("Id", typeof(int));
            dt.Columns.Add("设备代码", typeof(string));
            dt.Columns.Add("设备名称", typeof(string));
            dt.Columns.Add("设备类型", typeof(string));
            dt.Columns.Add("状态代码", typeof(string));
            dt.Columns.Add("开始时间", typeof(string));
            dt.Columns.Add("结束时间", typeof(string));
            dt.Columns.Add("合计用时", typeof(int));

            foreach (var item in wcsDeviceStatesArray)
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
