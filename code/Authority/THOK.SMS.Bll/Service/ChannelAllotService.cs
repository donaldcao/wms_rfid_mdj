using System;
using THOK.SMS.DbModel;
using THOK.SMS.Dal.Interfaces;
using THOK.SMS.Bll.Interfaces;
using System.Linq;
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
        [Dependency]
        public ISortBatchRepository SortBatchRepository { get; set; }
        [Dependency]
        public IChannelRepository ChannelRepository { get; set; }

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
                channelAllotQuery = channelAllotQuery.Where(c => c.SortBatch.OrderDate.Equals(date));
            }
            if (batchNo != "")
            {
                int batch = Convert.ToInt32(batchNo);
                channelAllotQuery = channelAllotQuery.Where(c => c.SortBatch.BatchNo.Equals(batch));
            }
            if (sortingLineCode != "")
            {
                channelAllotQuery = channelAllotQuery.Where(c => c.SortBatch.SortingLineCode.Equals(sortingLineCode));
            }
            if (productCode != "")
            {
                channelAllotQuery = channelAllotQuery.Where(c => c.ProductCode.Equals(productCode));
            }
            var channelAllot = channelAllotQuery.OrderBy(c => c.SortBatchId).ThenBy(c => c.ChannelCode).Select(c => new
            {
                c.SortBatchId,
                c.SortBatch.OrderDate,
                c.SortBatch.BatchNo,
                c.SortBatch.SortingLineCode,
                SortingLineName = sortingLineQuery.Where(s => s.SortingLineCode == c.SortBatch.SortingLineCode).FirstOrDefault().SortingLineName,
                c.ChannelCode,
                c.Channel.ChannelName,
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
            var channelQuery = ChannelRepository.GetQueryable();
            if (orderDate != string.Empty && orderDate != null)
            {
                DateTime date = Convert.ToDateTime(orderDate);
                channelAllotQuery = channelAllotQuery.Where(c => c.SortBatch.OrderDate.Equals(date));
            }
            if (batchNo != "")
            {
                int batch = Convert.ToInt32(batchNo);
                channelAllotQuery = channelAllotQuery.Where(c => c.SortBatch.BatchNo.Equals(batch));
            }
            if (sortingLineCode != "")
            {
                channelAllotQuery = channelAllotQuery.Where(c => c.SortBatch.SortingLineCode.Equals(sortingLineCode));
            }
            if (productCode != "")
            {
                channelAllotQuery = channelAllotQuery.Where(c => c.ProductCode.Equals(productCode));
            }
            var channelAllot = channelAllotQuery.Select(c => new
            {
                c.SortBatchId,
                c.SortBatch.OrderDate,
                c.SortBatch.BatchNo,
                c.SortBatch.SortingLineCode,
                SortingLineName = sortingLineQuery.Where(s => s.SortingLineCode == c.SortBatch.SortingLineCode).FirstOrDefault().SortingLineName,
                c.ChannelCode,
                c.Channel.ChannelName,
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

        public System.Data.DataTable GetChannelAllot(int page, int rows, string orderDate, string batchNo, string sortingLineCode, string productCode, string text)
        {

            var channelAllotQuery = ChannelAllotRepository.GetQueryable();
            var sortingLineQuery = SortingLineRepository.GetQueryable();

            var channelAllotDetail = channelAllotQuery.Where(c => c.ProductCode.Contains(productCode)).OrderBy(c => c.ChannelCode).Select(a => a);

            if (batchNo != null && batchNo != string.Empty)
            {
                int no = Convert.ToInt32(batchNo);
                channelAllotDetail = channelAllotDetail.Where(b => b.SortBatch.BatchNo.Equals(no));
            }
            if (sortingLineCode != "" && sortingLineCode != string.Empty)
            {
                channelAllotDetail = channelAllotDetail.Where(c => c.SortBatch.SortingLineCode.Contains(sortingLineCode));
            }
            if (orderDate != "" && orderDate != string.Empty)
            {
                DateTime date = Convert.ToDateTime(orderDate);
                channelAllotDetail = channelAllotDetail.Where(c => c.SortBatch.OrderDate.Equals(date));
            }

            var sortSupplyDetails = channelAllotDetail.Select(c => new
           {
               OrderDate = c.SortBatch.OrderDate,
               c.SortBatch.BatchNo,
               c.SortBatch.SortingLineCode,
               SortingLineName = sortingLineQuery.Where(s => s.SortingLineCode == c.SortBatch.SortingLineCode).FirstOrDefault().SortingLineName,
               c.ChannelCode,
               c.Channel.ChannelName,
               c.ProductCode,
               c.ProductName,
               Quantity = c.Quantity,
           });
           
            //分拣备货查询（带统计）
            var detalis = sortSupplyDetails.GroupBy(b => new { b.OrderDate, b.BatchNo, b.SortingLineCode, b.SortingLineName, b.ProductCode, b.ProductName }).Select(c => new
           {
               c.Key.OrderDate,
               c.Key.BatchNo,
               c.Key.SortingLineCode,
               c.Key.SortingLineName,
               c.Key.ProductCode,
               c.Key.ProductName,
               Quantity = c.Sum(g => g.Quantity)
           });

            System.Data.DataTable dt = new System.Data.DataTable();

            switch (text)
            {

                case "分拣备货":

                    dt.Columns.Add("订单日期", typeof(string));
                    dt.Columns.Add("批次号", typeof(string));
                    dt.Columns.Add("分拣线", typeof(string));
                    dt.Columns.Add("商品代码", typeof(string));
                    dt.Columns.Add("商品名称", typeof(string));
                    dt.Columns.Add("商品数量", typeof(string));

                    foreach (var item in detalis)
                    {
                        dt.Rows.Add
                            (
                                item.OrderDate,
                                item.BatchNo,
                                item.SortingLineName,
                                item.ProductCode,
                                item.ProductName,
                                item.Quantity

                            );
                    }
                    break;
                case "分拣烟道":

                    dt.Columns.Add("订单日期", typeof(string));
                    dt.Columns.Add("批次号", typeof(string));
                    dt.Columns.Add("分拣线", typeof(string));
                    dt.Columns.Add("烟道代码", typeof(string));
                    dt.Columns.Add("烟道名称", typeof(string));
                    dt.Columns.Add("商品代码", typeof(string));
                    dt.Columns.Add("商品名称", typeof(string));
                    dt.Columns.Add("商品数量", typeof(string));

                    foreach (var item in sortSupplyDetails)
                    {
                        dt.Rows.Add
                            (
                                item.OrderDate,
                                item.BatchNo,
                                item.SortingLineName,
                                item.ChannelCode,
                                item.ChannelName,
                                item.ProductCode,
                                item.ProductName,
                                item.Quantity

                            );
                    }
                    break;
            }
            return dt;
        }
    }
}
