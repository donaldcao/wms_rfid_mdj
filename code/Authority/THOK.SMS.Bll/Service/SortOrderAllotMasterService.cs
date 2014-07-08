using System;
using THOK.SMS.DbModel;
using THOK.SMS.Dal.Interfaces;
using THOK.SMS.Bll.Interfaces;
using System.Linq;
using THOK.Wms.SignalR.Common;
using Microsoft.Practices.Unity;
using THOK.Common.Entity;
using System.Data;
using THOK.Wms.Dal.Interfaces;

namespace THOK.SMS.Bll.Service
{
    public class SortOrderAllotMasterService : ServiceBase<SortOrderAllotMaster>, ISortOrderAllotMasterService
    {
        [Dependency]
        public ISortOrderAllotMasterRepository SortOrderAllotMasterRepository { get; set; }

        [Dependency]
        public ISortOrderAllotDetailService SortOrderAllotDetailService { get; set; }

        [Dependency]
        public ISortingLineRepository SortingLineRepository { get; set; }

        [Dependency]
        public IDeliverLineRepository DeliverLineRepository { get; set; }

        protected override Type LogPrefix
        {
            get { return this.GetType(); }
        }


        public object GetDetails(int page, int rows, string orderDate, string batchNo, string sortingLineCode, string deliverLineCode, string customerCode, string status)
        {
            var sortOrderAllotMasterQuery = SortOrderAllotMasterRepository.GetQueryable();
            var sortingLineQuery = SortingLineRepository.GetQueryable();
            var DeliverLineQuery = DeliverLineRepository.GetQueryable();
            if (orderDate != string.Empty && orderDate != null)
            {
                DateTime date = Convert.ToDateTime(orderDate);
                sortOrderAllotMasterQuery = sortOrderAllotMasterQuery.Where(c => c.sortBatch.OrderDate.Equals(date));
            }
            if (batchNo != "")
            {
                int batch = Convert.ToInt32(batchNo);
                sortOrderAllotMasterQuery = sortOrderAllotMasterQuery.Where(c => c.sortBatch.BatchNo.Equals(batch));
            }
            if (sortingLineCode != "")
            {
                sortOrderAllotMasterQuery = sortOrderAllotMasterQuery.Where(c => c.sortBatch.SortingLineCode.Equals(sortingLineCode));
            }
            if (deliverLineCode != "")
            {
                sortOrderAllotMasterQuery = sortOrderAllotMasterQuery.Where(c => c.DeliverLineCode.Equals(deliverLineCode));
            }
            if (customerCode != "")
            {
                sortOrderAllotMasterQuery = sortOrderAllotMasterQuery.Where(c => c.CustomerCode.Equals(customerCode));
            }
            if (status != "")
            {
                sortOrderAllotMasterQuery = sortOrderAllotMasterQuery.Where(c => c.Status.Equals(status));
            }
            var sortOrderAllotMaster = sortOrderAllotMasterQuery.OrderBy(c => c.SortBatchId).ThenBy(c => c.PackNo).Select(c => new
            {
                c.Id,
                c.PackNo,
                c.OrderId,
                c.SortBatchId,
                c.sortBatch.OrderDate,
                c.sortBatch.BatchNo,
                c.sortBatch.SortingLineCode,
                SortingLineName = sortingLineQuery.Where(s => s.SortingLineCode == c.sortBatch.SortingLineCode).FirstOrDefault().SortingLineName,
                c.DeliverLineCode,
                DeliverLineName = DeliverLineQuery.Where(s => s.DeliverLineCode == c.DeliverLineCode).FirstOrDefault().DeliverLineName,
                c.CustomerCode,
                c.CustomerName,
                c.CustomerOrder,
                c.CustomerDeliverOrder,
                c.Quantity,
                c.Status

            });
            int total = sortOrderAllotMaster.Count();
            var channelAllotDetail = sortOrderAllotMaster.Skip((page - 1) * rows).Take(rows);
            var channelAllotArray = channelAllotDetail.ToArray().Select(c => new
            {
                c.Id,
                c.PackNo,
                c.OrderId,
                c.SortBatchId,
                OrderDate = c.OrderDate.ToString("yyyy-MM-dd"),
                c.BatchNo,
                c.SortingLineCode,
                c.SortingLineName,
                c.DeliverLineCode,
                c.DeliverLineName,
                c.CustomerCode,
                c.CustomerName,
                c.CustomerOrder,
                c.CustomerDeliverOrder,
                c.Quantity,
                Status = c.Status == "01" ? "已完成" : "未完成"
            });
            return new { total, rows = channelAllotArray.ToArray() };
        }

        public DataTable GetSortOrderAllotMaster(int page,int rows,string batchNo, string orderId, string status)
        {
            //IQueryable<SortOrderAllotMaster> sortOrderAllotMasterQuery = SortOrderAllotMasterRepository.GetQueryable();
            //var sortOrderAllotMasterDetails = sortOrderAllotMasterQuery.Where(s => s.OrderId.Contains(orderId) && s.Status.Contains(status));
            //if (batchNo != "")
            //{
            //    int no;
            //    Int32.TryParse(batchNo, out no);
            //    if (no != 0)
            //    {
            //        sortOrderAllotMasterDetails = sortOrderAllotMasterDetails.Where(s => s.sortBatch.batch.BatchNo == no);
            //    }
            //}
            //int total = sortOrderAllotMasterDetails.Count();
            //var sortOrderAllotMasterArray = sortOrderAllotMasterDetails.OrderBy(s => s.PackNo).Skip((page - 1) * rows).Take(rows)
            //    .Select(s => new
            //    {
            //        s.OrderMasterCode,
            //        s.SortBatchId,
            //        OrderDate = s.sortBatch.batch.OrderDate,
            //        BatchNo = s.sortBatch.batch.BatchNo,
            //        s.PackNo,
            //        s.OrderId,
            //        s.CustomerOrder,
            //        s.CustomerDeliverOrder,
            //        s.Quantity,
            //        s.ExportNo,
            //        StartTime = s.StartTime,
            //        FinishTime = s.FinishTime,
            //        Status = s.Status == "01" ? "已分配" : s.Status == "02" ? "已中止" : s.Status == "03" ? "已完成" : "已结单"
            //    }).ToArray();
            DataTable dt = new DataTable();
            dt.Columns.Add("主单代码", typeof(string));
            dt.Columns.Add("订单日期", typeof(string));
            dt.Columns.Add("批次号", typeof(string));
            dt.Columns.Add("烟包包号", typeof(string));
            dt.Columns.Add("订单编码", typeof(string));
            dt.Columns.Add("客户顺序", typeof(string));
            dt.Columns.Add("配送顺序", typeof(string));
            dt.Columns.Add("烟包数量", typeof(string));
            dt.Columns.Add("开始时间", typeof(string));
            dt.Columns.Add("完成时间", typeof(string));
            dt.Columns.Add("任务状态", typeof(string));
            //foreach (var item in sortOrderAllotMasterArray)
            //{
            //    dt.Rows.Add(
            //        item.OrderMasterCode,
            //        item.OrderDate.ToString("yyyy-MM-dd"),
            //        item.BatchNo,
            //        item.PackNo,
            //        item.OrderId,
            //        item.CustomerOrder,
            //        item.CustomerDeliverOrder,
            //        item.Quantity,
            //        item.StartTime.ToString("yyyy-MM-dd"),
            //        item.FinishTime.ToString("yyyy-MM-dd"),
            //        item.Status);
            //    dt.Rows.Add("","","","细单代码","主单代码","商品编码","商品名称","烟道代码","数量","","");
            //    var sortOrderAllotDetail = SortOrderAllotDetailService.GetDetailsByOrderMasterCode(item.OrderMasterCode);
            //    foreach (DataRow ss in sortOrderAllotDetail.Rows)
            //    {
            //        dt.ImportRow(ss);
            //    }
            //}
            return dt;
        }
    }
}
