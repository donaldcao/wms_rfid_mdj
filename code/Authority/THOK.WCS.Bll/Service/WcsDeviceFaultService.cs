using System;
using THOK.WCS.DbModel;
using THOK.WCS.Dal.Interfaces;
using THOK.WCS.Bll.Interfaces;
using System.Linq;
using Microsoft.Practices.Unity;
using THOK.Common.Entity;
using System.Data;
using THOK.WCS.Dal.EntityRepository;
using THOK.Wms.Dal.Interfaces;

namespace THOK.WCS.Bll.Service
{
    public class WcsDeviceFaultService : ServiceBase<WcsDeviceFault>, IWcsDeviceFaultService
    {
        [Dependency]
        public IWcsDeviceFaultRepository WcsDeviceFaultRepository { get; set; }
        [Dependency]
        public IAlarmInfoRepository AlarmInfoRepository { get; set; }

        protected override Type LogPrefix
        {
            get { return this.GetType(); }
        }


        public object GetDetails(int page, int rows, string DeviceCode, string DeviceType, string FaultCode, string BeginTime, string EndTime, string UseTime)
        {
            IQueryable<WcsDeviceFault> wcsDeviceFaultQuery = WcsDeviceFaultRepository.GetQueryable();
            var AlarmInfoQuery = AlarmInfoRepository.GetQueryable();

            IQueryable<WcsDeviceFault> wcsDeviceFaultDetail1 = wcsDeviceFaultQuery;
            if (DeviceCode != "" )
            {
                wcsDeviceFaultDetail1 = wcsDeviceFaultQuery.Where(s => s.DeviceCode == DeviceCode);
            }
            IQueryable<WcsDeviceFault> wcsDeviceFaultDetail2 = wcsDeviceFaultDetail1;
            if (!string.IsNullOrEmpty(EndTime))
            {
                DateTime endTime = Convert.ToDateTime(EndTime);
                wcsDeviceFaultDetail2 = wcsDeviceFaultDetail1.Where(s => s.EndTime.Equals(endTime));
            }
            IQueryable<WcsDeviceFault> wcsDeviceFaultDetail3 = wcsDeviceFaultDetail2;
            if (DeviceType != "")
            {
                wcsDeviceFaultDetail3 = wcsDeviceFaultDetail2.Where(s => s.DeviceType.Contains(DeviceType));
            }
            IQueryable<WcsDeviceFault> wcsDeviceFaultDetail4 = wcsDeviceFaultDetail3;
            if (FaultCode != "")
            {
                wcsDeviceFaultDetail4 = wcsDeviceFaultDetail3.Where(s => s.FaultCode.Contains(FaultCode));
            }
            IQueryable<WcsDeviceFault> wcsDeviceFaultDetail5 = wcsDeviceFaultDetail4;
            if (BeginTime != string.Empty && BeginTime != null)
            {
                DateTime beginTime = Convert.ToDateTime(BeginTime);
                wcsDeviceFaultDetail5 = wcsDeviceFaultDetail4.Where(s => s.BeginTime.Equals(beginTime));
            }
            IQueryable<WcsDeviceFault> wcsDeviceFaultDetail6 = wcsDeviceFaultDetail5;
            if (UseTime != null && UseTime != "")
            {
                int useTime = Convert.ToInt32(UseTime);
                wcsDeviceFaultDetail6 = wcsDeviceFaultDetail5.Where(s => s.UseTime.Equals(useTime));
            }
            var v1 = wcsDeviceFaultDetail6.OrderBy(a => a.Id).Select(t => t);
            int total = v1.Count();
            v1 = v1.Skip((page - 1) * rows).Take(rows);

            var wcsDeviceFaultsArray = v1.ToArray().Select(s => new
                {
                    s.Id,
                    s.DeviceCode,
                    s.DeviceName,
                    s.DeviceType,
                    FaultCode = AlarmInfoQuery.Where(c=>c.AlarmCode==s.FaultCode).Select(c=>c.AlarmCode),
                    BeginTime = s.BeginTime.ToString("yyyy-MM-dd"),
                    EndTime = s.EndTime.ToString("yyyy-MM-dd"),
                    s.UseTime
                });
            return new { total, rows = wcsDeviceFaultsArray };
        }

        public DataTable GetWcsDeviceFault(int page, int rows, string DeviceCode, string DeviceType, string FaultCode, string BeginTime, string EndTime, string UseTime)
        {
            IQueryable<WcsDeviceFault> wcsDeviceFaultQuery = WcsDeviceFaultRepository.GetQueryable();
            var AlarmInfoQuery = AlarmInfoRepository.GetQueryable();

            IQueryable<WcsDeviceFault> wcsDeviceFaultDetail1 = wcsDeviceFaultQuery;
            if (DeviceCode != "")
            {
                wcsDeviceFaultDetail1 = wcsDeviceFaultQuery.Where(s => s.DeviceCode == DeviceCode);
            }
            IQueryable<WcsDeviceFault> wcsDeviceFaultDetail2 = wcsDeviceFaultDetail1;
            if (!string.IsNullOrEmpty(EndTime))
            {
                DateTime endTime = Convert.ToDateTime(EndTime);
                wcsDeviceFaultDetail2 = wcsDeviceFaultDetail1.Where(s => s.EndTime.Equals(endTime));
            }
            IQueryable<WcsDeviceFault> wcsDeviceFaultDetail3 = wcsDeviceFaultDetail2;
            if (DeviceType != "")
            {
                wcsDeviceFaultDetail3 = wcsDeviceFaultDetail2.Where(s => s.DeviceType.Contains(DeviceType));
            }
            IQueryable<WcsDeviceFault> wcsDeviceFaultDetail4 = wcsDeviceFaultDetail3;
            if (FaultCode != "")
            {
                wcsDeviceFaultDetail4 = wcsDeviceFaultDetail3.Where(s => s.FaultCode.Contains(FaultCode));
            }
            IQueryable<WcsDeviceFault> wcsDeviceFaultDetail5 = wcsDeviceFaultDetail4;
            if (BeginTime != string.Empty && BeginTime != null)
            {
                DateTime beginTime = Convert.ToDateTime(BeginTime);
                wcsDeviceFaultDetail5 = wcsDeviceFaultDetail4.Where(s => s.BeginTime.Equals(beginTime));
            }
            IQueryable<WcsDeviceFault> wcsDeviceFaultDetail6 = wcsDeviceFaultDetail5;
            if (UseTime != null && UseTime != "")
            {
                int useTime = Convert.ToInt32(UseTime);
                wcsDeviceFaultDetail6 = wcsDeviceFaultDetail5.Where(s => s.UseTime.Equals(useTime));
            }
            var v1 = wcsDeviceFaultDetail6.OrderBy(a => a.Id).Select(t => t);
            var wcsDeviceFaultsArray = v1.OrderBy(s => s.Id).AsEnumerable()
                .Select(s => new
                {
                    s.Id,
                    s.DeviceCode,
                    s.DeviceName,
                    s.DeviceType,
                    FaultCode = AlarmInfoQuery.Where(c => c.AlarmCode == s.FaultCode).Select(c => c.AlarmCode),
                    BeginTime = s.BeginTime.ToString("yyyy-MM-dd"),
                    EndTime = s.EndTime.ToString("yyyy-MM-dd"),
                    s.UseTime
                }).ToArray();
            DataTable dt = new DataTable();
            dt.Columns.Add("Id", typeof(int));
            dt.Columns.Add("设备代码", typeof(string));
            dt.Columns.Add("设备名称", typeof(string));
            dt.Columns.Add("设备类型", typeof(string));
            dt.Columns.Add("故障代码", typeof(string));
            dt.Columns.Add("开始时间", typeof(string));
            dt.Columns.Add("结束时间", typeof(string));
            dt.Columns.Add("合计用时", typeof(int));

            foreach (var item in wcsDeviceFaultsArray)
            {
                dt.Rows.Add(
                    item.Id,
                    item.DeviceCode,
                    item.DeviceName,
                    item.DeviceType,
                    item.FaultCode,
                    item.BeginTime,
                    item.EndTime,
                    item.UseTime);
            }
            return dt;
        }
    }
}
