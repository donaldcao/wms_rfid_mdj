using System;
using THOK.SMS.DbModel;
using THOK.SMS.Dal.Interfaces;
using THOK.SMS.Bll.Interfaces;
using System.Linq;
using THOK.Wms.SignalR.Common;
using Microsoft.Practices.Unity;
using THOK.Common.Entity;
using System.Data;

namespace THOK.SMS.Bll.Service
{
    public class DeliverLineAllotService : ServiceBase<DeliverLineAllot>, IDeliverLineAllotService
    {
        [Dependency]
        public IDeliverLineAllotRepository DeliverLineAllotRepository { get; set; }

        protected override Type LogPrefix
        {
            get { return this.GetType(); }
        }

        public object GetDetails(int page, int rows, DeliverLineAllot deliverLineAllot)
        {
            IQueryable<DeliverLineAllot> deliverLineAllotQuery = DeliverLineAllotRepository.GetQueryable();

            var deliverLineAllotDetails = deliverLineAllotQuery.Where(d =>
                d.DeliverLineCode.Contains(deliverLineAllot.DeliverLineAllotCode)
                && d.Status.Contains(deliverLineAllot.Status)).OrderBy(ul=>ul.DeliverLineAllotCode);

            int total = deliverLineAllotDetails.Count();
            var deliverLineAllotDetail = deliverLineAllotDetails.Skip((page - 1) * rows).Take(rows);
            var deliverLineAllotArray = deliverLineAllotDetail.ToArray().Select(d => new
            {
                d.DeliverLineAllotCode,
                d.BatchSortId,
                d.DeliverLineCode,
                d.DeliverQuantity,
                Status = d.Status == "01" ? "已分配" : d.Status == "02" ? "已中止" : d.Status == "03" ? "已完成" : "已结单"

            });
            return new { total, rows = deliverLineAllotArray.ToArray() };
        }

        public System.Data.DataTable GetDeliverLineAllot(int page, int rows, DeliverLineAllot deliverLineAllot)
        {
            IQueryable<DeliverLineAllot> deliverLineAllotQuery = DeliverLineAllotRepository.GetQueryable();

            var deliverLineAllotDetail = deliverLineAllotQuery.Where(d =>
                d.DeliverLineAllotCode.Contains(deliverLineAllot.DeliverLineAllotCode)
                    //&& d.BatchSortId.Contains(deliverLineAllot.BatchSortId)
                && d.DeliverLineCode.Contains(deliverLineAllot.DeliverLineCode)
                    //&& d.DeliverQuantity.Contains(deliverLineAllot.DeliverQuantity)
                && d.Status.Contains(deliverLineAllot.Status))
                    .OrderBy(ul => ul.DeliverLineAllotCode);
            var deliverLineAllotDetails = deliverLineAllotDetail.ToArray().Select(d => new
            {
                d.DeliverLineAllotCode,
                d.BatchSortId,
                d.DeliverLineCode,
                d.DeliverQuantity,
                Status = d.Status == "01" ? "已分配" : d.Status == "02" ? "已中止" : d.Status == "03" ? "已完成" : "已结单"

            });

            System.Data.DataTable dt = new System.Data.DataTable();

            dt.Columns.Add("分拣线分配代码", typeof(string));
            dt.Columns.Add("批次分拣编号", typeof(string));
            dt.Columns.Add("送货线路编码", typeof(string));
            dt.Columns.Add("配送数量", typeof(string));
            dt.Columns.Add("任务状态", typeof(string));

            foreach (var item in deliverLineAllotDetails)
            {
                dt.Rows.Add
                    (
                        item.DeliverLineAllotCode,
                        item.BatchSortId,
                        item.DeliverLineCode,
                        item.DeliverQuantity,
                        item.Status
                    );
            }
            return dt;
 
        }
    }
}
