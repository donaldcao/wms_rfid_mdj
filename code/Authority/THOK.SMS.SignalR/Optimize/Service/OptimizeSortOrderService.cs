using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using EntityFramework.Extensions;
using Microsoft.Practices.Unity;
using THOK.Authority.Dal.Interfaces;
using THOK.Common.SignalR;
using THOK.Common.SignalR.Model;
using THOK.SMS.Dal.Interfaces;
using THOK.SMS.DbModel;
using THOK.SMS.SignalR.Connection;
using THOK.SMS.SignalR.Optimize.Interfaces;
using THOK.SMS.SignalR.Optimize.Model;
using THOK.Wms.Dal.Interfaces;
using THOK.Wms.DbModel;

namespace THOK.SMS.SignalR.Optimize.Service
{
    public class OptimizeSortOrderService : Notifier<OptimizeSortOrderConnection>, IOptimizeSortOrderService
    {

        #region Dependency
        [Dependency]
        public ISortBatchRepository SortBatchRepository { get; set; }

        [Dependency]
        public ISortingLineRepository SortingLineRepository { get; set; }

        [Dependency]
        public IChannelRepository ChannelRepository { get; set; }

        [Dependency]
        public ISystemParameterRepository SystemParameterRepository { get; set; }

        [Dependency]
        public ISortOrderDispatchRepository SortOrderDispatchRepository { get; set; }

        [Dependency]
        public ISortOrderRepository SortOrderRepository { get; set; }

        [Dependency]
        public ISortOrderDetailRepository SortOrderDetailRepository { get; set; }

        [Dependency]
        public IChannelAllotRepository ChannelAllotRepository { get; set; }

        [Dependency]
        public ISortOrderAllotMasterRepository SortOrderAllotMasterRepository { get; set; }

        [Dependency]
        public ISortOrderAllotDetailRepository SortOrderAllotDetailRepository { get; set; }

        [Dependency]
        public ISortSupplyRepository SortSupplyRepository { get; set; }

        [Dependency]
        public IHandSupplyRepository HandSupplyRepository { get; set; }

        #endregion

        public void Optimize(string connectionId, ProgressState ps, CancellationToken cancellationToken, string id)
        {
            try
            {
                ConnectionId = connectionId;
                ps.State = StateType.Start;
                NotifyConnection(ps.Clone());
                StateTypeForProcessing(ps, "数据提取", new Random().Next(1, 5), "正在提取" + "优化数据", new Random().Next(1, 100));
                int sortBatchId = Convert.ToInt32(id);
                //优化的批次
                SortBatch sortBatch = SortBatchRepository.GetQueryable().FirstOrDefault(s => s.Id == sortBatchId);
                string orderDate = sortBatch.OrderDate.ToString("yyyyMMdd");
                //优化的分拣线
                SortingLine sortingLine = SortingLineRepository.GetQueryable()
                                                               .FirstOrDefault(s => s.SortingLineCode == sortBatch.SortingLineCode);
                //可优化的分拣线烟道
                Channel[] channels = ChannelRepository.GetQueryable()
                                                      .Where(c => c.SortingLineCode == sortBatch.SortingLineCode && c.IsActive == "1")
                                                      .ToArray();

                //大小品种划分系数
                double channelAllotScale = sortingLine.ProductType == "1" ?
                                               Convert.ToDouble(SystemParameterRepository.GetQueryable()
                                                      .Where(s => s.ParameterName == "ChannelAllotScale")
                                                      .Select(s => s.ParameterValue)
                                                      .FirstOrDefault())
                                               : 0.0;
                //是否使用整件分拣线
                bool isUseWholePieceSortingLine = SortingLineRepository.GetQueryable().Where(s => s.ProductType == "3" && s.IsActive == "1").Count() > 0;

                string[] deliverLineCodes = GetDeliverLine(sortBatchId, sortingLine.ProductType);

                //分拣的主单信息
                SortOrder[] sortOrders = SortOrderRepository.GetQueryable()
                                                            .Where(s => s.OrderDate == orderDate && deliverLineCodes.Contains(s.DeliverLineCode))
                                                            .ToArray();

                //分拣的细单信息
                SortOrderDetail[] sortOrderDetails = GetSortOrderDetail(sortOrders, sortingLine.ProductType, isUseWholePieceSortingLine);
                if (sortOrderDetails.Length > 0)
                {
                    if (sortingLine.ProductType == "1")
                    {
                        StateTypeForProcessing(ps, "数据优化", new Random().Next(1, 2) + 10, "正在优化" + "分拣烟道", new Random().Next(1, 5));

                        ChannelAllotOptimize(ConnectionId, ps, cancellationToken, sortBatchId, sortOrderDetails, channels, channelAllotScale);
                    }
                    StateTypeForProcessing(ps, "数据优化", new Random().Next(1, 2) + 22, "正在拆分" + "分拣订单", new Random().Next(1, 5));
                    OrderSplitOptimize(ConnectionId, ps, cancellationToken, sortBatchId, deliverLineCodes, sortOrders, sortOrderDetails);
                    if (sortingLine.ProductType == "1" && sortingLine.ProductType == "3")
                    {
                        ChannelAllot[] channelAllots = ChannelAllotRepository.GetQueryable().Where(c => c.SortBatchId == sortBatchId).ToArray();
                        SortOrderAllotMaster[] sortOrderAllotMasters = SortOrderAllotMasterRepository.GetQueryable().Where(c => c.SortBatchId == sortBatchId).ToArray();
                        StateTypeForProcessing(ps, "数据优化", new Random().Next(1, 5) + 42, "正在优化" + "出烟分配", new Random().Next(1, 5));
                        OrderDetailSplitOptimize(ConnectionId, ps, cancellationToken, sortBatchId, deliverLineCodes, sortOrders, sortOrderDetails, channelAllots, sortOrderAllotMasters);

                        StateTypeForProcessing(ps, "数据优化", new Random().Next(1, 5) + 90, "正在优化" + "手工补货", new Random().Next(10, 55));
                        Channel[] mixChannels = channels.Where(c => c.ChannelType == "5").OrderBy(c => c.SortAddress).ToArray();
                        SortOrderAllotDetail[] sortOrderAllotDetails = SortOrderAllotDetailRepository.GetQueryable()
                                                                                                     .Where(c => c.SortOrderAllotMaster.SortBatchId == sortBatchId && c.Channel.ChannelType == "5")
                                                                                                     .OrderBy(s => s.SortOrderAllotMaster.PackNo)
                                                                                                     .ToArray();
                        HandSupplyOptimize(ConnectionId, ps, cancellationToken, sortBatchId, sortOrderAllotDetails, mixChannels);
                    }
                    ps.Messages.Clear();
                    ps.Messages.Add("优化成功！");
                    sortBatch.Status = "02";
                    SortBatchRepository.SaveChanges();
                    StateTypeForProcessing(ps, "优化完成", 100, "订单优化完成！", 100);
                }
                else
                {
                    ps.Messages.Add("没有可优化数据！");
                }
            }
            catch (Exception ex)
            {
                try
                {
                    int sortBatchId = Convert.ToInt32(id);
                    SortOrderAllotDetailRepository.GetQueryable()
                                                  .Where(c => c.SortOrderAllotMaster.SortBatchId == sortBatchId)
                                                  .Delete();
                    ChannelAllotRepository.GetQueryable()
                                          .Where(c => c.SortBatchId == sortBatchId).Delete();
                    SortOrderAllotMasterRepository.GetQueryable()
                                                  .Where(c => c.SortBatchId == sortBatchId).Delete();

                }
                catch (Exception ex2)
                {
                    ps.State = StateType.Error;
                    ps.Messages.Add("优化失败！原因：" + ex.Message + ex2.Message);
                    NotifyConnection(ps.Clone());
                }
                ps.State = StateType.Error;
                ps.Messages.Add("优化失败！原因：" + ex.Message);
                NotifyConnection(ps.Clone());
            }
        }

        private string[] GetDeliverLine(int sortBatchId, string productType)
        {
            if (productType == "1")
            {
                //正常分拣线
                return SortOrderDispatchRepository.GetQueryable()
                                                  .Where(s => s.SortBatchId == sortBatchId)
                                                  .OrderBy(s => s.DeliverLineNo)
                                                  .Select(s => s.DeliverLineCode)
                                                  .ToArray();
            }
            if (productType == "2")
            {
                //异型分拣线未实现 
                return SortOrderDispatchRepository.GetQueryable()
                                                 .Where(s => s.SortBatchYxId == sortBatchId)
                                                 .OrderBy(s => s.DeliverLineNo)
                                                 .Select(s => s.DeliverLineCode)
                                                 .ToArray();
            }
            if (productType == "3")
            {
                //整件分拣线未实现 
                return SortOrderDispatchRepository.GetQueryable()
                                                  .Where(s => s.SortBatchZjId == sortBatchId)
                                                  .OrderBy(s => s.DeliverLineNo)
                                                  .Select(s => s.DeliverLineCode)
                                                  .ToArray();
            }
            if (productType == "4")
            {
                //手工分拣线未实现 
                return null;
            }
            else
            {
                return null;
            }
        }

        private SortOrderDetail[] GetSortOrderDetail(SortOrder[] sortOrders, string productType, bool isUseWholePieceSortingLine)
        {
            if (productType == "1")
            {
                if (isUseWholePieceSortingLine)
                {
                    var orderIds = sortOrders.Select(s => s.OrderID);
                    var SortOrderDetail = SortOrderDetailRepository.GetQueryable()
                                                                   .Where(d => orderIds.Contains(d.OrderID) && d.Product.IsAbnormity == "0")
                                                                   .ToArray();
                    SortOrderDetail.AsParallel().ForAll(d => d.SortQuantity %= 50);
                    return SortOrderDetail.Where(d => d.SortQuantity > 0).ToArray();
                }
                else
                {
                    var orderIds = sortOrders.Select(s => s.OrderID);
                    return SortOrderDetailRepository.GetQueryable()
                                                    .Where(d => orderIds.Contains(d.OrderID) && d.Product.IsAbnormity == "0")
                                                    .ToArray();
                }
            }
            if (productType == "2")
            {
                var orderIds = sortOrders.Select(s => s.OrderID);
                return SortOrderDetailRepository.GetQueryable()
                                                    .Where(d => orderIds.Contains(d.OrderID) && d.Product.IsAbnormity == "1")
                                                    .ToArray();
            }
            if (productType == "3")
            {
                var orderIds = sortOrders.Select(s => s.OrderID);
                var SortOrderDetail = SortOrderDetailRepository.GetQueryable()
                                                                   .Where(d => orderIds.Contains(d.OrderID) && d.Product.IsAbnormity == "0")
                                                                   .ToArray();
                SortOrderDetail.AsParallel().ForAll(d => d.SortQuantity = d.SortQuantity / 50 * 50);
                return SortOrderDetail.Where(d => d.SortQuantity > 0).ToArray();
            }
            if (productType == "4")
            {
                var orderIds = sortOrders.Select(s => s.OrderID);
                var SortOrderDetail = SortOrderDetailRepository.GetQueryable()
                                                                   .Where(d => orderIds.Contains(d.OrderID) && d.Product.IsAbnormity == "0")
                                                                   .ToArray();
                SortOrderDetail.AsParallel().ForAll(d => d.SortQuantity = d.RealQuantity - d.SortQuantity);
                return SortOrderDetail.Where(d => d.SortQuantity > 0).ToArray();
            }
            else
            {
                return null;
            }
        }

        private void ChannelAllotOptimize(string connectionId, ProgressState ps, CancellationToken cancellationToken, int sortBatchId, SortOrderDetail[] sortOrderDetails, Channel[] channels, double channelAllotScale)
        {
            ConnectionId = connectionId;
            ps.State = StateType.Processing;
            NotifyConnection(ps.Clone());
            //分拣线分拣品牌及数量集
            var beAllotProducts = sortOrderDetails.GroupBy(a => new { a.ProductCode, a.ProductName })
                                                 .Select(s => new
                                                 {
                                                     s.Key.ProductCode,
                                                     s.Key.ProductName,
                                                     Quantity = s.Sum(a => a.SortQuantity)
                                                 })
                                                 .OrderByDescending(s => s.Quantity);

            decimal sumQuantity = beAllotProducts.Sum(s => s.Quantity);
            double middleQuantity = Convert.ToDouble(sumQuantity) * channelAllotScale;
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
                if (bigs.Sum(b => b.Value) <= Convert.ToDecimal(middleQuantity))
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

            var bigChannels = channels.Where(c => (c.ChannelType == "3" || c.ChannelType == "4"))
                .OrderBy(c => c.OrderNo);
            int canAllotBigChannelCount = bigChannels.Count();
            StateTypeForProcessing(ps, "数据优化", new Random().Next(1, 2) + 12, "正在优化" + "分拣烟道", new Random().Next(10, 20));
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
                    if (bigs[bigProductCodeArray[i]] > 1)
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
                StateTypeForProcessing(ps, "数据优化", new Random().Next(1, 2) + 14, "正在优化" + "分拣烟道", new Random().Next(20, 30));
                //大品种烟道优化（New）   采用大品种烟道组A,B(C...)平均分配原则
                var channelGroups = channels.Select(c => c.GroupNo).Distinct();
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
                if (bigs.Count > 0)
                {
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
                            var channelGroup = ChannelAllotRepository.GetQueryable().Where(c => c.ProductCode == tempProductCode).Select(c => c.Channel.GroupNo).Distinct();
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
                }
                StateTypeForProcessing(ps, "数据优化", new Random().Next(1, 2) + 15, "正在优化" + "分拣烟道", new Random().Next(30, 50));
               
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
                var canAllotMixChannels = channels.Where(c => c.ChannelType == "5").OrderBy(c => c.OrderNo);
                int canAllotMixChannelCount = canAllotMixChannels.Count();
                var canAllotSingleSmallChannels = channels.Where(c => c.ChannelType == "2").OrderBy(c => c.OrderNo);
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
                StateTypeForProcessing(ps, "数据优化", new Random().Next(1, 2) + 16, "正在优化" + "分拣烟道", new Random().Next(50, 70));
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
                StateTypeForProcessing(ps, "数据优化", new Random().Next(1, 2) + 17, "正在优化" + "分拣烟道", new Random().Next(70, 99));
                ChannelAllotRepository.SaveChanges();
            }
        }

        private void OrderSplitOptimize(string connectionId, ProgressState ps, CancellationToken cancellationToken, int sortBatchId, string[] deliverLineCodes, SortOrder[] sortOrders, SortOrderDetail[] sortOrderDetails)
        {
            int packNo = 0;
            int customerOrder = 0;
            int psCount = deliverLineCodes.Count();
            int PsTemp = 0;
            foreach (var deliverLineCode in deliverLineCodes)
            {
                int customerDeliverOrder = 0;
                var sortOrdersArray = sortOrders.Where(s => s.DeliverLineCode == deliverLineCode)
                                                .OrderBy(s => s.DeliverOrder)
                                                .ToArray();
                PsTemp += 1;
                StateTypeForProcessing(ps, "数据优化", 22 + (PsTemp * 10 / psCount), "正在拆分" + "分拣订单", new Random().Next(PsTemp * 70 / psCount, PsTemp * 70 / psCount) + new Random().Next(9, 10));
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
            ps.Messages.Clear();
            ps.Messages.Add("保存结果时间较长（2-3分钟），请耐心等待...");
            StateTypeForProcessing(ps, "数据优化", new Random().Next(1, 10) + 32, "正在保存" + "拆分结果", new Random().Next(85, 99));
            SortOrderAllotMasterRepository.SaveChanges();
        }

        private void OrderDetailSplitOptimize(string connectionId, ProgressState ps, CancellationToken cancellationToken, int sortBatchId, string[] deliverLineCodes, SortOrder[] sortOrders, SortOrderDetail[] sortOrderDetails, ChannelAllot[] channelAllots, SortOrderAllotMaster[] sortOrderAllotMasters)
        {
            var channelGroupInfos = channelAllots.Select(c => c.Channel.GroupNo)
                                                 .Distinct()
                                                 .Select(g => new ChannelGroupInfo { ChannelGroup = g, Quantity = 0 })
                                                 .ToArray();

            var channelAllotInfos = channelAllots.Select(c => new ChannelAllotInfo
            {
                Id = c.Id,
                ChannelCode = c.ChannelCode,
                GroupNo = c.Channel.GroupNo,
                OrderNo = c.Channel.OrderNo,
                ProductCode = c.ProductCode,
                Quantity = ((c.Quantity - c.Channel.RemainQuantity) / 50) * 50,
                RemainQuantity = c.Channel.RemainQuantity + ((c.Quantity - c.Channel.RemainQuantity) % 50),
                ChannelCapacity = c.Channel.ChannelCapacity
            })
                .ToArray();

            int psCount = deliverLineCodes.Count();
            int PsTemp = 0;

            foreach (var deliverLineCode in deliverLineCodes)
            {
                var sortOrdersArray = sortOrders.Where(s => s.DeliverLineCode == deliverLineCode)
                                                .OrderBy(s => s.DeliverOrder)
                                                .ToArray();
                PsTemp += 1;
                StateTypeForProcessing(ps, "数据优化", 47 + (PsTemp * 33 / psCount) + new Random().Next(1, 2), "正在优化" + "出烟分配", new Random().Next(PsTemp * 70 / psCount, PsTemp * 70 / psCount) + new Random().Next(9, 10));
                foreach (var sortOrder in sortOrdersArray)
                {
                    var packInfos = sortOrderAllotMasters.Where(s => s.OrderId == sortOrder.OrderID)
                                                   .OrderBy(s => s.PackNo)
                                                   .Select(s => new PackInfo { Id = s.Id, PackNo = s.PackNo, Quantity = s.Quantity, SortOrderAllot = s })
                                                   .ToArray();

                    var sortOrderDetailInfos = sortOrderDetails.Where(s => s.OrderID == sortOrder.OrderID && s.SortQuantity > 0)
                        .OrderByDescending(s => s.SortQuantity)
                        .Select(s => new SortOrderDetailInfo
                        {
                            ProductCode = s.ProductCode,
                            ProductName = s.ProductName,
                            Quantity = s.SortQuantity
                        })
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

                                        if (allotQuantity > 0 && channelAllotInfo.OrderNo == tmp2.Where(t => t.Quantity + t.RemainQuantity > 0).Min(t => t.OrderNo))
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
                                            packInfo.SortOrderAllot.SortOrderAllotDetails.Add(addSortOrderAllotDetail);
                                        }

                                        if (channelAllotInfo.RemainQuantity <= (channelAllotInfo.ChannelCapacity - 50) && channelAllotInfo.Quantity >= 50)
                                        {
                                            channelAllotInfo.RemainQuantity += 50;

                                            //补货任务生成
                                            SortSupply addSortSupply = new SortSupply();
                                            addSortSupply.SortBatchId = sortBatchId;
                                            addSortSupply.PackNo = packInfo.PackNo;
                                            addSortSupply.ChannelCode = channelAllotInfo.ChannelCode;
                                            addSortSupply.ProductCode = channelAllotInfo.ProductCode;
                                            addSortSupply.ProductName = sortOrderDetailInfo.ProductName;
                                            SortSupplyRepository.Add(addSortSupply);

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
            ps.Messages.Clear();
            ps.Messages.Add("保存结果时间较长（3-5分钟），请耐心等待...");
            StateTypeForProcessing(ps, "数据优化", new Random().Next(1, 9) + 80, "正在保存" + "分配结果", new Random().Next(80, 99));
            SortOrderAllotDetailRepository.SaveChanges();
        }

        private void HandSupplyOptimize(string ConnectionId, ProgressState ps, CancellationToken cancellationToken, int sortBatchId, SortOrderAllotDetail[] sortOrderAllotDetails, Channel[] mixChannels)
        {
            int supplyId = 0;
            int supplyBatch = 0;
            int tempQuantity = 0;
            string tempChannelCode = "";
            if (mixChannels.Count() == 0)
            {
                //没有手工补货优化 跳出
            }
            else
            {
                //只有一个混合烟道
                if (mixChannels.Count() == 1)
                {
                    supplyBatch = 1;
                    foreach (var sortOrderAllotDetail in sortOrderAllotDetails)
                    {
                        int quantity = sortOrderAllotDetail.Quantity;
                        while (quantity > 0)
                        {
                            tempQuantity += quantity;
                            HandSupply addHandSupply = new HandSupply();
                            addHandSupply.SortBatchId = sortBatchId;
                            addHandSupply.SupplyId = ++supplyId;
                            addHandSupply.ChannelCode = mixChannels[0].ChannelCode;
                            addHandSupply.PackNo = sortOrderAllotDetail.SortOrderAllotMaster.PackNo;
                            addHandSupply.ProductCode = sortOrderAllotDetail.ProductCode;
                            addHandSupply.ProductName = sortOrderAllotDetail.ProductName;

                            if (tempQuantity <= 25)
                            {
                                addHandSupply.Quantity = quantity;
                                addHandSupply.SupplyBatch = supplyBatch;
                                if (tempQuantity == 25)
                                {
                                    tempQuantity = 0;
                                    supplyBatch++;
                                }
                            }
                            else
                            {
                                addHandSupply.Quantity = quantity + 25 - tempQuantity;
                                addHandSupply.SupplyBatch = supplyBatch++;
                                tempQuantity = 0;
                            }
                            quantity -= addHandSupply.Quantity;
                            HandSupplyRepository.Add(addHandSupply);
                            SortOrderAllotDetail addSortOrderAllotDetail = new SortOrderAllotDetail();
                            addSortOrderAllotDetail.MasterId = sortOrderAllotDetail.MasterId;
                            addSortOrderAllotDetail.ChannelCode = addHandSupply.ChannelCode;
                            addSortOrderAllotDetail.ProductCode = addHandSupply.ProductCode;
                            addSortOrderAllotDetail.ProductName = addHandSupply.ProductName;
                            addSortOrderAllotDetail.Quantity = addHandSupply.Quantity;
                            SortOrderAllotDetailRepository.Add(addSortOrderAllotDetail);
                        }
                        SortOrderAllotDetailRepository.Delete(sortOrderAllotDetail);
                    }
                }
                else
                {
                    //mixChannelDic_1用于切换烟道  mixChannelDic_2用于切换补货批次
                    Dictionary<string, int> mixChannelDic_1 = new Dictionary<string, int>();
                    Dictionary<string, int> mixChannelDic_2 = new Dictionary<string, int>();
                    Dictionary<int, int> supplyBatchQuantity = new Dictionary<int, int>();
                    int changeQuantity = 25;
                    foreach (var mixChannel in mixChannels)
                    {
                        mixChannelDic_1.Add(mixChannel.ChannelCode, 0);
                        mixChannelDic_2.Add(mixChannel.ChannelCode, 0);
                    }
                    foreach (var sortOrderAllotDetail in sortOrderAllotDetails)
                    {
                        int quantity = sortOrderAllotDetail.Quantity;
                        while (quantity > 0)
                        {
                            string channelCode = mixChannelDic_1.OrderBy(m => m.Value).FirstOrDefault().Key;
                            if (mixChannelDic_1.Where(m => m.Value % 20 > 0).Count() > 0)
                            {
                                channelCode = mixChannelDic_1.First(m => m.Value % 20 > 0).Key;
                            }
                            if (tempChannelCode != channelCode)
                            {
                                ++supplyBatch;
                                supplyBatchQuantity.Add(supplyBatch, 0);
                            }
                            tempChannelCode = channelCode;
                            tempQuantity += quantity;
                            HandSupply addHandSupply = new HandSupply();
                            addHandSupply.SortBatchId = sortBatchId;
                            addHandSupply.SupplyId = ++supplyId;

                            addHandSupply.ChannelCode = channelCode;
                            addHandSupply.PackNo = sortOrderAllotDetail.SortOrderAllotMaster.PackNo;
                            addHandSupply.ProductCode = sortOrderAllotDetail.ProductCode;
                            addHandSupply.ProductName = sortOrderAllotDetail.ProductName;

                            if (supplyBatch > mixChannelDic_1.Count() * 2)
                            {
                                changeQuantity = 20;

                            }
                            //每20条烟切换出烟烟道
                            if (tempQuantity <= 20)
                            {
                                if (mixChannelDic_2[channelCode] + quantity <= changeQuantity)
                                {
                                    addHandSupply.Quantity = quantity;
                                    mixChannelDic_1[channelCode] += addHandSupply.Quantity;
                                    mixChannelDic_2[channelCode] += addHandSupply.Quantity;
                                    addHandSupply.SupplyBatch = supplyBatch > mixChannelDic_2.Count() && supplyBatchQuantity[supplyBatch - mixChannelDic_2.Count()] < changeQuantity ? supplyBatch - mixChannelDic_2.Count() : supplyBatch;
                                    if (mixChannelDic_2[channelCode] == changeQuantity)
                                    {
                                        mixChannelDic_2[channelCode] = 0;
                                    }
                                }
                                else
                                {
                                    addHandSupply.Quantity = changeQuantity - mixChannelDic_2[channelCode];
                                    mixChannelDic_1[channelCode] += addHandSupply.Quantity;
                                    mixChannelDic_2[channelCode] = mixChannelDic_2[channelCode] + addHandSupply.Quantity - changeQuantity;
                                    addHandSupply.SupplyBatch = supplyBatch > mixChannelDic_2.Count() && supplyBatchQuantity[supplyBatch - mixChannelDic_2.Count()] < changeQuantity ? supplyBatch - mixChannelDic_2.Count() : supplyBatch;
                                    tempQuantity = tempQuantity + addHandSupply.Quantity - quantity;
                                }
                                if (tempQuantity == 20)
                                {
                                    tempQuantity = 0;
                                }
                                quantity -= addHandSupply.Quantity;
                                supplyBatchQuantity[addHandSupply.SupplyBatch] += addHandSupply.Quantity;
                            }
                            else
                            {
                                int allotQuantity = quantity + 20 - tempQuantity;

                                if (mixChannelDic_2[channelCode] + allotQuantity <= changeQuantity)
                                {
                                    addHandSupply.Quantity = allotQuantity;
                                    mixChannelDic_1[channelCode] += addHandSupply.Quantity;
                                    mixChannelDic_2[channelCode] += addHandSupply.Quantity;
                                    addHandSupply.SupplyBatch = supplyBatch > mixChannelDic_2.Count() && supplyBatchQuantity[supplyBatch - mixChannelDic_2.Count()] < changeQuantity ? supplyBatch - mixChannelDic_2.Count() : supplyBatch;
                                    if (mixChannelDic_2[channelCode] == changeQuantity)
                                    {
                                        mixChannelDic_2[channelCode] = 0;
                                    }
                                }
                                else
                                {
                                    addHandSupply.Quantity = changeQuantity - mixChannelDic_2[channelCode];
                                    mixChannelDic_1[channelCode] += addHandSupply.Quantity;
                                    mixChannelDic_2[channelCode] = mixChannelDic_2[channelCode] + addHandSupply.Quantity - changeQuantity;
                                    addHandSupply.SupplyBatch = supplyBatch > mixChannelDic_2.Count() && supplyBatchQuantity[supplyBatch - mixChannelDic_2.Count()] < changeQuantity ? supplyBatch - mixChannelDic_2.Count() : supplyBatch;
                                    tempQuantity = tempQuantity + addHandSupply.Quantity - quantity;
                                }
                                if (tempQuantity >= 20)
                                {
                                    tempQuantity = 0;
                                }
                                quantity -= addHandSupply.Quantity;
                                supplyBatchQuantity[addHandSupply.SupplyBatch] += addHandSupply.Quantity;
                            }
                            HandSupplyRepository.Add(addHandSupply);
                            //调整后更新细单的实际出烟位置
                            SortOrderAllotDetail addSortOrderAllotDetail = new SortOrderAllotDetail();
                            addSortOrderAllotDetail.MasterId = sortOrderAllotDetail.MasterId;
                            addSortOrderAllotDetail.ChannelCode = addHandSupply.ChannelCode;
                            addSortOrderAllotDetail.ProductCode = addHandSupply.ProductCode;
                            addSortOrderAllotDetail.ProductName = addHandSupply.ProductName;
                            addSortOrderAllotDetail.Quantity = addHandSupply.Quantity;
                            SortOrderAllotDetailRepository.Add(addSortOrderAllotDetail);
                        }
                        SortOrderAllotDetailRepository.Delete(sortOrderAllotDetail);
                    }
                }
                HandSupplyRepository.SaveChanges();
                StateTypeForProcessing(ps, "数据优化", new Random().Next(6, 9) + 90, "正在优化" + "手工补货", new Random().Next(60, 98));
                //调整后更新实际烟道分配情况
                ChannelAllotRepository.GetQueryable()
                            .Where(s => s.Channel.ChannelType == "5").Delete();
                var handSupplys = HandSupplyRepository.GetQueryable()
                                                      .Where(h => h.SortBatchId == sortBatchId)
                                                      .GroupBy(h => new { h.ChannelCode, h.ProductCode, h.ProductName })
                                                      .Select(s => new
                                                      {
                                                          s.Key.ChannelCode,
                                                          s.Key.ProductCode,
                                                          s.Key.ProductName,
                                                          Quantity = s.Sum(a => a.Quantity)
                                                      })
                                                      .OrderByDescending(s => s.Quantity);
                foreach (var handSupply in handSupplys)
                {
                    ChannelAllot addChannelAllot = new ChannelAllot();
                    addChannelAllot.SortBatchId = sortBatchId;
                    addChannelAllot.ChannelCode = handSupply.ChannelCode;
                    addChannelAllot.ProductCode = handSupply.ProductCode;
                    addChannelAllot.ProductName = handSupply.ProductName;
                    addChannelAllot.Quantity = handSupply.Quantity;
                    ChannelAllotRepository.Add(addChannelAllot);
                }
                ChannelAllotRepository.SaveChanges();
            }
        }

        private void StateTypeForProcessing(ProgressState ps, string totalName, int totalValue, string currentName, int currentValue)
        {
            ps.State = StateType.Processing;
            ps.TotalProgressName = totalName;
            ps.TotalProgressValue = totalValue;
            ps.CurrentProgressName = currentName;
            ps.CurrentProgressValue = currentValue;
            NotifyConnection(ps.Clone());
        }
    }
}
