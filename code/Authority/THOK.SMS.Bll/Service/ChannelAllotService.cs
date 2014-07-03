using System;
using THOK.SMS.DbModel;
using THOK.SMS.Dal.Interfaces;
using THOK.SMS.Bll.Interfaces;
using System.Linq;
using THOK.Wms.SignalR.Common;
using Microsoft.Practices.Unity;
using THOK.Common.Entity;
using THOK.Wms.Dal.Interfaces;

namespace THOK.SMS.Bll.Service
{
    public class ChannelAllotService : ServiceBase<ChannelAllot>, IChannelAllotService
    {
        [Dependency]
        public IChannelAllotRepository ChannelAllotRepository { get; set; }
        [Dependency]
        public ISortingLineRepository SortingLineRepository { get; set; }

        protected override Type LogPrefix
        {
            get { return this.GetType(); }
        }

        public object GetDetails(int page, int rows, string orderDate, string batchNo, string sortingLineCode, string productCode)
        {
            var channelAllotQuery = ChannelAllotRepository.GetQueryable();
            var sortingLineQuery = SortingLineRepository.GetQueryable();
            if (orderDate != string.Empty && orderDate != null)
            {
                DateTime date = Convert.ToDateTime(orderDate);
                channelAllotQuery = channelAllotQuery.Where(c => c.sortBatch.OrderDate.Equals(date));
            }
            if (batchNo != "")
            {
                int batch = Convert.ToInt32(batchNo);
                channelAllotQuery = channelAllotQuery.Where(c => c.sortBatch.BatchNo.Equals(batch));
            }
            if (sortingLineCode != "")
            {
                channelAllotQuery = channelAllotQuery.Where(c => c.sortBatch.SortingLineCode.Equals(sortingLineCode));
            }
            if (productCode != "")
            {
                channelAllotQuery = channelAllotQuery.Where(c => c.ProductCode.Equals(productCode));
            }
            var channelAllot = channelAllotQuery.OrderBy(c => c.SortBatchId).ThenBy(c => c.ChannelCode).Select(c => new 
            {
                c.SortBatchId,
                c.sortBatch.OrderDate,
                c.sortBatch.BatchNo,
                c.sortBatch.SortingLineCode,
                SortingLineName = sortingLineQuery.Where(s => s.SortingLineCode == c.sortBatch.SortingLineCode).FirstOrDefault().SortingLineName,
                c.ChannelCode,
                c.channel.ChannelName,
                c.ProductCode,
                c.ProductName,
                c.Quantity

            });
            int total = channelAllot.Count();
            var channelAllotDetail = channelAllot.Skip((page - 1) * rows).Take(rows);
            var channelAllotArray = channelAllotDetail.ToArray().Select(c => new
            {
                c.SortBatchId,
                OrderDate = c.OrderDate.ToString("yyyy-MM-dd"),
                c.BatchNo,
                c.SortingLineCode,
                c.SortingLineName,
                c.ChannelCode,
                c.ChannelName,
                c.ProductCode,
                c.ProductName,
                c.Quantity
            });
            return new { total, rows = channelAllotArray.ToArray() };
        }

        public object Details(int page, int rows, string orderDate, string batchNo, string sortingLineCode, string productCode)
        {
            var channelAllotQuery = ChannelAllotRepository.GetQueryable();
            var sortingLineQuery = SortingLineRepository.GetQueryable();
            if (orderDate != string.Empty && orderDate != null)
            {
                DateTime date = Convert.ToDateTime(orderDate);
                channelAllotQuery = channelAllotQuery.Where(c => c.sortBatch.OrderDate.Equals(date));
            }
            if (batchNo != "")
            {
                int batch = Convert.ToInt32(batchNo);
                channelAllotQuery = channelAllotQuery.Where(c => c.sortBatch.BatchNo.Equals(batch));
            }
            if (sortingLineCode != "")
            {
                channelAllotQuery = channelAllotQuery.Where(c => c.sortBatch.SortingLineCode.Equals(sortingLineCode));
            }
            if (productCode != "")
            {
                channelAllotQuery = channelAllotQuery.Where(c => c.ProductCode.Equals(productCode));
            }
            var channelAllot = channelAllotQuery.Select(c => new
            {
                c.SortBatchId,
                c.sortBatch.OrderDate,
                c.sortBatch.BatchNo,
                c.sortBatch.SortingLineCode,
                SortingLineName = sortingLineQuery.Where(s => s.SortingLineCode == c.sortBatch.SortingLineCode).FirstOrDefault().SortingLineName,
                c.ChannelCode,
                c.channel.ChannelName,
                c.ProductCode,
                c.ProductName,
                c.Quantity

            }).GroupBy(c => new { c.OrderDate, c.BatchNo, c.SortingLineCode, c.SortingLineName, c.ProductCode, c.ProductName }).Select(c => new
            {
                c.Key.OrderDate,
                c.Key.BatchNo,
                c.Key.SortingLineCode,
                c.Key.SortingLineName,
                c.Key.ProductCode,
                c.Key.ProductName,
                Quantity = c.Sum(g => g.Quantity)
            }).OrderByDescending(c => c.OrderDate).ThenByDescending(c => c.BatchNo).ThenBy(c => c.SortingLineCode).ThenByDescending(c => c.Quantity);
            int total = channelAllot.Count();
            var channelAllotDetail = channelAllot.Skip((page - 1) * rows).Take(rows);
            var channelAllotArray = channelAllotDetail.ToArray().Select(c => new
            {
                OrderDate = c.OrderDate.ToString("yyyy-MM-dd"),
                c.BatchNo,
                c.SortingLineCode,
                c.SortingLineName,
                c.ProductCode,
                c.ProductName,
                c.Quantity
            });
            return new { total, rows = channelAllotArray.ToArray() };
        }

        public System.Data.DataTable GetChannelAllot(int page, int rows, ChannelAllot channelAllot)
        {
            //IQueryable<ChannelAllot> channelAllotQuery = ChannelAllotRepository.GetQueryable();

            //var channelAllotDetail = channelAllotQuery.Where(c =>
            //    c.ChannelAllotCode.Contains(channelAllot.ChannelAllotCode)
            //    && c.ChannelCode.Contains(channelAllot.ChannelCode)
            //    && c.ProductCode.Contains(channelAllot.ProductCode))
            //        .OrderBy(ul => ul.ChannelAllotCode);
            //var sortSupplyDetails = channelAllotDetail.ToArray().Select(c => new
            //{
            //    c.ChannelAllotCode,
            //    c.SortBatchId,
            //    c.ChannelCode,
            //    c.ProductCode,
            //    c.ProductName,
            //    c.InQuantity,
            //    c.OutQuantity,
            //    c.RealQuantity,
            //    c.RemainQuantity
            //});

            System.Data.DataTable dt = new System.Data.DataTable();

            //dt.Columns.Add("烟道分配代码", typeof(string));
            //dt.Columns.Add("批次分拣编号", typeof(string));
            //dt.Columns.Add("烟道代码", typeof(string));
            //dt.Columns.Add("商品代码", typeof(string));
            //dt.Columns.Add("商品名称", typeof(string));
            //dt.Columns.Add("入库数量", typeof(string));
            //dt.Columns.Add("出库数量", typeof(string));
            //dt.Columns.Add("实际数量", typeof(string));
            //dt.Columns.Add("提前量", typeof(string));

            //foreach (var item in sortSupplyDetails)
            //{
            //    dt.Rows.Add
            //        (
            //            item.ChannelAllotCode,
            //            item.SortBatchId,
            //            item.ChannelCode,
            //            item.ProductCode,
            //            item.ProductName,
            //            item.InQuantity,
            //            item.OutQuantity,
            //            item.RealQuantity,
            //            item.RemainQuantity
            //        );
            //}
            return dt;
        }

    }
}
