using System;
using THOK.SMS.DbModel;
using THOK.SMS.Dal.Interfaces;
using THOK.SMS.Bll.Interfaces;
using System.Linq;
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
        public ISortOrderAllotDetailRepository SortOrderAllotDetailRepository { get; set; }

        [Dependency]
        public ISortOrderAllotDetailService SortOrderAllotDetailService { get; set; }

        [Dependency]
        public ISortingLineRepository SortingLineRepository { get; set; }

        [Dependency]
        public IDeliverLineRepository DeliverLineRepository { get; set; }

        [Dependency]
        public IChannelRepository ChannelRepository { get; set; }
      

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
                sortOrderAllotMasterQuery = sortOrderAllotMasterQuery.Where(c => c.SortBatch.OrderDate.Equals(date));
            }
            if (batchNo != "")
            {
                int batch = Convert.ToInt32(batchNo);
                sortOrderAllotMasterQuery = sortOrderAllotMasterQuery.Where(c => c.SortBatch.BatchNo.Equals(batch));
            }
            if (sortingLineCode != "")
            {
                sortOrderAllotMasterQuery = sortOrderAllotMasterQuery.Where(c => c.SortBatch.SortingLineCode.Equals(sortingLineCode));
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
                c.SortBatch.OrderDate,
                c.SortBatch.BatchNo,
                c.SortBatch.SortingLineCode,
                SortingLineName = sortingLineQuery.Where(s => s.SortingLineCode == c.SortBatch.SortingLineCode).FirstOrDefault().SortingLineName,
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
                Status = c.Status == "01" ? "未完成" : "已完成"
            });
            return new { total, rows = channelAllotArray.ToArray() };
        }

        public DataTable GetSortOrderAllotMaster(int page, int rows, string orderDate, string batchNo, string deliverLineCode, string sortingLineCode)
        {
           
         
            var sortOrderAllotMasterDetailsQuery = SortOrderAllotDetailRepository.GetQueryable();
            var channelQuery=ChannelRepository.GetQueryable();
           if (orderDate != string.Empty && orderDate != null)
            {
                DateTime date = Convert.ToDateTime(orderDate);
                sortOrderAllotMasterDetailsQuery = sortOrderAllotMasterDetailsQuery.Where(c => c.sortOrderAllotMaster.sortBatch.OrderDate.Equals(date));
            }
            if (batchNo != "")
            {
                int batch = Convert.ToInt32(batchNo);
                sortOrderAllotMasterDetailsQuery = sortOrderAllotMasterDetailsQuery.Where(c => c.sortOrderAllotMaster.sortBatch.BatchNo.Equals(batch));
            }
            if (sortingLineCode != "")
            {
                sortOrderAllotMasterDetailsQuery = sortOrderAllotMasterDetailsQuery.Where(c => c.sortOrderAllotMaster.sortBatch.SortingLineCode.Equals(sortingLineCode));
            }
            if (deliverLineCode != "")
            {
                sortOrderAllotMasterDetailsQuery = sortOrderAllotMasterDetailsQuery.Where(c => c.sortOrderAllotMaster.DeliverLineCode.Equals(deliverLineCode));
            }

            var sortOrderAllotMasterArray = sortOrderAllotMasterDetailsQuery.OrderBy(s => s.Id).AsEnumerable()
                .Select(s => new
                {
                    s.Id,
                    s.ProductCode,
                    s.ProductName,
                    s.ChannelCode,
                    ChannelName= channelQuery.FirstOrDefault(a=>a.ChannelCode==s.ChannelCode).ChannelName,
                    s.Quantity                 
                }).ToArray();
            DataTable dt = new DataTable();
            dt.Columns.Add("主单代码", typeof(string));
            dt.Columns.Add("商品编码", typeof(string));
            dt.Columns.Add("商品名称", typeof(string));
            dt.Columns.Add("烟道代码", typeof(string));
            dt.Columns.Add("烟道名称", typeof(string));
            dt.Columns.Add("数量", typeof(string));
            foreach (var item in sortOrderAllotMasterArray)
            {
                dt.Rows.Add(
                    item.Id,
                    item.ProductCode,
                    item.ProductName,
                    item.ChannelCode,
                    item.ChannelName,
                    item.Quantity);              
            }
            return dt;
        }
    }
}
