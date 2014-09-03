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
    }
}
