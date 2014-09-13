using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Transactions;
using EntityFramework.Extensions;
using Microsoft.Practices.Unity;
using Microsoft.Win32;
using THOK.Authority.Dal.Interfaces;
using THOK.Common.Entity;
using THOK.SMS.Bll.Interfaces;
using THOK.SMS.Dal.Interfaces;
using THOK.SMS.DbModel;
using THOK.Wms.Dal.Interfaces;
using THOK.Wms.DbModel;

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

        [Dependency]
        public ISortOrderAllotMasterRepository SortOrderAllotMasterRepository { get; set; }

        [Dependency]
        public ISortOrderAllotDetailRepository SortOrderAllotDetailRepository { get; set; }

        [Dependency]
        public IHandSupplyRepository HandSupplyRepository { get; set; }

        [Dependency]
        public ISortSupplyRepository SortSupplyRepository { get; set; }

        [Dependency]
        public IChannelAllotRepository ChannelAllotRepository { get; set; }

        [Dependency]
        public ISystemParameterRepository SystemParameterRepository { get; set; }

        [Dependency]
        public ISortOrderRepository SortOrderRepository { get; set; }

        [Dependency]
        public ISortOrderDetailRepository SortOrderDetailRepository { get; set; }

        protected override Type LogPrefix
        {
            get { return this.GetType(); }
        }

        public string WhatStatus(string status)
        {
            string statusStr = "";
            switch (status)
            {
                case "1":
                    statusStr = "正常";
                    break;
                case "2":
                    statusStr = "异型";
                    break;
                case "3":
                    statusStr = "整件";
                    break;
                case "4":
                    statusStr = "手工";
                    break;
            }
            return statusStr;
        }

        public object GetDetails(int page, int rows, string orderDate, string batchNo, string sortingLineCode)
        {
            var sortDispatchQuery = SortOrderDispatchRepository.GetQueryable().Where(s => s.SortBatchId > 0);
            var sortBatchQuery = SortBatchRepository.GetQueryable();
            var sortDispatch = sortDispatchQuery.Where(s => s.ID == s.ID);
            if (orderDate != string.Empty && orderDate != null)
            {
                orderDate = Convert.ToDateTime(orderDate).ToString("yyyyMMdd");
                sortDispatch = sortDispatch.Where(s => s.OrderDate == orderDate);
            }
            if (batchNo != string.Empty && batchNo != null)
            {
                int TheBatchNo = Convert.ToInt32(batchNo);
                var sortBatchIds = sortBatchQuery.Where(s => s.BatchNo == TheBatchNo).Select(s => s.Id);
                sortDispatch = sortDispatch.Where(b => sortBatchIds.Contains(b.SortBatchId));
            }
            if (sortingLineCode != string.Empty && sortingLineCode != null)
            {
                sortDispatch = sortDispatch.Where(s => s.SortingLineCode == sortingLineCode);
            }
            var temp = sortDispatch.OrderByDescending(b => b.SortBatchId).ThenBy(b => b.DeliverLineNo).AsEnumerable().Select(b => new
            {
                b.SortingLineCode,
                b.SortingLine.SortingLineName,
                OrderDate = sortBatchQuery.Where(s => s.Id == b.SortBatchId).FirstOrDefault().OrderDate.ToString("yyyy-MM-dd"),
                BatchNo = sortBatchQuery.Where(s => s.Id == b.SortBatchId).FirstOrDefault().BatchNo.ToString(),
                b.DeliverLineCode,
                b.DeliverLineNo,
                SortStatus = b.SortStatus == "1" ? "未分拣" : "已分拣",
                b.DeliverLine.DeliverLineName,
                IsActive = b.IsActive == "1" ? "可用" : "不可用"
            });

            int total = temp.Count();
            temp = temp.Skip((page - 1) * rows).Take(rows);
            return new { total, rows = temp.ToArray() };
        }

        public object GetDetails(int page, int rows, SortBatch sortBatch)
        {
            var sortBatchQuery = SortBatchRepository.GetQueryable();
            var sortingLineQuery = SortingLineRepository.GetQueryable();
            var sortBatchDetials = sortBatchQuery.Join(sortingLineQuery, batch => batch.SortingLineCode, line => line.SortingLineCode,
                (batch, line) => new
                {
                    batch.Id,
                    batch.OrderDate,
                    batch.BatchNo,
                    batch.SortingLineCode,
                    batch.NoOneProjectBatchNo,
                    batch.NoOneProjectSortDate,
                    batch.Status,
                    line.SortingLineName,
                    line.ProductType
                })
                .Where(a => a.SortingLineCode.Contains(sortBatch.SortingLineCode) && a.Status.Contains(sortBatch.Status));
            if (sortBatch.OrderDate.CompareTo(Convert.ToDateTime("1900-01-01")) > 0)
            {
                sortBatchDetials = sortBatchDetials.Where(a => a.OrderDate.Equals(sortBatch.OrderDate));
            }
            if (sortBatch.BatchNo > 0)
            {
                sortBatchDetials = sortBatchDetials.Where(a => a.BatchNo.Equals(sortBatch.BatchNo));
            }
            int total = sortBatchDetials.Count();
            sortBatchDetials = sortBatchDetials.OrderBy(a => a.Id).Skip((page - 1) * rows).Take(rows);
            var sortBatchArray = sortBatchDetials.AsEnumerable().Select(a => new
            {
                a.Id,
                OrderDate = a.OrderDate.ToString("yyyy-MM-dd"),
                a.BatchNo,
                a.SortingLineCode,
                a.SortingLineName,
                ProductType = WhatStatus(a.ProductType) + "分拣线",
                a.NoOneProjectBatchNo,
                NoOneProjectSortDate = a.NoOneProjectSortDate.ToString("yyyy-MM-dd"),
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
            IQueryable<SortOrderDispatch> sortDispatchQuery = SortOrderDispatchRepository.GetQueryable();
            var sortOrderDetailRepository = SortOrderDetailRepository.GetQueryable();
            var sortOrderRepository = SortOrderRepository.GetQueryable();
            var sortingAbnormal = sortOrderDetailRepository.Where(s => s.Product.IsAbnormity == "1")
                               .Join(sortOrderRepository,
                               sod => sod.OrderID,
                               so => so.OrderID,
                               (sod, so) => new { so.OrderDate, so.DeliverLineCode, sod.RealQuantity, sod.ProductCode })
                               .GroupBy(a => new { a.OrderDate, a.DeliverLineCode })
                               .Select(a => new { a.Key.DeliverLineCode, quantity = a.Sum(s => s.RealQuantity) });
            var sortingPieces = sortOrderDetailRepository.Where(s => s.Product.IsAbnormity == "0")
                              .Join(sortOrderRepository,
                              sod => sod.OrderID,
                              so => so.OrderID,
                              (sod, so) => new { so.OrderDate, so.DeliverLineCode, sod.RealQuantity, sod.ProductCode })
                              .GroupBy(a => new { a.OrderDate, a.DeliverLineCode })
                              .Select(a => new { a.Key.DeliverLineCode, quantity = a.Sum(s => s.RealQuantity) });
            var sortingManual = sortOrderDetailRepository.Where(s => s.Product.IsAbnormity == "0")
                              .Join(sortOrderRepository,
                              sod => sod.OrderID,
                              so => so.OrderID,
                              (sod, so) => new { so.OrderDate, so.DeliverLineCode, sod.RealQuantity, sod.DemandQuantity, sod.ProductCode })
                              .GroupBy(a => new { a.OrderDate, a.DeliverLineCode })
                              .Select(a => new { a.Key.DeliverLineCode, quantity = a.Sum(s => s.DemandQuantity - s.RealQuantity) });
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
                        var batchNo = sortBatchQuery.Where(a => a.OrderDate.Equals(date) && a.SortingLineCode.Equals(item)).Select(a => new { a.BatchNo, a.Status, a.NoOneProjectBatchNo }).OrderByDescending(a => a.BatchNo).ToArray();
                        if (batchNo.Count() > 0)
                        {
                            if (batchNo[0].Status == "01")
                                continue;
                            else
                            {
                                sortBatch.BatchNo = Convert.ToInt32(batchNo[0].BatchNo) + 1;
                                sortBatch.NoOneProjectBatchNo = Convert.ToInt32(batchNo[0].NoOneProjectBatchNo) + 1;
                            }
                        }
                        else
                        {
                            sortBatch.BatchNo = 1;
                            sortBatch.NoOneProjectBatchNo = 1;
                        }
                        sortBatch.SortingLineCode = item;
                        sortBatch.NoOneProjectSortDate = DateTime.Today.AddDays(1);
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
                        //正常分拣线
                        sortOrderDispatch.SortBatchId = sortBatchQuery.FirstOrDefault(a => a.SortingLineCode.Equals(sortOrderDispatch.SortingLineCode) && a.OrderDate.Equals(date)).Id;
                        //异型烟分拣线
                        if (sortOrderDispatch.SortBatchAbnormalId == 0)
                        {
                            var sortingLineAbnormal = sortingLineQuery.Where(a => a.ProductType == "2" && a.IsActive == "1").Select(a => a.SortingLineCode);
                            if (sortingLineAbnormal.ToArray().Count() > 0)
                            {
                                var sortingLineCode = sortBatchQuery.Where(a => a.OrderDate.Equals(date) && a.Status == "01")
                                    .Join(sortingLineQuery.Where(a => a.ProductType == "2" && a.IsActive == "1"), a => a.SortingLineCode, b => b.SortingLineCode, (a, b) => new { a.Id })
                                    .Select(a => new
                                    {
                                        a.Id,
                                        quantity = sortOrderDispatchQuery.FirstOrDefault(b => b.SortBatchAbnormalId.Equals(a.Id)) == null ? 0 :
                                        sortingAbnormal
                                        .Join(sortOrderDispatchQuery,
                                        b => b.DeliverLineCode,
                                        c => c.DeliverLineCode,
                                       (b, c) => new { b.DeliverLineCode, c.SortBatchAbnormalId, b.quantity })
                                       .GroupBy(b => new { SortBatchAbnormalId = b.SortBatchAbnormalId })
                                       .Select(b => new { quantity = b.Sum(s => s.quantity), b.Key.SortBatchAbnormalId }).FirstOrDefault(s => s.SortBatchAbnormalId.Equals(a.Id)).quantity
                                    }).OrderBy(a => a.quantity).ToArray();
                                sortOrderDispatch.SortBatchAbnormalId = sortingLineCode.FirstOrDefault().Id;
                                SortOrderDispatchRepository.SaveChanges();
                            }
                        }
                        //整件分拣线
                        if (sortOrderDispatch.SortBatchPiecesId == 0)
                        {
                            var sortingLinePieces = sortingLineQuery.Where(a => a.ProductType == "3" && a.IsActive == "1").Select(a => a.SortingLineCode);
                            if (sortingLinePieces.ToArray().Count() > 0)
                            {
                                var sortingLineCode = sortBatchQuery.Where(a => a.OrderDate.Equals(date) && a.Status == "01")
                                   .Join(sortingLineQuery.Where(a => a.ProductType == "3" && a.IsActive == "1"), a => a.SortingLineCode, b => b.SortingLineCode, (a, b) => new { a.Id })
                                   .Select(a => new
                                   {
                                       a.Id,
                                       quantity = sortOrderDispatchQuery.FirstOrDefault(b => b.SortBatchAbnormalId.Equals(a.Id)) == null ? 0 :
                                        sortingPieces
                                        .Join(sortOrderDispatchQuery,
                                        b => b.DeliverLineCode,
                                        c => c.DeliverLineCode,
                                       (b, c) => new { b.DeliverLineCode, c.SortBatchPiecesId, b.quantity })
                                       .GroupBy(b => new { SortBatchPiecesId = b.SortBatchPiecesId })
                                       .Select(b => new { quantity = b.Sum(s => s.quantity), b.Key.SortBatchPiecesId }).FirstOrDefault(s => s.SortBatchPiecesId.Equals(a.Id)).quantity
                                   }).OrderBy(a => a.quantity).ToArray();
                                sortOrderDispatch.SortBatchPiecesId = sortingLineCode.FirstOrDefault().Id;
                                SortOrderDispatchRepository.SaveChanges();
                            }
                        }
                        //手工分拣线
                        if (sortOrderDispatch.SortBatchManualId == 0)
                        {
                            var sortingLineManual = sortingLineQuery.Where(a => a.ProductType == "4" && a.IsActive == "1").Select(a => a.SortingLineCode);
                            if (sortingLineManual.ToArray().Count() > 0)
                            {
                                var sortingLineCode = sortBatchQuery.Where(a => a.OrderDate.Equals(date) && a.Status == "01")
                                   .Join(sortingLineQuery.Where(a => a.ProductType == "4" && a.IsActive == "1"), a => a.SortingLineCode, b => b.SortingLineCode, (a, b) => new { a.Id })
                                   .Select(a => new
                                   {
                                       a.Id,
                                       quantity = sortOrderDispatchQuery.FirstOrDefault(b => b.SortBatchAbnormalId.Equals(a.Id)) == null ? 0 :
                                           sortingManual
                                           .Join(sortOrderDispatchQuery,
                                           b => b.DeliverLineCode,
                                           c => c.DeliverLineCode,
                                          (b, c) => new { b.DeliverLineCode, c.SortBatchManualId, b.quantity })
                                          .GroupBy(b => new { SortBatchManualId = b.SortBatchManualId })
                                          .Select(b => new { quantity = b.Sum(s => s.quantity), b.Key.SortBatchManualId }).FirstOrDefault(s => s.SortBatchManualId.Equals(a.Id)).quantity
                                   }).OrderBy(a => a.quantity).ToArray();
                                sortOrderDispatch.SortBatchManualId = sortingLineCode.FirstOrDefault().Id;
                                SortOrderDispatchRepository.SaveChanges();
                            }
                        }
                    }
                    foreach (var sortingLineCode in sortingLineQuery.Where(a => a.ProductType.Equals("1")).Select(a => a.SortingLineCode))
                    {
                        var sortOrderDispatchDetail = sortOrderDispatchQuery.Where(a => a.OrderDate.Equals(orderDate) && a.SortStatus.Equals("1") && a.SortingLineCode.Equals(sortingLineCode) && a.SortBatchId > 0)
                            .Join(deliverLineQuery, dis => dis.DeliverLineCode, line => line.DeliverLineCode,
                            (dis, line) => new { dis.ID, DeliverLineOrder = line.DeliverOrder, line.DistCode })
                            .Join(deliverDistQuery, a => a.DistCode, dist => dist.DistCode,
                            (a, dist) => new { a.ID, a.DeliverLineOrder, DeliverDistOrder = dist.DeliverOrder })
                            .OrderBy(a => new { a.DeliverDistOrder, a.DeliverLineOrder })
                            .Select(a => new { a.ID, a.DeliverLineOrder, a.DeliverDistOrder }).ToArray();
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

        public bool AddSortBatch(string dispatchId, string productType, out string strResult)
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
            IQueryable<SortOrderDispatch> sortDispatchQuery = SortOrderDispatchRepository.GetQueryable();
            var sortOrderDetailRepository = SortOrderDetailRepository.GetQueryable();
            var sortOrderRepository = SortOrderRepository.GetQueryable();
            var sortingAbnormal = sortOrderDetailRepository.Where(s => s.Product.IsAbnormity == "1")
                               .Join(sortOrderRepository,
                               sod => sod.OrderID,
                               so => so.OrderID,
                               (sod, so) => new { so.OrderDate, so.DeliverLineCode, sod.RealQuantity, sod.ProductCode })
                               .GroupBy(a => new { a.OrderDate, a.DeliverLineCode })
                               .Select(a => new { a.Key.DeliverLineCode, quantity = a.Sum(s => s.RealQuantity) });
            var sortingPieces = sortOrderDetailRepository.Where(s => s.Product.IsAbnormity == "0")
                              .Join(sortOrderRepository,
                              sod => sod.OrderID,
                              so => so.OrderID,
                              (sod, so) => new { so.OrderDate, so.DeliverLineCode, sod.RealQuantity, sod.ProductCode })
                              .GroupBy(a => new { a.OrderDate, a.DeliverLineCode })
                              .Select(a => new { a.Key.DeliverLineCode, quantity = a.Sum(s => s.RealQuantity/50)*50 });
            var sortingManual = sortOrderDetailRepository.Where(s => s.Product.IsAbnormity == "0")
                              .Join(sortOrderRepository,
                              sod => sod.OrderID,
                              so => so.OrderID,
                              (sod, so) => new { so.OrderDate, so.DeliverLineCode, sod.RealQuantity, sod.DemandQuantity, sod.ProductCode })
                              .GroupBy(a => new { a.OrderDate, a.DeliverLineCode })
                              .Select(a => new { a.Key.DeliverLineCode, quantity = a.Sum(s => s.DemandQuantity - s.RealQuantity) });
            foreach (var orderDate in sortOrderDispatchArray)
            {
                try
                {
                    DateTime date = DateTime.ParseExact(orderDate, "yyyyMMdd", System.Globalization.CultureInfo.CurrentCulture);
                    var sortingLineDetailquery = sortingLineQuery;
                    if (productType == "0")
                    {
                        sortingLineDetailquery = sortingLineDetailquery.Where(a => a.IsActive.Equals("1")
                           && !sortBatchQuery.Where(b => b.OrderDate.Equals(date) && b.Status.Equals("01")).Select(b => b.SortingLineCode).Contains(a.SortingLineCode));
                    }
                    else
                    {
                        sortingLineDetailquery = sortingLineDetailquery.Where(a => a.IsActive.Equals("1") && a.ProductType == productType
                            && !sortBatchQuery.Where(b => b.OrderDate.Equals(date) && b.Status.Equals("01")).Select(b => b.SortingLineCode).Contains(a.SortingLineCode));
                    }
                    var sortingLineDetail = sortingLineDetailquery.Select(s => s.SortingLineCode);
                    //更新批次分拣表
                    foreach (var item in sortingLineDetail)
                    {
                        SortBatch sortBatch = new SortBatch();
                        sortBatch.OrderDate = date;
                        var batchNo = sortBatchQuery.Where(a => a.OrderDate.Equals(date) && a.SortingLineCode.Equals(item)).Select(a => new { a.BatchNo, a.Status, a.NoOneProjectBatchNo }).OrderByDescending(a => a.BatchNo).ToArray();
                        if (batchNo.Count() > 0)
                        {
                            if (batchNo[0].Status == "01")
                                continue;
                            else
                            {
                                sortBatch.BatchNo = Convert.ToInt32(batchNo[0].BatchNo) + 1;
                                sortBatch.NoOneProjectBatchNo = Convert.ToInt32(batchNo[0].NoOneProjectBatchNo) + 1;
                            }
                        }
                        else
                        {
                            sortBatch.BatchNo = 1;
                            sortBatch.NoOneProjectBatchNo = 1;
                        }
                        sortBatch.SortingLineCode = item;
                        sortBatch.NoOneProjectSortDate = DateTime.Today.AddDays(1);
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
                        var sortOrderDispatch = sortOrderDispatchQuery.FirstOrDefault(a => a.ID.Equals(id) && a.OrderDate.Equals(orderDate));
                        if (sortOrderDispatch == null)
                        {
                            continue;
                        }
                        //正常分拣线
                        if (productType == "1" || productType == "0")
                        {
                            sortOrderDispatch.SortBatchId = sortBatchQuery.FirstOrDefault(a => a.SortingLineCode.Equals(sortOrderDispatch.SortingLineCode) && a.OrderDate.Equals(date)).Id;
                        }
                        //异型烟分拣线
                        if (productType == "2" || productType == "0")
                        {
                            var sortingLineCode = sortBatchQuery.Where(a => a.OrderDate.Equals(date) && a.Status == "01")
                                .Join(sortingLineQuery.Where(a => a.ProductType == "2" && a.IsActive == "1"), a => a.SortingLineCode, b => b.SortingLineCode, (a, b) => new { a.Id })
                                .Select(a => new
                                {
                                    a.Id,
                                    quantity = sortOrderDispatchQuery.FirstOrDefault(b => b.SortBatchAbnormalId.Equals(a.Id)) == null ? 0 :
                                    sortingAbnormal
                                    .Join(sortOrderDispatchQuery,
                                    b => b.DeliverLineCode,
                                    c => c.DeliverLineCode,
                                   (b, c) => new { b.DeliverLineCode, c.SortBatchAbnormalId, b.quantity })
                                   .GroupBy(b => new { SortBatchAbnormalId = b.SortBatchAbnormalId })
                                   .Select(b => new { quantity = b.Sum(s => s.quantity), b.Key.SortBatchAbnormalId }).FirstOrDefault(s => s.SortBatchAbnormalId.Equals(a.Id)).quantity
                                }).OrderBy(a => a.quantity).ToArray();
                            sortOrderDispatch.SortBatchAbnormalId = sortingLineCode.FirstOrDefault() == null ? 0 : sortingLineCode.FirstOrDefault().Id;
                        }
                        //整件分拣线
                        if (productType == "3" || productType == "0")
                        {
                            var sortingLineCode = sortBatchQuery.Where(a => a.OrderDate.Equals(date) && a.Status == "01")
                                  .Join(sortingLineQuery.Where(a => a.ProductType == "3" && a.IsActive == "1"), a => a.SortingLineCode, b => b.SortingLineCode, (a, b) => new { a.Id })
                                  .Select(a => new
                                  {
                                      a.Id,
                                      quantity = sortOrderDispatchQuery.FirstOrDefault(b => b.SortBatchPiecesId.Equals(a.Id)) == null ? 0 :
                                          sortingManual
                                          .Join(sortOrderDispatchQuery,
                                          b => b.DeliverLineCode,
                                          c => c.DeliverLineCode,
                                         (b, c) => new { b.DeliverLineCode, c.SortBatchPiecesId, b.quantity })
                                         .GroupBy(b => new { SortBatchPiecesId = b.SortBatchPiecesId })
                                         .Select(b => new { quantity = b.Sum(s => s.quantity), b.Key.SortBatchPiecesId }).FirstOrDefault(s => s.SortBatchPiecesId.Equals(a.Id)).quantity
                                  }).OrderBy(a => a.quantity).ToArray();
                            sortOrderDispatch.SortBatchPiecesId = sortingLineCode.FirstOrDefault() == null ? 0 : sortingLineCode.FirstOrDefault().Id;
                        }
                        //手工分拣线
                        if (productType == "4" || productType == "0")
                        {
                            var sortingLineCode = sortBatchQuery.Where(a => a.OrderDate.Equals(date) && a.Status == "01")
                               .Join(sortingLineQuery.Where(a => a.ProductType == "4" && a.IsActive == "1"), a => a.SortingLineCode, b => b.SortingLineCode, (a, b) => new { a.Id })
                               .Select(a => new
                               {
                                   a.Id,
                                   quantity = sortOrderDispatchQuery.FirstOrDefault(b => b.SortBatchAbnormalId.Equals(a.Id)) == null ? 0 :
                                       sortingManual
                                       .Join(sortOrderDispatchQuery,
                                       b => b.DeliverLineCode,
                                       c => c.DeliverLineCode,
                                      (b, c) => new { b.DeliverLineCode, c.SortBatchManualId, b.quantity })
                                      .GroupBy(b => new { SortBatchManualId = b.SortBatchManualId })
                                      .Select(b => new { quantity = b.Sum(s => s.quantity), b.Key.SortBatchManualId }).FirstOrDefault(s => s.SortBatchManualId.Equals(a.Id)).quantity
                               }).OrderBy(a => a.quantity).ToArray();
                            sortOrderDispatch.SortBatchManualId = sortingLineCode.FirstOrDefault() == null ? 0 : sortingLineCode.FirstOrDefault().Id;
                        }
                        SortOrderDispatchRepository.SaveChanges();
                    }
                    foreach (var sortingLineCode in sortingLineQuery.Where(a => a.ProductType.Equals("1")).Select(a => a.SortingLineCode))
                    {
                        var sortOrderDispatchDetail = sortOrderDispatchQuery.Where(a => a.OrderDate.Equals(orderDate) && a.SortStatus.Equals("1") && a.SortingLineCode.Equals(sortingLineCode) && a.SortBatchId > 0)
                            .Join(deliverLineQuery, dis => dis.DeliverLineCode, line => line.DeliverLineCode,
                            (dis, line) => new { dis.ID, DeliverLineOrder = line.DeliverOrder, line.NewDeliverLineCode })
                            .Join(deliverDistQuery, a => a.NewDeliverLineCode, dist => dist.DistCode,
                            (a, dist) => new { a.ID, a.DeliverLineOrder, DeliverDistOrder = dist.DeliverOrder })
                            .OrderBy(a => new { a.DeliverDistOrder, a.DeliverLineOrder })
                            .Select(a => new { a.ID, a.DeliverLineOrder, a.DeliverDistOrder }).ToArray();
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

        public bool Edit(SortBatch sortBatch, string IsRemoveOptimization, out string strResult)
        {

            strResult = string.Empty;
            bool result = false;
            var sortBatchs = SortBatchRepository.GetQueryable().FirstOrDefault(a => a.Id == sortBatch.Id);
            if (sortBatchs != null)
            {
                if (IsRemoveOptimization == "1")
                {
                    //删除订单主表、细表
                    SortOrderAllotDetailRepository.GetQueryable()
                        .Where(s => s.SortOrderAllotMaster.SortBatchId == sortBatch.Id).Delete();
                    SortOrderAllotMasterRepository.GetQueryable()
                        .Where(a => a.SortBatchId == sortBatch.Id).Delete();

                    //删除烟道分配表
                    HandSupplyRepository.GetQueryable()
                        .Where(a => a.SortBatchId == sortBatch.Id).Delete();

                    //删除手工补货表
                    ChannelAllotRepository.GetQueryable()
                        .Where(a => a.SortBatchId.Equals(sortBatch.Id)).Delete();

                    //删除分拣补货计划表
                    SortSupplyRepository.GetQueryable()
                       .Where(a => a.SortBatchId.Equals(sortBatch.Id)).Delete();
                }
                sortBatchs.NoOneProjectBatchNo = sortBatch.NoOneProjectBatchNo;
                sortBatchs.Status = sortBatch.Status;
                sortBatchs.NoOneProjectSortDate = sortBatch.NoOneProjectSortDate;
                SortBatchRepository.SaveChanges();
                result = true;
            }
            else
            {
                strResult = "原因:找不到相应数据";
            }
            return result;
        }

        public bool Delete(string id, out string strResult)
        {

            strResult = string.Empty;
            bool result = false;
            try
            {
                int batchId = Convert.ToInt32(id);
                var SortBatch = SortBatchRepository.GetQueryable().FirstOrDefault(a => a.Id.Equals(batchId));
                if (SortBatch != null)
                {
                    //更新调度表
                    //正常分拣线
                    var sortOrderDispatchQuery = SortOrderDispatchRepository.GetQueryable().Where(a => a.SortBatchId.Equals(batchId)).AsEnumerable();
                    //异型分拣线
                    var sortOrderDispatchAbnormalQuery = SortOrderDispatchRepository.GetQueryable().Where(a => a.SortBatchAbnormalId.Equals(batchId)).AsEnumerable();
                    //整件分拣线
                    var sortOrderDispatchPiecesQuery = SortOrderDispatchRepository.GetQueryable().Where(a => a.SortBatchPiecesId.Equals(batchId)).AsEnumerable();
                    //手工分拣线
                    var sortOrderDispatchManualQuery = SortOrderDispatchRepository.GetQueryable().Where(a => a.SortBatchManualId.Equals(batchId)).AsEnumerable();
                    foreach (var sortOrderDispatch in sortOrderDispatchQuery)
                    {
                        sortOrderDispatch.SortBatchId = 0;
                        sortOrderDispatch.DeliverLineNo = 0;
                    }
                    foreach (var sortOrderDispatch in sortOrderDispatchAbnormalQuery)
                    {
                        sortOrderDispatch.SortBatchAbnormalId = 0;
                        sortOrderDispatch.DeliverLineNo = 0;
                    }
                    foreach (var sortOrderDispatch in sortOrderDispatchPiecesQuery)
                    {
                        sortOrderDispatch.SortBatchPiecesId = 0;
                        sortOrderDispatch.DeliverLineNo = 0;
                    }
                    foreach (var sortOrderDispatch in sortOrderDispatchManualQuery)
                    {
                        sortOrderDispatch.SortBatchManualId = 0;
                        sortOrderDispatch.DeliverLineNo = 0;
                    }
                    SortBatchRepository.Delete(SortBatch);
                    SortOrderDispatchRepository.SaveChanges();
                    result = true;
                }
                else
                {
                    strResult = "原因:没有找到相应数据";
                }
            }
            catch (Exception e)
            {
                strResult = "原因：" + e.Message;
            }
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

        #region  上传一号工程
        public bool UpLoad(SortBatch sortbatch, out string strResult)
        {

            strResult = string.Empty;
            bool result = false;

            try
            {
                int sortBatchId = Convert.ToInt32(sortbatch.Id);
                var sortBatch = SortBatchRepository.GetQueryable().FirstOrDefault(s => s.Id == sortBatchId);
                var sortingLine = SortingLineRepository.GetQueryable().FirstOrDefault(s => s.SortingLineCode == sortBatch.SortingLineCode);
                var systemParameterQuery = SystemParameterRepository.GetQueryable().ToArray();

                var NoOneProIP = systemParameterQuery.FirstOrDefault(s => s.ParameterName.Equals("NoOneProIP")).ParameterValue;
                var NoOneProPort = systemParameterQuery.FirstOrDefault(s => s.ParameterName.Equals("NoOneProPort")).ParameterValue;
                var NoOneProFilePath = systemParameterQuery.FirstOrDefault(s => s.ParameterName.Equals("NoOneProFilePath")).ParameterValue;
                if (sortBatch.Status == "02")  //02 已优化
                {
                    string txtFile = "RetailerOrder" + System.DateTime.Now.ToString("yyyyMMddHHmmss");
                    string zipFile = NoOneProFilePath + txtFile + ".zip";
                    txtFile += ".Order";

                    CreateDataFile(sortingLine.ProductType, sortBatchId, txtFile, zipFile);
                    if (CreateZipFile(NoOneProFilePath, txtFile, zipFile,out strResult))
                    {
                        SendZipFile(NoOneProIP, NoOneProPort, zipFile);
                        sortBatch.Status = "03";
                        SortBatchRepository.SaveChanges();
                        DeleteFiles(NoOneProFilePath);
                        result = true;
                    }
                    else
                    {
                        result = false;
                    }

                }
                else
                {
                    strResult = "原因:已上传一号工程";
                }
            }
            catch (Exception e)
            {
                strResult = "原因：" + e.Message;
            }
            return result;
        }

        /// <summary>
        /// 创建数据文件
        /// </summary>
        /// <param name="orderDate"></param>
        /// <param name="batchNo"></param>
        private void CreateDataFile(string productType, int sortBatchId, string txtFile, string zipFile)
        {
            var systemParameterQuery = SystemParameterRepository.GetQueryable().ToArray();
            var NoOneProFilePath = systemParameterQuery.FirstOrDefault(s => s.ParameterName.Equals("NoOneProFilePath")).ParameterValue;

            FileStream file = new FileStream(NoOneProFilePath + txtFile, FileMode.Create);
            StreamWriter writer = new StreamWriter(file, Encoding.UTF8);

            var deliverLineQuery = DeliverLineRepository.GetQueryable();
            var productQuery = SortOrderAllotDetailRepository.GetQueryable();

            //正常分拣打码
            var uploadOrder = SortOrderAllotMasterRepository.GetQueryable().Where(a => a.SortBatchId == sortBatchId).Join(productQuery, a => a.Id, b => b.MasterId,
                (a, b) => new
                {
                    a.OrderId,
                    a.CustomerCode,
                    a.CustomerName,
                    b.ProductCode,
                    b.ProductName,
                    b.Quantity,
                    a.SortBatch.BatchNo,
                    a.CustomerOrder,
                    a.DeliverLineCode,
                    DeliverLineName = deliverLineQuery.Where(c => c.DeliverLineCode == a.DeliverLineCode).FirstOrDefault().DeliverLineName,
                    a.SortBatch.OrderDate,
                    a.SortBatch.NoOneProjectSortDate,
                    a.SortBatch.SortingLineCode
                }).GroupBy(c => new { c.OrderId, c.CustomerCode, c.CustomerName, c.ProductCode, c.ProductName, c.BatchNo, c.CustomerOrder, c.DeliverLineCode, c.DeliverLineName, c.OrderDate, c.NoOneProjectSortDate, c.SortingLineCode })
                .ToArray()
                .OrderBy(a => a.Key.CustomerOrder)
                .Select(a => new
                {
                    a.Key.OrderId,
                    a.Key.CustomerCode,
                    a.Key.CustomerName,
                    a.Key.ProductCode,
                    a.Key.ProductName,
                    Quantity = a.Sum(b => b.Quantity),
                    a.Key.BatchNo,
                    a.Key.CustomerOrder,
                    a.Key.DeliverLineCode,
                    a.Key.DeliverLineName,
                    OrderDate = a.Key.OrderDate.ToString("yyyy-MM-dd"),
                    NoOneProjectSortDate = a.Key.NoOneProjectSortDate.ToString("yyyy-MM-dd"),
                    a.Key.SortingLineCode
                }).ToArray();
            int i = 1;
            foreach (var row in uploadOrder)
            {
                string rowData = (i++).ToString() + "," + row.OrderId + "," + row.CustomerCode + "," + row.CustomerName + "," + row.ProductCode + ","
                    + row.ProductName + "," + row.Quantity + "," + row.BatchNo + "," + row.CustomerOrder + ","
                    + row.DeliverLineCode + "," + row.DeliverLineName + "," + row.OrderDate + "," + row.NoOneProjectSortDate + ","
                    + row.SortingLineCode + ",1;";
                writer.WriteLine(rowData);
                writer.Flush();
            }

            file.Close();
        }


        /// <summary>
        /// 创建压缩文件
        /// </summary>
        public bool CreateZipFile(string NoOneProFilePath,string txtFile, string zipFile,out string strResult)
        {
            try
            {
                String the_rar;
                RegistryKey the_Reg;
                Object the_Obj;
                String the_Info;
                ProcessStartInfo the_StartInfo;
                Process zipProcess;

                the_Reg = Registry.ClassesRoot.OpenSubKey("Applications\\WinRAR.exe\\Shell\\Open\\Command");
                the_Obj = the_Reg.GetValue("");
                the_rar = the_Obj.ToString();
                the_Reg.Close();
                the_rar = the_rar.Substring(1, the_rar.Length - 7);
                the_Info = " a    " + zipFile + "  " + txtFile;
                the_StartInfo = new ProcessStartInfo();
                the_StartInfo.WorkingDirectory = NoOneProFilePath;
                the_StartInfo.FileName = the_rar;
                the_StartInfo.Arguments = the_Info;
                the_StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                zipProcess = new Process();
                zipProcess.StartInfo = the_StartInfo;
                zipProcess.Start();

                //等待压缩文件进程退出
                while (!zipProcess.HasExited)
                {
                    System.Threading.Thread.Sleep(100);
                }
                strResult = "创建压缩文件成功";
                return true;
            }
            catch (Exception ex)
            {
                strResult = "创建压缩文件失败";
                return false;
            }
        }

        /// <summary>
        /// 发送压缩文件给中软一号工程
        /// </summary>
        public void SendZipFile(string NoOneProIP, string NoOneProPort, string zipFile)
        {

            TcpClient client = new TcpClient();
            client.Connect(NoOneProIP, Convert.ToInt32(NoOneProPort));
            NetworkStream stream = client.GetStream();
            FileStream file = new FileStream(zipFile, FileMode.Open);
            int fileLength = (int)file.Length;
            byte[] fileBytes = BitConverter.GetBytes(fileLength);
            Array.Reverse(fileBytes);
            //发送文件长度
            stream.Write(fileBytes, 0, 4);
            stream.Flush();

            byte[] data = new byte[1024];
            int len = 0;
            while ((len = file.Read(data, 0, 1024)) > 0)
            {
                stream.Write(data, 0, len);
            }

            file.Close();

            byte[] buffer = new byte[1024];
            string recvStr = "";
            while (true)
            {
                int bytes = stream.Read(buffer, 0, 1024);

                if (bytes <= 0)
                    continue;
                else
                {
                    recvStr = Encoding.ASCII.GetString(buffer, bytes - 3, 2);
                    if (recvStr == "##")
                    {
                        recvStr = Encoding.GetEncoding("gb2312").GetString(buffer, 4, bytes - 5);
                        break;
                    }
                }
            }
            client.Close();
            if (recvStr.Split(';').Length > 2)
            {
                throw new Exception(recvStr);
            }
        }

        /// <summary>
        /// 删除数据文件和压缩文件
        /// </summary>
        public void DeleteFiles(string NoOneProFilePath)
        {
            try
            {
                DirectoryInfo dir = new DirectoryInfo(NoOneProFilePath);
                FileInfo[] files = dir.GetFiles("*.*Order");

                if (files != null)
                {
                    foreach (FileInfo file in files)
                        file.Delete();
                }
                dir = new DirectoryInfo(NoOneProFilePath);
                if (dir.Exists)
                {
                    files = dir.GetFiles("*.zip");
                    if (files != null)
                    {
                        foreach (FileInfo file in files)
                            file.Delete();
                    }
                }
            }
            catch (Exception)
            {
            }
        }

        #endregion


        //分拣配送打印
        public System.Data.DataTable DeliverOrderSearchInfo(int page, int rows, string orderDate, string batchNo, string sortingLineCode)
        {
            var sortDispatch = SortOrderDispatchRepository.GetQueryable().Where(a => a.SortBatchId > 0);
            var sortBatchQuery = SortBatchRepository.GetQueryable();
            var sortLineQuery = SortingLineRepository.GetQueryable();
            var deliverLineQuery = DeliverLineRepository.GetQueryable();
            if (orderDate != string.Empty && orderDate != null)
            {
                orderDate = Convert.ToDateTime(orderDate).ToString("yyyyMMdd");
                sortDispatch = sortDispatch.Where(s => s.OrderDate == orderDate);
            }
            if (batchNo != string.Empty && batchNo != null)
            {
                int TheBatchNo = Convert.ToInt32(batchNo);
                var sortBatchIds = sortBatchQuery.Where(s => s.BatchNo == TheBatchNo).Select(s => s.Id);
                sortDispatch = sortDispatch.Where(b => sortBatchIds.Contains(b.SortBatchId));
            }
            if (sortingLineCode != string.Empty && sortingLineCode != null)
            {
                sortDispatch = sortDispatch.Where(s => s.SortingLineCode == sortingLineCode);
            }

            var batchsort = sortDispatch.ToArray().OrderByDescending(b => b.SortBatchId).Select(a => new
            {
                OrderDate = sortBatchQuery.Where(s => s.Id == a.SortBatchId).FirstOrDefault().OrderDate.ToString("yyyy-MM-dd"),
                a.SortingLineCode,
                SortingLineName = sortLineQuery.FirstOrDefault(b => b.SortingLineCode == a.SortingLineCode).SortingLineName,
                a.DeliverLineCode,
                DeliverLineName = deliverLineQuery.FirstOrDefault(c => c.DeliverLineCode == a.DeliverLineCode).DeliverLineName,
                a.DeliverLineNo,
                SortStatus = a.SortStatus == "1" ? "未分拣" : "已分拣",
                IsActive = a.IsActive == "1" ? "可用" : "不可用"
            });

            var batch = batchsort.OrderByDescending(a => a.OrderDate).ToArray()
               .Select(a =>
               new
               {
                   a.OrderDate,
                   a.SortingLineCode,
                   a.SortingLineName,
                   a.DeliverLineCode,
                   a.DeliverLineName,
                   a.DeliverLineNo,
                   a.SortStatus,
                   a.IsActive
               }).ToArray();

            System.Data.DataTable dt = new System.Data.DataTable();
            dt.Columns.Add("订单日期", typeof(string));
            dt.Columns.Add("分拣线编码", typeof(string));
            dt.Columns.Add("分拣线名称", typeof(string));
            dt.Columns.Add("送货线路编码", typeof(string));
            dt.Columns.Add("送货线路名称", typeof(string));
            dt.Columns.Add("送货顺序", typeof(string));
            dt.Columns.Add("分拣状态", typeof(string));
            dt.Columns.Add("是否可用", typeof(string));

            foreach (var item in batch)
            {
                dt.Rows.Add
                    (
                    item.OrderDate,
                    item.SortingLineCode,
                    item.SortingLineName,
                    item.DeliverLineCode,
                    item.DeliverLineName,
                    item.DeliverLineNo,
                    item.SortStatus,
                    item.IsActive
                    );
            }

            return dt;
        }
    }
}
