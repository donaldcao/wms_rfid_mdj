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
using THOK.SMS.Bll.Model;
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
                    SortOrderAllotDetailRepository.GetQueryable()
                        .Where(s => s.sortOrderAllotMaster.SortBatchId == sortBatch.Id).Delete();
                    SortOrderAllotMasterRepository.GetQueryable()
                        .Where(a => a.SortBatchId == sortBatch.Id).Delete();
                    
                    //删除烟道分配表
                    HandSupplyRepository.GetQueryable()
                        .Where(a => a.SortBatchId == sortBatch.Id).Delete();

                    //删除手工补货表
                    ChannelAllotRepository.GetQueryable()
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
            //try
            //{
                int sortBatchId = Convert.ToInt32(id);

                var sortBatchQuery = SortBatchRepository.GetQueryable();
                var sortingLineQuery = SortingLineRepository.GetQueryable();
                var channelQuery = ChannelRepository.GetQueryable();

                var sortOrderDispatchQuery = SortOrderDispatchRepository.GetQueryable();
                var sortOrderQuery = SortOrderRepository.GetQueryable();
                var sortOrderDetailQuery = SortOrderDetailRepository.GetQueryable();

                var channelAllotQuery = ChannelAllotRepository.GetQueryable();
                var sortOrderAllotMasterQuery = SortOrderAllotMasterRepository.GetQueryable();
                var systemParameterQuery = SystemParameterRepository.GetQueryable();

                //优化的分拣日期、批次
                var sortBatch = sortBatchQuery.FirstOrDefault(s => s.Id == sortBatchId);
                string orderDate = sortBatch.OrderDate.ToString("yyyyMMdd");
                //优化的分拣线
                var sortingLine = sortingLineQuery.FirstOrDefault(s => s.SortingLineCode == sortBatch.SortingLineCode);
                //优化的分拣线的可用烟道
                var channel = channelQuery.Where(c => c.SortingLineCode == sortingLine.SortingLineCode && c.IsActive == "1").ToArray();

                //烟道优化后结果
                var channelAllot = channelAllotQuery.Where(c => c.SortBatchId == sortBatch.Id);
                //主单拆完后的结果
                var sortOrderAllot = sortOrderAllotMasterQuery.Where(s => s.SortBatchId == sortBatch.Id);

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
                        sortOrderDispatchQuery = sortOrderDispatchQuery.Where(s => s.SortBatchId == sortBatchId).OrderBy(s => s.DeliverLineNo);
                        sortOrderQuery = sortOrderQuery.Where(s => s.OrderDate == orderDate && sortOrderDispatchQuery.Select(sd=>sd.DeliverLineCode).Contains(s.DeliverLineCode));
                        sortOrderDetailQuery = sortOrderDetailQuery.Where(s => s.OrderID == s.OrderID && s.Product.IsAbnormity == "0");
                    }
                    else
                    {
                        sortOrderDispatchQuery = sortOrderDispatchQuery.Where(s => s.SortBatchId == sortBatchId).OrderBy(s => s.DeliverLineNo);
                        sortOrderQuery = sortOrderQuery.Where(s => s.OrderDate == orderDate && sortOrderDispatchQuery.Select(sd => sd.DeliverLineCode).Contains(s.DeliverLineCode));
                        sortOrderDetailQuery = sortOrderDetailQuery.Where(s => s.OrderID == s.OrderID && s.Product.IsAbnormity == "0");
                        sortOrderDetailQuery.AsParallel().ForAll(s => s.SortQuantity %= 50);
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

                ChannelAllotOptimize(sortBatchId, sortOrderDetailQuery.ToArray(), channel, channelAllotScale);
                OrderSplitOptimize(sortBatchId, sortOrderDispatchQuery.Select(s => s.DeliverLineCode).ToArray(), sortOrderQuery.ToArray(), sortOrderDetailQuery.ToArray());
                OrderDetailSplitOptimize(sortBatchId, sortOrderDispatchQuery.Select(s => s.DeliverLineCode).ToArray(), sortOrderQuery.ToArray(), sortOrderDetailQuery.ToArray(), channelAllotQuery.ToArray(), sortOrderAllotMasterQuery.ToArray());
                sortBatch.Status = "02";
                SortBatchRepository.SaveChanges();
                strResult = "优化成功！";
                return true;
            //}
            //catch (Exception e)
            //{
            //    strResult = "优化失败，详情：" + e.Message;
            //    return false;
            //}
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

        public void ChannelAllotOptimize(int sortBatchId, SortOrderDetail[] sortOrderDetail, Channel[] channel, double ChannelAllotScale)
        {
            //分拣线分拣品牌及数量集
            var beAllotProducts = sortOrderDetail.GroupBy(a => new { a.ProductCode, a.ProductName })
                                                 .Select(s => new
                                                 {
                                                     s.Key.ProductCode,
                                                     s.Key.ProductName,
                                                     Quantity = s.Sum(a => a.SortQuantity)
                                                 })
                                                 .OrderByDescending(s => s.Quantity);

            decimal sumQuantity = beAllotProducts.Sum(s => s.Quantity);
            double middleQuantity = Convert.ToDouble(sumQuantity) * ChannelAllotScale;
            //大小品牌分配数量（占用烟道数）
            Dictionary<string, decimal> bigs = new Dictionary<string, decimal>();
            Dictionary<string, decimal> smalls = new Dictionary<string, decimal>();
            //大小品牌
            string[] bigProductCodeArray = new string[beAllotProducts.Count()];
            string[] smallProductCodeArray = new string[beAllotProducts.Count()];

            int itemCount = 0;
            int bigCount = 0;

            //按划分系数划分大小品牌
            foreach (var item in beAllotProducts)
            {
                itemCount++;
                if (bigs.Sum(b=>b.Value) <= Convert.ToDecimal(middleQuantity))
                {
                    bigCount++;
                    bigProductCodeArray[itemCount - 1] = item.ProductCode;
                    bigs.Add(item.ProductCode, item.Quantity);
                }
                else
                {
                    smalls.Add(item.ProductCode, item.Quantity);
                }
            }
            //大品牌数量（扣除分配数量用）
            Dictionary<string, decimal> bigQuantitys = new Dictionary<string, decimal>();
            foreach (var item in bigs)
            {
                bigQuantitys.Add(item.Key, item.Value);
            }

            var bigChannels = channel.Where(c => (c.ChannelType == "3" || c.ChannelType == "4"))
                .OrderBy(c => c.OrderNo);
            int canAllotBigChannelCount = bigChannels.Count();

            //按可用大品牌烟道进行二次划分
            if (canAllotBigChannelCount < bigCount)
            {
                for (int i = canAllotBigChannelCount; i < bigCount; i++)
                {
                    smallProductCodeArray[i] = bigProductCodeArray[i];
                    bigs.Remove(bigProductCodeArray[i]);
                    bigProductCodeArray[i] = null;
                }
            }

            string[] channelProducts = new string[canAllotBigChannelCount];
            //依据品牌比重分配烟道
            decimal tempSumChannelCount = 0;
            foreach (string productCode in bigProductCodeArray)
            {
                if (productCode != null)
                {
                    bigs[productCode] = bigs[productCode] * canAllotBigChannelCount / sumQuantity > 1 ? Convert.ToInt32(bigs[productCode] * canAllotBigChannelCount / sumQuantity) : 1;
                    tempSumChannelCount += bigs[productCode];
                }
            }
            //烟道占用数四舍五入分配后多还少补
            if (tempSumChannelCount > canAllotBigChannelCount)
            {
                for (int i = 0; i <= tempSumChannelCount - canAllotBigChannelCount - 1; i++)
                {
                    if (bigs[bigProductCodeArray[i]]>1)
                    {
                        bigs[bigProductCodeArray[i]] -= 1;
                    }
                    else
                    {
                        bigs[bigs.First(b => b.Value == bigs.Max(m => m.Value)).Key] -= 1;
                    }
                }
            }
            else if (tempSumChannelCount < canAllotBigChannelCount)
            {
                for (int i = 0; i <= tempSumChannelCount - tempSumChannelCount; i++)
                {
                    bigs[bigProductCodeArray[i]] = bigs[bigProductCodeArray[i]] + 1;
                }
            }

            decimal tempChannelCount = 0;
            foreach (string productCode in bigProductCodeArray)
            {
                if (productCode != null)
                {
                    tempChannelCount += bigs[productCode];
                }
            }
            //验证分配是否正确  
            if (tempChannelCount != canAllotBigChannelCount)
            {
                throw new Exception("Error：大品种占用烟道数与实际不符！");
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
                //单品牌多烟道组保证不在同一烟道组
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
                    if (groupCount[groupNo] == bigChannels.Where(c => c.GroupNo == groupNo).Count())
                    {
                        do
                        {
                            groupQuantity.Remove(groupNo);
                            groupNo = groupQuantity.OrderBy(g => g.Value).ToDictionary(g => g.Key, g => g.Value).First().Key;
                        } while (groupCount[groupNo] == bigChannels.Where(c => c.GroupNo == groupNo).Count());

                    }
                    //确认分配  往烟道分配表写数据
                    var beAllotBigChannel = bigChannels.Where(c => c.GroupNo == groupNo && (usedChannelGroupNo.Count == 0 || usedChannelGroupNo.Keys.Contains(c.ChannelCode) == false)).FirstOrDefault();
                    ChannelAllot addChannelAllot = new ChannelAllot();
                    addChannelAllot.SortBatchId = sortBatchId;
                    addChannelAllot.ChannelCode = beAllotBigChannel.ChannelCode;
                    string productName = beAllotProducts.Where(p => p.ProductCode == productCode).Select(p => p.ProductName).FirstOrDefault();
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

                //小品种烟道够不够用判断
                var canAllotMixChannels = channel.Where(c => c.ChannelType == "5").OrderBy(c => c.OrderNo);
                int canAllotMixChannelCount = canAllotMixChannels.Count();
                var canAllotSingleSmallChannels = channel.Where(c => c.ChannelType == "2").OrderBy(c => c.OrderNo);
                int canAllotSmallChannelCount = canAllotSingleSmallChannels.Count();
                if (canAllotMixChannelCount == 0 && canAllotSmallChannelCount < smalls.Count)
                {
                    throw new Exception("Error：小品种占用烟道数与实际不符！");
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
                    string productName = productCode == null ? null : beAllotProducts.Where(p => p.ProductCode == productCode).Select(p => p.ProductName).FirstOrDefault();
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
                                string productName = productCode == null ? null : beAllotProducts.Where(p => p.ProductCode == productCode).Select(p => p.ProductName).FirstOrDefault();
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

        private void OrderSplitOptimize(int sortBatchId, string[] deliverLineCodes, SortOrder[] sortOrders, SortOrderDetail[] sortOrderDetails)
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

        private void OrderDetailSplitOptimize(int sortBatchId, string[] deliverLineCodes, SortOrder[] sortOrders, SortOrderDetail[] sortOrderDetails, ChannelAllot[] channelAllots, SortOrderAllotMaster[] sortOrderAllots)
        {
            var channelGroupInfos = channelAllots.Select(c => c.channel.GroupNo)
                                                 .Distinct()
                                                 .Select(g => new ChannelGroupInfo { ChannelGroup = g, Quantity = 0 })
                                                 .ToArray();

            var channelAllotInfos = channelAllots.Select(c => new ChannelAllotInfo { Id = c.Id, ChannelCode = c.ChannelCode, 
                    GroupNo = c.channel.GroupNo, OrderNo = c.channel.OrderNo, ProductCode = c.ProductCode,
                    Quantity = ((c.Quantity - c.channel.RemainQuantity) / 50) * 50,
                    RemainQuantity = c.channel.RemainQuantity + ((c.Quantity - c.channel.RemainQuantity) % 50), 
                    ChannelCapacity = c.channel.ChannelCapacity})
                .ToArray();

            foreach (var deliverLineCode in deliverLineCodes)
            {
                var sortOrdersArray = sortOrders.Where(s => s.DeliverLineCode == deliverLineCode)
                                                .OrderBy(s => s.DeliverOrder)
                                                .ToArray();

                foreach (var sortOrder in sortOrdersArray)
                {
                    var packInfos = sortOrderAllots.Where(s => s.OrderId == sortOrder.OrderID)
                                                   .OrderBy(s => s.PackNo)
                                                   .Select(s => new PackInfo { Id = s.Id, PackNo = s.PackNo, Quantity = s.Quantity, SortOrderAllot = s })
                                                   .ToArray();

                    var sortOrderDetailInfos = sortOrderDetails.Where(s => s.OrderID == sortOrder.OrderID && s.SortQuantity > 0)
                        .OrderByDescending(s => s.SortQuantity)
                        .Select(s => new SortOrderDetailInfo { ProductCode = s.ProductCode, ProductName = s.ProductName,
                            Quantity = s.SortQuantity })
                        .ToArray();

                    //主单对应细单数据分配优化
                    foreach (var packInfo in packInfos)
                    {
                        foreach (var channelGroupInfo in channelGroupInfos)
                        {
                            if (packInfo.Quantity == 0)
                                break;

                            var tmp1 = sortOrderDetailInfos.Where(s => s.Quantity > 0
                                    && (channelAllotInfos.Where(c => c.GroupNo == channelGroupInfo.ChannelGroup)).Any(t => t.ProductCode == s.ProductCode))
                                .ToArray();

                            foreach (var sortOrderDetailInfo in tmp1)
                            {
                                if (packInfo.Quantity == 0)
                                    break;

                                var tmp2 = channelAllotInfos.Where(c => c.ProductCode == sortOrderDetailInfo.ProductCode
                                                                && c.GroupNo == channelGroupInfo.ChannelGroup)
                                                            .OrderBy(c => c.OrderNo)
                                                            .ToArray();

                                while (packInfo.Quantity > 0 && sortOrderDetailInfo.Quantity > 0 && tmp2.Sum(t => t.Quantity + t.RemainQuantity) > 0)
                                {
                                    foreach (var channelAllotInfo in tmp2)
                                    {
                                        if (packInfo.Quantity == 0 || sortOrderDetailInfo.Quantity == 0)
                                            break;

                                        decimal allotQuantity = 0;
                                        
                                        if (channelAllotInfo.Quantity >= 50)
                                        {
                                            allotQuantity = sortOrderDetailInfo.Quantity < (channelAllotInfo.RemainQuantity - (channelAllotInfo.ChannelCapacity - 50)) ? sortOrderDetailInfo.Quantity : (channelAllotInfo.RemainQuantity - (channelAllotInfo.ChannelCapacity - 50));
                                        }
                                        else
                                        {
                                            allotQuantity = sortOrderDetailInfo.Quantity < channelAllotInfo.RemainQuantity ? sortOrderDetailInfo.Quantity : channelAllotInfo.RemainQuantity;
                                        }

                                        allotQuantity = packInfo.Quantity < allotQuantity ? packInfo.Quantity : allotQuantity;

                                        if (allotQuantity > 0 && channelAllotInfo.OrderNo == tmp2.Where(t=> t.Quantity + t.RemainQuantity > 0).Min(t => t.OrderNo))
                                        {
                                            packInfo.Quantity -= (int)allotQuantity;
                                            sortOrderDetailInfo.Quantity -= allotQuantity;
                                            channelAllotInfo.RemainQuantity -= (int)allotQuantity;
                                            channelGroupInfo.Quantity += (int)allotQuantity;

                                            SortOrderAllotDetail addSortOrderAllotDetail = new SortOrderAllotDetail();
                                            addSortOrderAllotDetail.MasterId = packInfo.Id;
                                            addSortOrderAllotDetail.ChannelCode = channelAllotInfo.ChannelCode;
                                            addSortOrderAllotDetail.ProductCode = sortOrderDetailInfo.ProductCode;
                                            addSortOrderAllotDetail.ProductName = sortOrderDetailInfo.ProductName;
                                            addSortOrderAllotDetail.Quantity = (int)allotQuantity;
                                            //SortOrderAllotDetailRepository.Add(addSortOrderAllotDetail);
                                            packInfo.SortOrderAllot.SortOrderAllotDetails.Add(addSortOrderAllotDetail);
                                        }

                                        if (channelAllotInfo.RemainQuantity <= (channelAllotInfo.ChannelCapacity - 50) && channelAllotInfo.Quantity >= 50)
                                        {
                                            channelAllotInfo.RemainQuantity += 50;
                                            channelAllotInfo.Quantity -= 50;
                                            channelAllotInfo.OrderNo = tmp2.Max(t => t.OrderNo) + 1;
                                        }
                                    }
                                }
                            }
                        }

                        if (packInfo.Quantity > 0)
                        {
                            throw new Exception("优化出现错误，烟包分配细单时出错，有数量未分配！");
                        }
                    }
                }
            }
            SortOrderAllotDetailRepository.SaveChanges();
        }

        #endregion

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
                    string txtFile =NoOneProFilePath+ "RetailerOrder" + System.DateTime.Now.ToString("yyyyMMddHHmmss");
                    string zipFile =txtFile + ".zip";
                    txtFile += ".Order";
                  
                  try
                    {
                        CreateDataFile(sortingLine.ProductType, sortBatchId, txtFile, zipFile);
                        CreateZipFile(NoOneProFilePath, txtFile, zipFile);
                        SendZipFile(NoOneProIP, NoOneProPort, zipFile);
                        sortBatch.Status = "03";
                        SortBatchRepository.SaveChanges();
                        DeleteFiles(NoOneProFilePath); 
                        result = true;
                    }
                    catch (Exception e)
                    {
                        strResult = "原因：" + e.Message;
                        return false;
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
        private void CreateDataFile(string productType,int sortBatchId, string txtFile, string zipFile)
        {
            FileStream file = new FileStream(txtFile, FileMode.Create);
            StreamWriter writer = new StreamWriter(file, Encoding.UTF8);           

            var deliverLineQuery = DeliverLineRepository.GetQueryable();
            var productQuery = SortOrderAllotDetailRepository.GetQueryable();

            //正常分拣打码
            var uploadOrder = SortOrderAllotMasterRepository.GetQueryable().Where(a => a.SortBatchId == sortBatchId).Select(c => new
            {
                c.Id,
                c.SortBatchId,
                c.PackNo,
                c.OrderId,
                c.DeliverLineCode,
                DeliverLineName = deliverLineQuery.Where(b => b.DeliverLineCode == c.DeliverLineCode).FirstOrDefault().DeliverLineName,
                ProductCode = productQuery.Where(d => d.MasterId == c.Id).FirstOrDefault().ProductCode,
                ProductName = productQuery.Where(d => d.MasterId == c.Id).FirstOrDefault().ProductName,
                c.CustomerCode,
                c.CustomerName,
                c.CustomerOrder,
                c.CustomerDeliverOrder,
                c.CustomerInfo,
                c.Quantity,
                c.ExportNo,
                c.FinishTime,
                c.StartTime,
                c.Status
            }).ToArray();

            foreach (var row in uploadOrder)
            {
                string rowData = row.ToString().Trim() + ";";
                writer.WriteLine(rowData);
                writer.Flush();
            }
        }


        /// <summary>
        /// 创建压缩文件
        /// </summary>
        public void CreateZipFile(string NoOneProFilePath,string txtFile, string zipFile)
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
        }

        /// <summary>
        /// 发送压缩文件给中软一号工程
        /// </summary>
        public void SendZipFile(string NoOneProIP,string NoOneProPort,string zipFile)
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
    }
}
