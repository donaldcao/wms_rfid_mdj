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
    public class SmsDeviceFaultService : ServiceBase<SmsDeviceFault>, ISmsDeviceFaultService
    {
        [Dependency]
        public ISmsDeviceFaultRepository SmsDeviceFaultRepository { get; set; }
        [Dependency]
        public ISmsAlarmInfoRepository SmsAlarmInfoRepository { get; set; }

        protected override Type LogPrefix
        {
            get { return this.GetType(); }
        }


        public object GetDetails(int page, int rows, string DeviceCode, string DeviceType, string FaultCode, string BeginTime, string EndTime, string UseTime)
        {
            IQueryable<SmsDeviceFault> smsDeviceFaultQuery = SmsDeviceFaultRepository.GetQueryable();
            var smsAlarmInfoQuery = SmsAlarmInfoRepository.GetQueryable();

            IQueryable<SmsDeviceFault> smsDeviceFaultDetail1 = smsDeviceFaultQuery;
            if (DeviceCode != "" )
            {
                smsDeviceFaultDetail1 = smsDeviceFaultQuery.Where(s => s.DeviceCode == DeviceCode);
            }
            IQueryable<SmsDeviceFault> smsDeviceFaultDetail2 = smsDeviceFaultDetail1;
            if (!string.IsNullOrEmpty(EndTime))
            {
                DateTime endTime = Convert.ToDateTime(EndTime);
                smsDeviceFaultDetail2 = smsDeviceFaultDetail1.Where(s => s.EndTime.Equals(endTime));
            }
            IQueryable<SmsDeviceFault> smsDeviceFaultDetail3 = smsDeviceFaultDetail2;
            if (DeviceType != "")
            {
                smsDeviceFaultDetail3 = smsDeviceFaultDetail2.Where(s => s.DeviceType.Contains(DeviceType));
            }
            IQueryable<SmsDeviceFault> smsDeviceFaultDetail4 = smsDeviceFaultDetail3;
            if (FaultCode != "")
            {
                smsDeviceFaultDetail4 = smsDeviceFaultDetail3.Where(s => s.FaultCode.Contains(FaultCode));
            }
            IQueryable<SmsDeviceFault> smsDeviceFaultDetail5 = smsDeviceFaultDetail4;
            if (BeginTime != string.Empty && BeginTime != null)
            {
                DateTime beginTime = Convert.ToDateTime(BeginTime);
                smsDeviceFaultDetail5 = smsDeviceFaultDetail4.Where(s => s.BeginTime.Equals(beginTime));
            }
            IQueryable<SmsDeviceFault> smsDeviceFaultDetail6 = smsDeviceFaultDetail5;
            if (UseTime != null && UseTime != "")
            {
                int useTime = Convert.ToInt32(UseTime);
                smsDeviceFaultDetail6 = smsDeviceFaultDetail5.Where(s => s.UseTime.Equals(useTime));
            }

            var smsDeviceFaultsArray = smsDeviceFaultDetail6.ToArray().OrderBy(s => s.Id).AsEnumerable()
                .Select(s => new
                {
                    s.Id,
                    s.DeviceCode,
                    s.DeviceName,
                    s.DeviceType,
                    //FaultCode = smsAlarmInfoQuery.Where(c=>c.AlarmCode==s.FaultCode).Select(c=>c.AlarmCode),
                    s.FaultCode,
                    BeginTime = s.BeginTime.ToString("yyyy-MM-dd HH:mm:ss"),
                    EndTime = s.EndTime.ToString("yyyy-MM-dd HH:mm:ss"),
                    s.UseTime
                });
            int total = smsDeviceFaultsArray.Count();
            smsDeviceFaultsArray = smsDeviceFaultsArray.Skip((page - 1) * rows).Take(rows);
            return new { total, rows = smsDeviceFaultsArray.ToArray() };
        }

        public DataTable GetSmsDeviceFault(int page, int rows, SmsDeviceFault sdf)
        {
            IQueryable<SmsDeviceFault> smsDeviceFaultQuery = SmsDeviceFaultRepository.GetQueryable();

            var smsDeviceFaultsArray = smsDeviceFaultQuery.OrderBy(s => s.Id).AsEnumerable()
                .Select(s => new
                {
                    s.Id,
                    s.DeviceCode,
                    s.DeviceName,
                    s.DeviceType,
                    s.FaultCode,
                    BeginTime = s.BeginTime.ToString("yyyy-MM-dd HH:mm:ss"),
                    EndTime = s.EndTime.ToString("yyyy-MM-dd HH:mm:ss"),
                    s.UseTime
                });
            System.Data.DataTable dt = new System.Data.DataTable();
            dt.Columns.Add("Id", typeof(int));
            dt.Columns.Add("设备代码", typeof(string));
            dt.Columns.Add("设备名称", typeof(string));
            dt.Columns.Add("设备类型", typeof(string));
            dt.Columns.Add("故障代码", typeof(string));
            dt.Columns.Add("开始时间", typeof(string));
            dt.Columns.Add("结束时间", typeof(string));
            dt.Columns.Add("合计用时", typeof(int));

            foreach (var item in smsDeviceFaultsArray)
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
