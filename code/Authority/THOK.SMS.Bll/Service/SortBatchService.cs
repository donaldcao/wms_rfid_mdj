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

        protected override Type LogPrefix
        {
            get { return this.GetType(); }
        }

        //判断其状态
        public string WhatStatus(string state)
        {
            string statusStr = "";
            switch (state)
            {
                case "01":
                    statusStr = "未下载";
                    break;
                case "02":
                    statusStr = "已下载";
                    break;
                case "03":
                    statusStr = "已挂起";
                    break;
                case "04":
                    statusStr = "已结单";
                    break;
                case "05":
                    statusStr = "";
                    break;
            }
            return statusStr;
        }



        public string WhatState(string Status)
        {
            string optimizescheduleStr = "";
            switch (Status)
            {
                case "未下载":
                    optimizescheduleStr = "01";
                    break;
                case "已下载":
                    optimizescheduleStr = "02";
                    break;
                case "已挂起":
                    optimizescheduleStr = "03";
                    break;
                case "已结单":
                    optimizescheduleStr = "04";
                    break;
                case "":
                    optimizescheduleStr = "";
                    break;
            }
            return optimizescheduleStr;
        }


        public object GetDetails(int page, int rows, string Status, string BatchNo, string BatchName, string OrderDate)
        {
            return null;
            //IQueryable<SortBatch> batchsortquery = SortBatchRepository.GetQueryable();       
            //IQueryable<SortingLine> sortlingquery = SortingLineRepository.GetQueryable();

            //var batchsort = batchsortquery;

            //if (BatchNo != "")
            //{
            //    int batchNo = 0;
            //    int.TryParse(BatchNo, out batchNo);
            //    if (batchNo > 0)
            //    {
            //        batchsort = batchsortquery.Where(a => a.BatchNo == batchNo);
            //    }
            //}

            //if (OrderDate != string.Empty && OrderDate != null)
            //{
            //    DateTime opdate = Convert.ToDateTime(OrderDate);
            //    batchsort = batchsortquery.Where(a => a.OrderDate == opdate);
            //}
            //if (Status != string.Empty && Status != null)
            //{
            //    batchsort = batchsortquery.Where(a => a.Status == Status);
            //}
            //if (BatchName != string.Empty && BatchName != null)
            //{
            //    batchsort = batchsort.Where(a => a.BatchName==BatchName);
            //}

            //var batchsorts = batchsort.OrderByDescending(a => a.BatchNo).Select(a => new 
            //{

            //    a.SortingLineCode
            //});

            //int total = batchsorts.Count();
            //var batchsRow = batchsort.Skip((page - 1) * rows).Take(rows);

            //var batch = batchsorts.ToArray()
            //     .Select(a =>
            //     new
            //     {
            //         a.SortBatchId,
            //         a.BatchId,
            //         a.BatchName,
            //         a.BatchNo,
            //         Status = WhatStatus(a.Status),
            //         a.SortingLineCode,                  
            //         SortingLineName =a.SortingLineCode==null? "":(sortlingquery.Where(b => b.SortingLineCode==a.SortingLineCode).FirstOrDefault().SortingLineName),            
            //         SortingLineType = a.SortingLineCode == null ? "" : (sortlingquery.Where(b => b.SortingLineCode == a.SortingLineCode).FirstOrDefault().SortingLineType),
            //         OrderDate = a.OrderDate.ToString("yyyy-MM-dd")
            //     });
        
            //return new { total, rows = batch.ToArray() };
        }

        public object GetBatch(int page, int rows, string queryString, string value)
        {
            return null;
            //string batchno = "", batchname = "";

            //if (queryString == "BatchNo")
            //{
            //    batchno = value;
            //}
            //else
            //{
            //    batchname = value;
            //}
            //IQueryable<Batch> batchQuery = BatchRepository.GetQueryable();
            //var batchs = batchQuery.Where(e => e.BatchName.Contains(batchname));

            //if (batchno != "")
            //{
            //    int batchNo = 0;
            //    int.TryParse(batchno, out batchNo);
            //    if (batchNo > 0)
            //    {
            //        batchs = batchs.Where(a => a.BatchNo == batchNo);
            //    }
            //}

            //var batch = batchs.OrderByDescending(a => a.BatchId).OrderBy(e => e.BatchId).AsEnumerable().Select(a => new
            //{
            //    a.BatchId,
            //    a.BatchName,
            //    a.BatchNo,
            //    OrderDate = a.OrderDate.ToString("yyyy-MM-dd")
            //});

            //int total = batch.Count();
            //batch = batch.Skip((page - 1) * rows).Take(rows);
            //return new { total, rows = batch.ToArray() };
        }


        public bool Add(SortBatch SortBatch, out string strResult)
        {

            strResult = string.Empty;
            bool result = false;
            //var al = SortBatchRepository.GetQueryable().FirstOrDefault(a => a.SortBatchId == SortBatch.SortBatchId);
            //if (al == null)
            //{

            //    SortBatch SortBatchs = new SortBatch();
            //    try
            //    {
            //        //SortBatchs.BatchId = SortBatch.BatchId;
            //        SortBatchs.SortingLineCode = SortBatch.SortingLineCode;
            //        SortBatchs.Status = SortBatch.Status;
            //        SortBatchRepository.Add(SortBatchs);
            //        SortBatchRepository.SaveChanges();

            //        result = true;
            //    }
            //    catch (Exception e)
            //    {

            //        strResult = "原因:" + e.Message;
            //    }
            //}
            //else
            //{
            //    strResult = "原因:批次分拣已存在";
            //}
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
