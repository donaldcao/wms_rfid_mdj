using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using Microsoft.Practices.Unity;
using THOK.Authority.Dal.Interfaces;
using THOK.Common.Entity;
using THOK.SMS.Bll.Interfaces;
using THOK.SMS.Dal.Interfaces;
using THOK.SMS.DbModel;
using THOK.Wms.Dal.Interfaces;
using THOK.Wms.DbModel;
using THOK.Wms.SignalR.Common;

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
        public ISortOrderRepository SortOrderRepository { get; set; }

        [Dependency]
        public ISortOrderDetailRepository SortOrderDetailRepository { get; set; }

        [Dependency]
        public ISortOrderAllotMasterRepository SortOrderAllotMasterRepository { get; set; }

        [Dependency]
        public ISortOrderAllotDetailRepository SortOrderAllotDetailRepository { get; set; }

        [Dependency]
        public IHandSupplyRepository HandSupplyRepository { get; set; }

        [Dependency]
        public IChannelRepository ChannelRepository { get; set; }

        [Dependency]
        public IChannelAllotRepository ChannelAllotRepository { get; set; }

        [Dependency]
        public ISystemParameterRepository SystemParameterRepository { get; set; }

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
                        sortOrderDispatch.SortBatchId = sortBatchQuery.FirstOrDefault(a => a.SortingLineCode.Equals(sortOrderDispatch.SortingLineCode) && a.OrderDate.Equals(date)).Id;
                    }
                    SortOrderDispatchRepository.SaveChanges();
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
                    var sortOrderAllotMasterQuery = SortOrderAllotMasterRepository.GetQueryable().Where(a => a.SortBatchId.Equals(sortBatch.Id)).AsEnumerable();
                    var sortOrderAllotDetailQuery = SortOrderAllotDetailRepository.GetQueryable();
                    foreach (var master in sortOrderAllotMasterQuery)
                    {
                        var sortOrderAllotDetail = sortOrderAllotDetailQuery.Where(a => a.MasterId.Equals(master.Id)).AsEnumerable();
                        foreach (var detail in sortOrderAllotDetail)
                        {
                            SortOrderAllotDetailRepository.Delete(detail);
                        }
                        SortOrderAllotMasterRepository.Delete(master);
                    }
                    //删除烟道分配表
                    var handSupplyQuery = HandSupplyRepository.GetQueryable().Where(a => a.SortBatchId.Equals(sortBatch.Id)).AsEnumerable();
                    foreach (var handSupply in handSupplyQuery)
                    {
                        HandSupplyRepository.Delete(handSupply);
                    }
                    //删除手工补货表
                    var channelAllotQuery = ChannelAllotRepository.GetQueryable().Where(a => a.SortBatchId.Equals(sortBatch.Id)).AsEnumerable();
                    foreach (var channelAllot in channelAllotQuery)
                    {
                        ChannelAllotRepository.Delete(channelAllot);
                    }
                    SortOrderDispatchRepository.SaveChanges();
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
                    var sortOrderDispatchQuery = SortOrderDispatchRepository.GetQueryable().Where(a => a.SortBatchId.Equals(batchId)).AsEnumerable();
                    foreach (var sortOrderDispatch in sortOrderDispatchQuery)
                    {
                        sortOrderDispatch.SortBatchId = 0;
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

        public bool Optimize(string id, out string strResult)
        {
            strResult = string.Empty;
            bool result = false;
            try
            {
                int sortBatchId = Convert.ToInt32(id);

                var systemParameterQuery = SystemParameterRepository.GetQueryable();
                var sortingLineQuery = SortingLineRepository.GetQueryable();
                var channelQuery = ChannelRepository.GetQueryable();
                var sortOrderDispatchQuery = SortOrderDispatchRepository.GetQueryable();
                var sortOrderQuery = SortOrderRepository.GetQueryable();
                var sortOrderDetailQuery = SortOrderDetailRepository.GetQueryable();

                //优化的分拣批次
                var sortBatch = SortBatchRepository.GetQueryable().FirstOrDefault(s => s.Id == sortBatchId);

                //优化的分拣线
                var sortingLine = sortingLineQuery.FirstOrDefault(s => s.SortingLineCode == sortBatch.SortingLineCode);

                //分拣线可用烟道
                var channel = channelQuery.Where(c => c.SortingLineCode == sortingLine.SortingLineCode && c.IsActive == "1");

                //烟道优化后结果
                var channelAllot = ChannelAllotRepository.GetQueryable().Where(c => c.SortBatchId == sortBatch.Id);

                //是否使用整件线
                bool isUseWholePieceSortingLine = sortingLineQuery.Where(s => s.ProductType == "3").Count() > 0;

                //大小品种烟道划分比例
                double channelAllotScale = Convert.ToDouble(systemParameterQuery
                                               .Where(s => s.ParameterName == "ChannelAllotScale")
                                               .Select(s => s.ParameterValue).FirstOrDefault());

                if (sortingLine.ProductType == "1")
                {
                    if (!isUseWholePieceSortingLine)
                    {
                        var sortOrderInfo = sortOrderDispatchQuery.Where(s => s.SortBatchId == sortBatchId)
                            .Join(sortOrderQuery,
                                sod => new { sod.OrderDate, sod.DeliverLineCode },
                                so => new { so.OrderDate, so.DeliverLineCode },
                                (sod, so) => new { sod, so })
                            .Join(sortOrderDetailQuery.Where(s => s.Product.IsAbnormity == "0"),
                                t => t.so.OrderID,
                                sod => sod.OrderID,
                                (t, sod) => new { SortOrderDispatch = t.sod, SortOrder = t.so, SortOrderDetail = sod })
                            .OrderBy(t => t.SortOrder.DeliverLine.DeliverOrder)
                            .ThenBy(t => t.SortOrder.DeliverOrder);

                        ChannelAllotOptimize(sortBatchId, sortOrderInfo.Select(s=>s.SortOrderDetail), channel, channelAllotScale);
                        OrderSplitOptimize(sortBatchId, sortOrderInfo.Select(s=>s.SortOrderDispatch.DeliverLineCode), sortOrderInfo.Select(s => s.SortOrder), sortOrderInfo.Select(s => s.SortOrderDetail));
                        OrderDetailSplitOptimize(sortBatchId, sortOrderInfo.Select(s => s.SortOrderDispatch.DeliverLineCode), sortOrderInfo.Select(s => s.SortOrder).ToArray(), sortOrderInfo.Select(s => s.SortOrderDetail).ToArray(), channelAllot.ToArray());
                    }
                    else
                    {
                        var sortOrderInfo = sortOrderDispatchQuery.Where(s => s.SortBatchId == sortBatchId)
                            .Join(sortOrderQuery,
                                sod => new { sod.OrderDate, sod.DeliverLineCode },
                                so => new { so.OrderDate, so.DeliverLineCode },
                                (sod, so) => new { sod, so })
                            .Join(sortOrderDetailQuery.Where(s => s.Product.IsAbnormity == "0"),
                                t => t.so.OrderID,
                                sod => sod.OrderID,
                                (t, sod) => new { SortOrderDispatch = t.sod, SortOrder = t.so, SortOrderDetail = sod })
                            .OrderBy(t => t.SortOrder.DeliverLine.DeliverOrder)
                            .ThenBy(t => t.SortOrder.DeliverOrder);

                        sortOrderInfo.AsParallel().ForAll(s => s.SortOrderDetail.SortQuantity %= 50);

                        ChannelAllotOptimize(sortBatchId, sortOrderInfo.Select(s => s.SortOrderDetail), channel, channelAllotScale);
                        OrderSplitOptimize(sortBatchId, sortOrderInfo.Select(s => s.SortOrderDispatch.DeliverLineCode), sortOrderInfo.Select(s => s.SortOrder), sortOrderInfo.Select(s => s.SortOrderDetail));
                        OrderDetailSplitOptimize(sortBatchId, sortOrderInfo.Select(s => s.SortOrderDispatch.DeliverLineCode), sortOrderInfo.Select(s => s.SortOrder).ToArray(), sortOrderInfo.Select(s => s.SortOrderDetail).ToArray(), channelAllot.ToArray());
                    }
                }
                else if (sortingLine.ProductType == "2")
                {
                    channelAllotScale = 0;
                    //异型分拣
                    sortOrderDetailQuery = sortOrderDetailQuery.Where(s => s.Product.IsAbnormity == "1");
                    //异型分拣对应的配送线路未实现：
                    //sortOrderOrderDispatchQuery = sortOrderOrderDispatchQuery.Where(s => s.SortBatchId == sortBatchId);
                }
                else if (sortingLine.ProductType == "3")
                {
                    channelAllotScale = 0;
                    sortOrderDetailQuery = sortOrderDetailQuery.Where(s => s.Product.IsAbnormity == "0");
                    foreach (var sortOrderDetail in sortOrderDetailQuery)
                    {
                        sortOrderDetail.SortQuantity = sortOrderDetail.SortQuantity / 50 * 50;
                    }
                    //整件分拣对应的配送线路：
                    //sortOrderOrderDispatchQuery = sortOrderOrderDispatchQuery.Where(s => s.SortBatchId == sortBatchId);
                }
                else if (sortingLine.ProductType == "4")
                {
                    channelAllotScale = 0;
                    //手工分拣
                    sortOrderDetailQuery = sortOrderDetailQuery.Where(s => s.Product.IsAbnormity == "0");
                    foreach (var sortOrderDetail in sortOrderDetailQuery)
                    {
                        sortOrderDetail.SortQuantity = sortOrderDetail.RealQuantity - sortOrderDetail.SortQuantity;
                    }
                    //手工分拣对应的配送线路未实现：
                    //sortOrderOrderDispatchQuery = sortOrderOrderDispatchQuery.Where(s => s.SortBatchId == sortBatchId);
                }

                strResult = "优化成功！";
                result = true;

            }
            catch (Exception e)
            {
                strResult = "优化失败，详情：" + e.Message;
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

        #region 优化

        public void ChannelAllotOptimize(int sortBatchId, IQueryable<SortOrderDetail> sortOrderDetail, IQueryable<Channel> channel, double ChannelAllotScale)
        {
            //分拣线分拣品牌及数量
            var products = sortOrderDetail.GroupBy(a => new { a.ProductCode, a.ProductName })
                                          .Select(s => new
                                            {
                                                s.Key.ProductCode,
                                                s.Key.ProductName,
                                                Quantity = s.Sum(a => a.SortQuantity)
                                            })
                                          .OrderByDescending(s => s.Quantity);

            decimal sumQuantity = products.Sum(s => s.Quantity);
            double middleQuantity = Convert.ToDouble(sumQuantity) * ChannelAllotScale;

            Dictionary<string, decimal> bigs = new Dictionary<string, decimal>();
            Dictionary<string, decimal> smalls = new Dictionary<string, decimal>();
            Dictionary<string, decimal> bigQuantitys = new Dictionary<string, decimal>();
                        
            //按划系数划分大小品牌
            foreach (var item in products)
            {
                if (bigs.Sum(b=>b.Value) <= Convert.ToDecimal(middleQuantity) && true)
                {
                    bigs.Add(item.ProductCode, item.Quantity);
                }
                else
                {
                    smalls.Add(item.ProductCode, item.Quantity);
                }
            }

            var BigChannels = channel.Where(c => c.ProductCode == "" && ( c.ChannelType == "3" || c.ChannelType == "4"))
                                             .OrderBy(c => c.OrderNo);

            int BigChannelCount = BigChannels.Count();

            //按可用大品牌烟道进行二次划分
            if (BigChannelCount < bigs.Count)
            {
                for (int i = 0; i < bigs.Count - BigChannelCount; i++)
                {
                    var big = bigs.Where(b=>b.Value == bigs.Min(bb=>bb.Value)).FirstOrDefault();
                    smalls.Add(big.Key,big.Value);
                    bigs.Remove(big.Key);
                }
            }
  
            //依据品牌比重分配烟道
            //decimal tempSumChannelCount = 0;
            //foreach (string productCode in bigs.Keys)
            //{
            //    if (productCode != null)
            //    {
            //        bigs[productCode] = (bigs[productCode] * BigChannelCount) / sumQuantity > 1 ? Convert.ToInt32(bigs[productCode] * BigChannelCount / sumQuantity) : 1; ;
            //        tempSumChannelCount += bigs[productCode];
            //    }
            //}



            //验证分配是否正确  
            if (tempChannelCount != BigChannelCount)
            {
                throw new Exception("Error：烟道分配出错  无法继续执行！");
            }
            //是则继续进行大品种单品牌多烟道数量拆分
            else
            {
                //大品种烟道优化（New）   采用大品种烟道组A,B(C...)平均分配原则
                var channelGroups = channel.Select(c => c.GroupNo).Distinct();
                Dictionary<int, int> groupQuantity = new Dictionary<int, int>();
                Dictionary<int, int> groupCount = new Dictionary<int, int>();
                Dictionary<string, int> usedChannelGroupNo = new Dictionary<string, int>();
                foreach (var groupNo in channelGroups)
                {
                    groupQuantity.Add(groupNo, 0);
                    groupCount.Add(groupNo, 0);
                }
                 
                string tempProductCode = "";
                while (bigs.Max(v => v.Value) > 0)
                {
                    string productCode = null;
                    bool isChangeProduct = true;
                    int groupNo = 0;
                    int quantity = 0;
                    for (int i = 0; i < bigCount; i++)
                    {
                        productCode = bigProductCodeArray[i];
                        if (bigs[productCode] >= 1)
                        {
                            int allotQuantity = Convert.ToInt32(bigQuantitys[productCode]);
                            quantity = (allotQuantity - allotQuantity % 50) / 50 / Convert.ToInt32(bigs[productCode]) * 50;
                            bigQuantitys[productCode] -= quantity;
                            bigs[productCode] -= 1;
                            break;
                        }
                        else
                        {
                            continue;
                        }
                    }
                    if (productCode == tempProductCode)
                    {
                        isChangeProduct = false;
                    }
                    tempProductCode = productCode;
                    //如果品牌未更换 要保证该品牌分配在多个烟道组上
                    if (!isChangeProduct)
                    {
                        var channelGroup = ChannelAllotRepository.GetQueryable().Where(c => c.ProductCode == tempProductCode).Select(c => c.channel.GroupNo).Distinct();
                        int tempCount = groupQuantity.Where(g => !channelGroup.Contains(g.Key)).Count();
                        if (tempCount > 0)
                        {
                            groupNo = groupQuantity.Where(g => !channelGroup.Contains(g.Key)).OrderBy(g => g.Value).ToDictionary(g => g.Key, g => g.Value).First().Key;
                        }
                        //如果已经保证多烟道组或者换品牌 该品牌分配数量用来调节两线平衡
                        else
                        {
                            groupNo = groupQuantity.OrderBy(g => g.Value).ToDictionary(g => g.Key, g => g.Value).First().Key;
                        }
                    }
                    else
                    {
                        groupNo = groupQuantity.OrderBy(g => g.Value).ToDictionary(g => g.Key, g => g.Value).First().Key;
                    }
                    //分配的组烟道不够用则切换至有空余烟道的组
                    if (groupCount[groupNo] == BigChannels.Where(c => c.GroupNo == groupNo).Count())
                    {
                        do
                        {
                            groupQuantity.Remove(groupNo);
                            groupNo = groupQuantity.OrderBy(g => g.Value).ToDictionary(g => g.Key, g => g.Value).First().Key;
                        } while (groupCount[groupNo] == BigChannels.Where(c => c.GroupNo == groupNo).Count());

                    }
                    //确认分配  往烟道分配表写数据
                    var beAllotBigChannel = BigChannels.Where(c => c.GroupNo == groupNo && (usedChannelGroupNo.Count == 0 || usedChannelGroupNo.Keys.Contains(c.ChannelCode) == false)).FirstOrDefault();
                    ChannelAllot addChannelAllot = new ChannelAllot();
                    addChannelAllot.SortBatchId = sortBatchId;
                    addChannelAllot.ChannelCode = beAllotBigChannel.ChannelCode;
                    string productName = products.Where(p => p.ProductCode == productCode).Select(p => p.ProductName).FirstOrDefault();
                    addChannelAllot.ProductCode = productCode;
                    addChannelAllot.ProductName = productName;
                    addChannelAllot.Quantity = quantity;
                    ChannelAllotRepository.Add(addChannelAllot);
                    usedChannelGroupNo.Add(beAllotBigChannel.ChannelCode, groupNo);
                    groupQuantity[groupNo] += quantity;
                    groupCount[groupNo] += 1;
                }
                ChannelAllotRepository.SaveChanges();

                //小品种增加大品种零烟
                foreach (var item in bigQuantitys)
                {
                    if (item.Value > 0)
                    {
                        smalls.Add(item.Key, item.Value);
                    }
                }
                smalls = smalls.OrderByDescending(s => s.Value).ToDictionary(s => s.Key, s => s.Value);
                int smallCount = 0;
                foreach (var item in smalls)
                {
                    smallCount++;
                    smallProductCodeArray[itemCount - 1] = item.Key;
                }
                //小品种烟道够不够用判断
                var canAllotMixChannels = channel.Where(c => c.ChannelType == "5").OrderBy(c => c.OrderNo);
                int canAllotMixChannelCount = canAllotMixChannels.Count();
                var canAllotSingleSmallChannels = channel.Where(c => c.ChannelType == "2").OrderBy(c => c.OrderNo);
                int canAllotSmallChannelCount = canAllotSingleSmallChannels.Count();
                if (canAllotMixChannelCount == 0 && canAllotSmallChannelCount < smallCount)
                {
                    throw new Exception("Error：烟道分配出错  无法继续执行！");
                }

                //单一立式机分配
                foreach (var beAllotSingleChannel in canAllotSingleSmallChannels)
                {
                    string productCode = null;
                    int quantity = 0;
                    if (smalls.Count() > 0)
                    {
                        productCode = smalls.FirstOrDefault().Key;
                        quantity = Convert.ToInt32(smalls.FirstOrDefault().Value);
                        smalls.Remove(productCode);
                    }
                    ChannelAllot addChannelAllot = new ChannelAllot();
                    addChannelAllot.SortBatchId = sortBatchId;
                    addChannelAllot.ChannelCode = beAllotSingleChannel.ChannelCode;
                    string productName = productCode == null ? null : products.Where(p => p.ProductCode == productCode).Select(p => p.ProductName).FirstOrDefault();
                    addChannelAllot.ProductCode = productCode;
                    addChannelAllot.ProductName = productName;
                    addChannelAllot.Quantity = quantity;
                    ChannelAllotRepository.Add(addChannelAllot);
                }
                //混合烟道优化分配
                if (smalls.Count > 0)
                {
                    if (canAllotMixChannelCount > 0)
                    {
                        Dictionary<string, int> mixChannelQuantity = new Dictionary<string, int>();
                        foreach (var mixChannel in canAllotMixChannels)
                        {
                            string channelCode = mixChannel.ChannelCode;
                            mixChannelQuantity.Add(channelCode, 0);
                        }
                        foreach (var small in smalls)
                        {
                            if (small.Value > 0)
                            {

                                var mixChannel = mixChannelQuantity.OrderBy(m => m.Value).ToDictionary(m => m.Key, m => m.Value).First();
                                string channelCode = mixChannel.Key;
                                string productCode = small.Key;
                                int quantity = Convert.ToInt32(small.Value);
                                mixChannelQuantity[channelCode] += quantity;
                                ChannelAllot addChannelAllot = new ChannelAllot();
                                addChannelAllot.SortBatchId = sortBatchId;
                                addChannelAllot.ChannelCode = channelCode;
                                string productName = productCode == null ? null : products.Where(p => p.ProductCode == productCode).Select(p => p.ProductName).FirstOrDefault();
                                addChannelAllot.ProductCode = productCode;
                                addChannelAllot.ProductName = productName;
                                addChannelAllot.Quantity = quantity;
                                ChannelAllotRepository.Add(addChannelAllot);
                                
                            }
                        }
                    }
                }
                ChannelAllotRepository.SaveChanges();
            }
        }

        private void OrderSplitOptimize(int sortBatchId, IQueryable<string> deliverLineCodes, IQueryable<SortOrder> sortOrders, IQueryable<SortOrderDetail> sortOrderDetails)
        {
            int packNo = 0;
            int customerOrder = 0;

            foreach (var deliverLineCode in deliverLineCodes)
            {
                int customerDeliverOrder = 0;
                var sortOrdersArray = sortOrders.Where(s => s.DeliverLineCode == deliverLineCode)
                                                .OrderBy(s => s.DeliverOrder)
                                                .ToArray();

                foreach (var sortOrder in sortOrdersArray)
                {
                    var sortOrderDetailArray = sortOrderDetails.Where(s => s.OrderID == sortOrder.OrderID)
                                                               .OrderByDescending(s => s.SortQuantity)
                                                               .ToArray();

                    int orderQuantity = Convert.ToInt32(sortOrderDetailArray.Sum(c => c.SortQuantity));
                    if (orderQuantity == 0 )
                    {
                        continue;
                    }
                    Dictionary<int, int> BagQuantity = new Dictionary<int, int>();

                    int splitQuantity = 25;
                    int bagCount = orderQuantity / splitQuantity;
                    if (orderQuantity % splitQuantity > 0 && orderQuantity % splitQuantity < 5 && orderQuantity > splitQuantity)
                    {
                        for (int i = 1; i <= bagCount - 1; i++)
                        {
                            BagQuantity.Add(++packNo, splitQuantity);
                        }
                        BagQuantity.Add(++packNo, splitQuantity - 5);
                        BagQuantity.Add(++packNo, orderQuantity % splitQuantity + 5);
                    }
                    else
                    {
                        for (int i = 1; i <= bagCount; i++)
                        {
                            BagQuantity.Add(++packNo, splitQuantity);
                        }
                        if (orderQuantity % splitQuantity > 0)
                        {
                            BagQuantity.Add(++packNo, orderQuantity % splitQuantity);
                        }
                    }

                    customerOrder += 1;
                    customerDeliverOrder += 1;

                    //往分拣订单分配主表添加数据
                    foreach (var item in BagQuantity)
                    {
                        SortOrderAllotMaster addSortOrderAllotMaster = new SortOrderAllotMaster();
                        addSortOrderAllotMaster.SortBatchId = sortBatchId;
                        addSortOrderAllotMaster.OrderId = sortOrder.OrderID;
                        addSortOrderAllotMaster.PackNo = item.Key;
                        addSortOrderAllotMaster.Quantity = item.Value;
                        addSortOrderAllotMaster.CustomerCode = sortOrder.CustomerCode;
                        addSortOrderAllotMaster.CustomerName = sortOrder.CustomerName;
                        addSortOrderAllotMaster.DeliverLineCode = sortOrder.DeliverLineCode;
                        addSortOrderAllotMaster.CustomerInfo = "";
                        addSortOrderAllotMaster.CustomerOrder = customerOrder;
                        addSortOrderAllotMaster.CustomerDeliverOrder = customerDeliverOrder;
                        addSortOrderAllotMaster.ExportNo = 0;
                        addSortOrderAllotMaster.StartTime = DateTime.Now;
                        addSortOrderAllotMaster.FinishTime = DateTime.Now;
                        addSortOrderAllotMaster.Status = "01";
                        SortOrderAllotMasterRepository.Add(addSortOrderAllotMaster);
                    }
                }
            }
            SortOrderAllotMasterRepository.SaveChanges();
        }

        private void OrderDetailSplitOptimize(int sortBatchId, IQueryable<string> deliverLineCodes, SortOrder[] sortOrders, SortOrderDetail[] sortOrderDetails, ChannelAllot[] channelAllots)
        {
            var sortOrderAllotMasterQuery = SortOrderAllotMasterRepository.GetQueryable();

            Dictionary<int, int> groupQuantity = channelAllots.Select(c => c.channel.GroupNo)
                                                             .Distinct()
                                                             .ToDictionary(g => g, g => 0);

            foreach (var deliverLineCode in deliverLineCodes)
            {
                var sortOrdersArray = sortOrders.Where(s => s.DeliverLineCode == deliverLineCode)
                                                .OrderBy(s => s.DeliverOrder)
                                                .ToArray();

                foreach (var sortOrder in sortOrdersArray)
                {
                    var sortOrderDetailArray = sortOrderDetails.Where(s => s.OrderID == sortOrder.OrderID)
                                                               .OrderByDescending(s => s.SortQuantity)
                                                               .ToArray();

                    int orderQuantity = Convert.ToInt32(sortOrderDetailArray.Sum(c => c.SortQuantity));

                    if (orderQuantity == 0)
                    {
                        continue;
                    }

                    Dictionary<int, int> bagQuantity = sortOrderAllotMasterQuery.Where(s => s.OrderId == sortOrder.OrderID)
                                                           .ToDictionary(p => p.PackNo, p => p.Quantity);
                    Dictionary<string, int> orderRemainQuantity = sortOrderDetailArray.ToDictionary(c => c.ProductCode, c => Convert.ToInt32(c.SortQuantity));
                    Dictionary<int, int> channelRemainQuantity = channelAllots.ToDictionary(c => c.Id, c => c.Quantity);

                    //主单对应细单数据分配优化
                    foreach (var item in bagQuantity)
                    {
                        int quantity = item.Value;
                        //烟仓出到剩下20条换仓
                        int channelTempQuantity = 20;
                        int groupNo = groupQuantity.OrderBy(g => g.Value).First().Key;

                        Dictionary<int, bool> groupIsAllot = new Dictionary<int, bool>();
                        foreach (var groupItem in groupQuantity.Keys)
                        {
                            groupIsAllot.Add(groupItem, false);
                        }

                        while (quantity > 0)
                        {
                            if (groupIsAllot[groupNo])
                            {
                                if (groupIsAllot.Where(g => g.Value == false).Count() > 0)
                                {
                                    groupNo = groupIsAllot.Where(g => g.Value == false).First().Key;
                                }
                                else
                                {
                                    throw new Exception();
                                }
                            }

                            var tmp = channelAllots.Where(c => c.channel.GroupNo == groupNo);
                            foreach (var sortOrderDetail in orderRemainQuantity.Where(s => s.Value > 0 && tmp.Any(t => t.ProductCode == s.Key)))
                            {
                                string pruductCode = sortOrderDetail.Key;
                                var channelAllotIDs = channelAllots.Where(c => c.ProductCode == pruductCode && c.channel.GroupNo==groupNo)
                                                                     .Select(c => c.Id);
                                var productRemainQuantity = channelRemainQuantity.Where(c => channelAllotIDs.Contains(c.Key))
                                                                                 .OrderByDescending(g => g.Value);
                                if (productRemainQuantity.Count() <= 0)
                                {
                                    continue;
                                }
                                var realAllotChannel = productRemainQuantity.Where(p => p.Value % 50 % channelTempQuantity > 0);
                                int channelAllotId;
                                int channelQuantity;
                                if (realAllotChannel.Count() > 0)
                                {
                                    channelAllotId = realAllotChannel.First().Key;
                                    channelQuantity = realAllotChannel.First().Value % 50 % channelTempQuantity;
                                }
                                else
                                {
                                    channelAllotId = productRemainQuantity.First().Key;
                                    channelQuantity = productRemainQuantity.First().Value;
                                }

                                int realAllotQuantity = channelQuantity > quantity ? quantity : channelQuantity;

                                quantity -= realAllotQuantity;
                                groupQuantity[groupNo] += realAllotQuantity;
                                channelRemainQuantity[channelAllotId] -= realAllotQuantity;
                                orderRemainQuantity[pruductCode] -= realAllotQuantity;
                                SortOrderAllotDetail addSortOrderAllotDetail = new SortOrderAllotDetail();
                                addSortOrderAllotDetail.MasterId = sortOrderAllotMasterQuery.First(s => s.PackNo == item.Key).Id;
                                addSortOrderAllotDetail.ChannelCode = channelAllots.Where(c => c.Id == channelAllotId)
                                                                                   .Select(c => c.ChannelCode)
                                                                                   .First();
                                addSortOrderAllotDetail.ProductCode = pruductCode;
                                addSortOrderAllotDetail.ProductName = channelAllots.Where(c => c.Id == channelAllotId)
                                                                                   .Select(c => c.ProductName)
                                                                                   .First();
                                addSortOrderAllotDetail.Quantity = realAllotQuantity;
                                SortOrderAllotDetailRepository.Add(addSortOrderAllotDetail);
                                if (channelRemainQuantity[channelAllotId] == 0)
                                {
                                    channelRemainQuantity.Remove(channelAllotId);
                                }
                                if (quantity == 0)
                                {
                                    break;
                                }
                            }
                            if (quantity > 0)
                            {
                                groupIsAllot[groupNo] = true;
                            }
                        }
                    }
                }
            }
            SortOrderAllotDetailRepository.SaveChanges();
        }

        #endregion
    }
}
