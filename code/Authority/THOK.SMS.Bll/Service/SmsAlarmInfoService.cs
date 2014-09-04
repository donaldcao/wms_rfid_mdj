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
    public class SmsAlarmInfoService : ServiceBase<SmsAlarmInfo>, ISmsAlarmInfoService
    {
        [Dependency]
        public ISmsAlarmInfoRepository SmsAlarmInfoRepository { get; set; }

        protected override Type LogPrefix
        {
            get { return this.GetType(); }
        }

        public object GetDetails(int page, int rows, string AlarmCode, string Description)
        {
            IQueryable<SmsAlarmInfo> smsAlarmInfoQuery = SmsAlarmInfoRepository.GetQueryable();
            IQueryable<SmsAlarmInfo> smsAlarmInfoDetail1 = smsAlarmInfoQuery;
            if (AlarmCode != "" )
            {
                smsAlarmInfoDetail1 = smsAlarmInfoQuery.Where(s => s.AlarmCode == AlarmCode);
            }
            IQueryable<SmsAlarmInfo> smsAlarmInfoDetail2 = smsAlarmInfoDetail1;
            if (Description!="")
            {
                smsAlarmInfoDetail2 = smsAlarmInfoDetail1.Where(s => s.Description == Description);
            }

            var v1 = smsAlarmInfoDetail2.OrderBy(a => a.AlarmCode).Select(t => t);
            int total = v1.Count();
            v1 = v1.Skip((page - 1) * rows).Take(rows);

            var smsAlarmInfosArray = v1.ToArray().Select(s => new
                {
                    s.AlarmCode,
                    s.Description,
                });
            return new { total, rows = smsAlarmInfosArray };
        }

        public bool Add(SmsAlarmInfo alarmInfo, out string strResult)
        {
            strResult = string.Empty;
            bool result = false;
            var al = SmsAlarmInfoRepository.GetQueryable().FirstOrDefault(a => a.AlarmCode == alarmInfo.AlarmCode);
            if (al == null)
            {
                SmsAlarmInfo alarm = new SmsAlarmInfo();
                try
                {
                    alarm.AlarmCode = alarmInfo.AlarmCode;
                    alarm.Description = alarmInfo.Description;
                    SmsAlarmInfoRepository.Add(alarm);
                    SmsAlarmInfoRepository.SaveChanges();
                    result = true;
                }
                catch (Exception e)
                {
                    strResult = "原因：" + e.Message;
                }
            }
            else
            {
                strResult = "原因：报警编码已存在";
            }
            return result;
        }

        public bool Save(SmsAlarmInfo alarmInfo, out string strResult)
        {
            strResult = string.Empty;
            bool result = false;
            var alarmInfos = SmsAlarmInfoRepository.GetQueryable().FirstOrDefault(s => s.AlarmCode == alarmInfo.AlarmCode);
            if (alarmInfos != null)
            {
                alarmInfos.Description = alarmInfo.Description;
                SmsAlarmInfoRepository.SaveChanges();
                result = true;
            }
            else
            {
                strResult = "原因：找不到相应数据";
            }
            return result;
        }

        public bool Delete(string code, out string strResult)
        {
            strResult = string.Empty;
            bool result = false;
            var alarmInfo = SmsAlarmInfoRepository.GetQueryable().FirstOrDefault(a => a.AlarmCode == code);
            if (alarmInfo != null)
            {
                SmsAlarmInfoRepository.Delete(alarmInfo);
                SmsAlarmInfoRepository.SaveChanges();
                result = true;
            }
            else
            {
                strResult = "原因：没有找到相应数据";
            }
            return result;
        }

        public System.Data.DataTable GetAlarmInfo(int page, int rows, SmsAlarmInfo alarmInfo)
        {
            IQueryable<SmsAlarmInfo> alarmInfoQuery = SmsAlarmInfoRepository.GetQueryable();

            var alarmInfoDetail = alarmInfoQuery.Where(a =>
                a.AlarmCode.Contains(alarmInfo.AlarmCode)
                && a.Description.Contains(alarmInfo.Description))
                .OrderBy(a => a.AlarmCode);
            var alarmInfo_Detail = alarmInfoDetail.ToArray().Select(a => new
            {
                a.AlarmCode,
                a.Description
            });

            System.Data.DataTable dt = new System.Data.DataTable();

            dt.Columns.Add("报警编码", typeof(string));
            dt.Columns.Add("描述", typeof(string));
            foreach (var s in alarmInfo_Detail)
            {
                dt.Rows.Add
                    (
                        s.AlarmCode,
                        s.Description
                    );
            }
            return dt;
        }
    }
}
