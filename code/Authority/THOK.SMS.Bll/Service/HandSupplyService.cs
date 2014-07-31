using System;
using THOK.SMS.DbModel;
using THOK.SMS.Dal.Interfaces;
using THOK.SMS.Bll.Interfaces;
using System.Linq;
using Microsoft.Practices.Unity;
using THOK.Common.Entity;
using System.Data;

namespace THOK.SMS.Bll.Service
{
    public class HandSupplyService : ServiceBase<HandSupply>, IHandSupplyService
    {
        [Dependency]
        public IHandSupplyRepository SortSupplyRepository { get; set; }

        protected override Type LogPrefix
        {
            get { return this.GetType(); }
        }


        public object GetDetails(int page, int rows, HandSupply sortSupply)
        {
            return null;
            //IQueryable<HandSupply> sortSupplyQuery = SortSupplyRepository.GetQueryable();

            //var sortSupplyDetails = sortSupplyQuery.Where(d =>
            //    d.SortSupplyCode.Contains(sortSupply.SortSupplyCode)).OrderBy(ul => ul.SortSupplyCode);

            //int total = sortSupplyDetails.Count();
            //var sortSupplyDetail = sortSupplyDetails.Skip((page - 1) * rows).Take(rows);
            //var sortSupplyArray = sortSupplyDetail.ToArray().Select(s => new
            //{
            //    s.SortSupplyCode,
            //    s.SortBatchId,
            //    s.ChannelCode,
            //    s.SupplyId,
            //    s.PackNo,
            //    s.ProductCode,
            //    s.ProductName
            //});
            //return new { total, rows = sortSupplyArray.ToArray() };
        }

        public System.Data.DataTable GetSortSupply(int page, int rows, HandSupply sortSupply)
        {
            //IQueryable<HandSupply> sortSupplyQuery = SortSupplyRepository.GetQueryable();

            //var sortSupplyDetail = sortSupplyQuery.Where(s =>
            //    s.SortSupplyCode.Contains(sortSupply.SortSupplyCode)
            //        //&& d.SortBatchId.Contains(deliverLineAllot.SortBatchId)
            //    && s.ChannelCode.Contains(sortSupply.ChannelCode)
            //        //&& d.DeliverQuantity.Contains(deliverLineAllot.DeliverQuantity)
            //    && s.ProductCode.Contains(sortSupply.ProductCode))
            //        .OrderBy(ul => ul.SortSupplyCode);
            //var sortSupplyDetails = sortSupplyDetail.ToArray().Select(s => new
            //{
            //    s.SortSupplyCode,
            //    s.SortBatchId,
            //    s.ChannelCode,
            //    s.SupplyId,
            //    s.PackNo,
            //    s.ProductCode,
            //    s.ProductName
            //});

            System.Data.DataTable dt = new System.Data.DataTable();

            dt.Columns.Add("订单细单代码", typeof(string));
            dt.Columns.Add("批次分拣编号", typeof(string));
            dt.Columns.Add("补货编码", typeof(string));
            dt.Columns.Add("烟包包号", typeof(string));
            dt.Columns.Add("烟道代码", typeof(string));
            dt.Columns.Add("商品代码", typeof(string));
            dt.Columns.Add("商品名称", typeof(string));

            //foreach (var item in sortSupplyDetails)
            //{
            //    dt.Rows.Add
            //        (
            //            item.SortSupplyCode,
            //            item.SortBatchId,
            //            item.ChannelCode,
            //            item.SupplyId,
            //            item.PackNo,
            //            item.ProductCode,
            //            item.ProductName
            //        );
            //}
            return dt;
        }

    }
}
