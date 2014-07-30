using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using THOK.SMS.SignalR.Connection;
using THOK.SMS.SignalR.Optimize.Interfaces;
using THOK.SMS.SignalR.Model;
using System.Threading;
using Microsoft.Practices.Unity;
using THOK.SMS.Optimize.Interfaces;
using THOK.Wms.DbModel;
using THOK.SMS.DbModel;

namespace THOK.SMS.SignalR.Optimize.Service
{
    public class OptimizeSortOrderService : Notifier<OptimizeSortOrderConnection>, IOptimizeSortOrderService
    {
        [Dependency]
        public IGetOptimizeInfoService GetOptimizeInfoService { get; set; }

        [Dependency]
        public IChannelOptimizeService ChannelOptimizeService { get; set; }

        [Dependency]
        public IOrderSplitOptimizeService OrderSplitOptimizeService { get; set; }
        
        public void Optimize(string connectionId, ProgressState ps, CancellationToken cancellationToken, string id)
        {
            ConnectionId = connectionId;
            ps.State = StateType.Start;
            ps.Messages.Add("开始优化");
            NotifyConnection(ps.Clone());
            Random random = new Random();
            try
            {
                StateTypeForProcessing(ps, "数据提取", new Random().Next(1, 5), "正在提取" + "批次信息", new Random().Next(1, 100));
                int sortBatchId = Convert.ToInt32(id);
                //优化的批次
                SortBatch sortBatch = GetOptimizeInfoService.GetSortBatch(sortBatchId);
                //优化的分拣线
                SortingLine sortingLine = GetOptimizeInfoService.GetSortingLine(sortBatch.SortingLineCode);
                //可优化的分拣线烟道
                Channel[] channels = GetOptimizeInfoService.GetChannel(sortBatch.SortingLineCode);

                //大小品种划分系数
                double channelAllotScale = sortingLine.ProductType=="1"? GetOptimizeInfoService.GetChannelAllotScale():0.0;
                //是否使用整件分拣线
                bool isUseWholePieceSortingLine = GetOptimizeInfoService.GetIsUseWholePieceSortingLine();

                StateTypeForProcessing(ps, "数据提取", new Random().Next(1, 5) + 5, "正在提取" + "线路信息", 100);
                string[] deliverLineCodes = GetOptimizeInfoService.GetDeliverLine(sortBatchId, sortingLine.ProductType);

                StateTypeForProcessing(ps, "数据提取", new Random().Next(1, 5) + 10, "正在提取" + "主单信息", new Random().Next(1, 100));
                //分拣的主单信息
                SortOrder[] sortOrders = GetOptimizeInfoService.GetSortOrder(sortBatch.OrderDate.ToString("yyyyMMdd"), deliverLineCodes);

                StateTypeForProcessing(ps, "数据提取", new Random().Next(1, 5) + 15, "正在提取" + "细单信息", 100);
                //分拣的细单信息
                SortOrderDetail[] sortOrderDetails = GetOptimizeInfoService.GetSortOrderDetail(sortOrders, sortingLine.ProductType, isUseWholePieceSortingLine);

                StateTypeForProcessing(ps, "数据优化", new Random().Next(1, 5) + 20, "正在优化" + "分拣烟道", new Random().Next(1, 100));
                ChannelOptimizeService.ChannelAllotOptimize(sortBatchId, sortOrderDetails, channels, channelAllotScale);
                StateTypeForProcessing(ps, "数据优化", new Random().Next(1, 5) + 30, "正在保存" + "分配结果", 100);
                ChannelAllot[] channelAllots = GetOptimizeInfoService.GetChannelAllot(sortBatchId);
                StateTypeForProcessing(ps, "数据优化", new Random().Next(1, 5) + 35, "正在拆分" + "分拣订单", new Random().Next(1, 100));
                OrderSplitOptimizeService.OrderSplitOptimize(sortBatchId, deliverLineCodes, sortOrders, sortOrderDetails);
                StateTypeForProcessing(ps, "数据优化", new Random().Next(1, 5) + 45, "正在保存" + "拆分结果", 100);
                SortOrderAllotMaster[] sortOrderAllotMasters = GetOptimizeInfoService.GetSortOrderAllotMaster(sortBatchId);
                StateTypeForProcessing(ps, "数据优化", new Random().Next(1, 5) + 65, "正在优化" + "出烟分配", new Random().Next(1, 100));
                OrderSplitOptimizeService.OrderDetailSplitOptimize(sortBatchId, deliverLineCodes, sortOrders, sortOrderDetails, channelAllots, sortOrderAllotMasters);
                StateTypeForProcessing(ps, "数据优化", new Random().Next(1, 5) + 90, "正在保存" + "优化结果", 100);
                StateTypeForInfo(ps, "优化成功！");
                StateTypeForProcessing(ps, "优化完成", 100,"订单优化完成！", 100);
                ps.State = StateType.Info;
                ps.Messages.Clear();
                ps.Messages.Add("优化成功！");
                NotifyConnection(ps.Clone());
            }
            catch (Exception ex)
            {

                StateTypeForInfo(ps, "优化出错！" + ex.Message);
            }
        }

        void StateTypeForInfo(ProgressState ps, string message)
        {
            ps.State = StateType.Info;
            ps.Messages.Add(message);
            NotifyConnection(ps.Clone());
        }

        void StateTypeForProcessing(ProgressState ps, string TotalName, int TotalValue, string CurrentName, int CurrentValue)
        {
            ps.State = StateType.Processing;
            ps.TotalProgressName = TotalName;
            ps.TotalProgressValue = TotalValue;
            ps.CurrentProgressName = CurrentName;
            ps.CurrentProgressValue = CurrentValue;
            ps.Messages.Add(CurrentName);
            NotifyConnection(ps.Clone());
        }
    }
}
