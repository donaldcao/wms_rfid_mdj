using System;
using THOK.SMS.DbModel;
using THOK.SMS.Dal.Interfaces;
using THOK.SMS.Bll.Interfaces;
using System.Linq;
using THOK.Wms.SignalR.Common;
using Microsoft.Practices.Unity;
using THOK.Common.Entity;

using THOK.Wms.DbModel;
using THOK.Wms.Dal.Interfaces;

namespace THOK.SMS.Bll.Service
{
    public class SortBatchService : ServiceBase<SortBatch>, ISortBatchService
    {
        [Dependency]
        public ISortBatchRepository SortBatchRepository { get; set; }

        [Dependency]
        public ISortingLineRepository SortingLineRepository { get; set; }

        [Dependency]
        public ISortOrderDispatchRepository SortOrderDispatchRepository { get; set; }

        [Dependency]
        public IDeliverLineRepository DeliverLineRepository { get; set; }

        [Dependency]
        public IDeliverDistRepository DeliverDistRepository { get; set; }

        protected override Type LogPrefix
        {
            get { return this.GetType(); }
        }

        public object GetDetails(int page, int rows, SortBatch sortBatch, string sortingLineName, string sortingLineType)
        {
            var sortBatchQuery = SortBatchRepository.GetQueryable();
            var sortingLineQuery = SortingLineRepository.GetQueryable();
            var sortBatchDetials = sortBatchQuery.Join(sortingLineQuery,batch => batch.SortingLineCode,line => line.SortingLineCode,
                (batch, line) => new { batch.Id,batch.OrderDate,batch.BatchNo,batch.SortingLineCode,batch.NoOneBatchNo,batch.SortDate,batch.Status,line.SortingLineName,line.SortingLineType})
                .Where(a => a.SortingLineCode.Contains(sortBatch.SortingLineCode) && a.Status.Contains(sortBatch.Status)&&a.SortingLineName.Contains(sortingLineName)&&a.SortingLineType.Contains(sortingLineType));
            if (sortBatch.OrderDate.CompareTo(Convert.ToDateTime("1900-01-01")) > 0)
            {
                sortBatchDetials = sortBatchDetials.Where(a => a.OrderDate.Equals(sortBatch.OrderDate));
            }
            if (sortBatch.BatchNo > 0)
            {
                sortBatchDetials = sortBatchDetials.Where(a => a.BatchNo.Equals(sortBatch.BatchNo));
            }
            int total = sortBatchDetials.Count();
            sortBatchDetials = sortBatchDetials.OrderBy(a=>a.Id).Skip((page - 1) * rows).Take(rows);
            var sortBatchArray = sortBatchDetials.AsEnumerable().Select(a => new
            {
                a.Id,
                OrderDate=a.OrderDate.ToShortDateString(),
                a.BatchNo,
                a.SortingLineCode,
                a.SortingLineName,
                a.SortingLineType,
                a.NoOneBatchNo,
                SortDate=a.SortDate.ToShortDateString(),
                Status = a.Status == "01" ? "未优化" : a.Status == "02" ? "已优化" : a.Status == "03" ? "已上传" : a.Status == "04" ? "已下载" : a.Status == "05" ? "已挂起" : "已完成"
            }).ToArray();
            return new { total, rows = sortBatchArray.ToArray() };
        }

        public bool Add(string dispatchId, out string strResult)
        {
            strResult = string.Empty;
            bool result = false;
            var sortOrderDispatchQuery = SortOrderDispatchRepository.GetQueryable();
            var dispatchIds = dispatchId.Substring(0, dispatchId.Length - 1).Split(',');
            var sortOrderDispatchArray = sortOrderDispatchQuery.Where(a => dispatchIds.Contains(a.ID.ToString()))
                .GroupBy(a => a.OrderDate)
                .Select(a => a.Key).ToArray();
            var sortBatchQuery = SortBatchRepository.GetQueryable();
            var sortingLineQuery = SortingLineRepository.GetQueryable();
            foreach (var orderDate in sortOrderDispatchArray)
            {
                try
                {
                    DateTime date = DateTime.ParseExact(orderDate, "yyyyMMdd", System.Globalization.CultureInfo.CurrentCulture);
                    var sortingLineDetail = sortingLineQuery.Where(a => a.IsActive.Equals("1")
                        && !sortBatchQuery.Where(b => b.OrderDate.Equals(date) && b.Status.Equals("01")).Select(b => b.SortingLineCode).Contains(a.SortingLineCode))
                        .Select(a => a.SortingLineCode).ToArray();
                    //更新批次分拣表
                    foreach (var item in sortingLineDetail)
                    {
                        SortBatch sortBatch = new SortBatch();
                        sortBatch.OrderDate = date;
                        var batchNo = sortBatchQuery.Where(a => a.OrderDate.Equals(date) && a.SortingLineCode.Equals(item)).Select(a => new { a.BatchNo, a.Status, a.NoOneBatchNo }).OrderByDescending(a => a.BatchNo).ToArray();
                        if (batchNo.Count() > 0)
                        {
                            if (batchNo[0].Status == "01")
                                continue;
                            else
                            {
                                sortBatch.BatchNo = Convert.ToInt32(batchNo[0].BatchNo) + 1;
                                sortBatch.NoOneBatchNo = Convert.ToInt32(batchNo[0].NoOneBatchNo) + 1;
                            }
                        }
                        else
                        {
                            sortBatch.BatchNo = 1;
                            sortBatch.NoOneBatchNo = 1;
                        }
                        sortBatch.SortingLineCode = item;
                        sortBatch.SortDate = DateTime.Today.AddDays(1);
                        sortBatch.Status = "01";
                        SortBatchRepository.Add(sortBatch);
                    }
                    SortBatchRepository.SaveChanges();
                    //更新分拣调度表
                    var deliverDistQuery = DeliverDistRepository.GetQueryable();
                    var deliverLineQuery = DeliverLineRepository.GetQueryable();
                    foreach (var item in dispatchIds)
                    {
                        int id = Convert.ToInt32(item);
                        var sortOrderDispatch = sortOrderDispatchQuery.FirstOrDefault(a => a.ID.Equals(id));
                        if (sortOrderDispatch == null)
                        {
                            continue;
                        }
                        sortOrderDispatch.BatchSortId = sortBatchQuery.FirstOrDefault(a => a.SortingLineCode.Equals(sortOrderDispatch.SortingLineCode) && a.OrderDate.Equals(date)).Id;
                    }
                    SortOrderDispatchRepository.SaveChanges();
                    foreach (var sortingLineCode in sortingLineQuery.Where(a => a.ProductType.Equals("1")).Select(a => a.SortingLineCode))
                    {
                        var sortOrderDispatchDetail = sortOrderDispatchQuery.Where(a => a.OrderDate.Equals(orderDate) && a.SortStatus.Equals("1") && a.SortingLineCode.Equals(sortingLineCode)&&a.BatchSortId>0)
                            .Join(deliverLineQuery, dis => dis.DeliverLineCode, line => line.DeliverLineCode,
                            (dis, line) => new { dis.ID, DeliverLineOrder = line.DeliverOrder, line.DistCode })
                            .Join(deliverDistQuery, a => a.DistCode, dist => dist.DistCode,
                            (a, dist) => new { a.ID, a.DeliverLineOrder, DeliverDistOrder = dist.DeliverOrder })
                            .OrderBy(a => new { a.DeliverDistOrder, a.DeliverLineOrder })
                            .Select(a => new { a.ID, a.DeliverLineOrder,a.DeliverDistOrder}).ToArray();
                        for (int i = 0; i < sortOrderDispatchDetail.Count(); i++)
                        {
                            int id = sortOrderDispatchDetail[i].ID;
                            var sortOrderDispatch = sortOrderDispatchQuery.FirstOrDefault(a => a.ID.Equals(id));
                            if (sortOrderDispatch != null)
                            {
                                sortOrderDispatch.DeliverLineNo = i + 1;
                            }
                        }
                        
                    }
                    SortOrderDispatchRepository.SaveChanges();
                    result = true;
                }
                catch (Exception e)
                {
                    strResult = "原因：" + e.Message;
                }
            }
            return result;
        }

        public bool Save(SortBatch SortBatch, out string strResult)
        {

            strResult = string.Empty;
            bool result = false;
            //var SortBatchs = SortBatchRepository.GetQueryable().FirstOrDefault(a => a.SortBatchId == SortBatch.SortBatchId);
            //if (SortBatchs != null)
            //{
            //    //SortBatchs.BatchId = SortBatch.BatchId;
            //    SortBatchs.SortingLineCode = SortBatch.SortingLineCode;
            //    SortBatchs.Status = SortBatch.Status;
            //    SortBatchRepository.SaveChanges();
            //    result = true;
            //}
            //else
            //{
            //    strResult = "原因:找不到相应数据";
            //}
            return result;
        }

        public bool Delete(int SortBatchId, out string strResult)
        {

            strResult = string.Empty;
            bool result = false;
            //var SortBatch = SortBatchRepository.GetQueryable().FirstOrDefault(a => a.SortBatchId.Equals(SortBatchId));
            //if (SortBatch != null)
            //{
            //    SortBatchRepository.Delete(SortBatch);
            //    SortBatchRepository.SaveChanges();
            //    result = true;
            //}
            //else
            //{
            //    strResult = "原因:没有找到相应数据";
            //}
            return result;
        }

        public System.Data.DataTable GetSortBatch(int page, int rows, string SortBatchId)
        {
            System.Data.DataTable dt = new System.Data.DataTable();
          
                //IQueryable<SortBatch> batchsortQuery = SortBatchRepository.GetQueryable();
                //IQueryable<Batch> batchQuery = BatchRepository.GetQueryable();
                //IQueryable<SortingLine> SortingLineQuery = SortingLineRepository.GetQueryable();
                //var batchsort = batchsortQuery.Join(batchQuery, a => a.BatchId, u => u.BatchId, (a, u) => new
                //{
                //    a.BatchId,
                //    a.SortBatchId,
                //    BatchName = u.BatchName,
                //    BatchNo = u.BatchNo,
                //    OperateDate = u.OperateDate,

                //    a.SortingLineCode,
                //    pSortingLineName = SortingLineQuery.Where(b => b.SortingLineCode == a.SortingLineCode).Select(b => b.SortingLineName),
                //    pSortingLineType = SortingLineQuery.Where(b => b.SortingLineCode == a.SortingLineCode).Select(b => b.SortingLineType == "1" ? "半自动" : "全自动"),
                //    a.Status

                //});

                //var batch = batchsort.OrderByDescending(a => a.SortBatchId).ToArray()
                //   .Select(a =>
                //   new
                //   {
                //       a.SortBatchId,
                //       a.BatchId,
                //       a.BatchName,
                //       a.BatchNo,

                //       Status = WhatStatus(a.Status),
                //       a.SortingLineCode,
                //       pSortingLineName=a.pSortingLineName.ToArray().Count()>0?a.pSortingLineName.ToArray()[0]:"",
                //       pSortingLineType = a.pSortingLineType.ToArray().Count()>0?a.pSortingLineType.ToArray()[0]:"",
                //       OperateDate = a.OperateDate.ToString("yyyy-MM-dd HH:mm:ss")

                //   }).ToArray();
             
                //dt.Columns.Add("批次号", typeof(string));
                //dt.Columns.Add("批次名称", typeof(string));
                //dt.Columns.Add("分拣日期", typeof(string));
                //dt.Columns.Add("分拣线名称", typeof(string));
            
                //dt.Columns.Add("分拣线类型", typeof(string));
                //dt.Columns.Add("状态", typeof(string));

                //foreach (var item in batch)
                //{
                //    dt.Rows.Add
                //        (
                //        item.BatchNo,
                //        item.BatchName,
                //        item.OperateDate,
                //        item.pSortingLineName,
                //        item.pSortingLineType,
                //        item.Status
                //        );
                //}
          
            return dt;
        }

    }
}
