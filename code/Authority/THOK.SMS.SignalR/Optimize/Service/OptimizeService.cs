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
    public class OptimizeService : Notifier<OptimizeSortOrderConnection>, IOptimizeService
    {
        [Dependency]
        public ISystemParameterRepository SystemParameterRepository { get; set; }


        [Dependency]
        public ISortBatchRepository SortBatchRepository { get; set; }

        [Dependency]
        public ISortingLineRepository SortingLineRepository { get; set; }

        [Dependency]
        public IChannelRepository ChannelRepository { get; set; }


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

        public void Optimize(int sortBatchID)
        {
            var systemParameterQuery = SystemParameterRepository.GetQueryable();

            var sortBatchQuery = SortBatchRepository.GetQueryable();
            var sortingLineQuery = SortingLineRepository.GetQueryable();
            var channelQuery = ChannelRepository.GetQueryable();

            var sortOrderDispatchQuery = SortOrderDispatchRepository.GetQueryable();
            var sortOrderQuery = SortOrderRepository.GetQueryable();
            var sortOrderDetailQuery = SortOrderDetailRepository.GetQueryable();

            var channelAllotQuery = ChannelAllotRepository.GetQueryable();
            var sortOrderAllotMasterQuery = SortOrderAllotMasterRepository.GetQueryable();

            //优化的分拣批次
            var sortBatch = sortBatchQuery.FirstOrDefault(s => s.Id == sortBatchID);
            //优化的分拣线
            var sortingLine = sortingLineQuery.FirstOrDefault(s => s.SortingLineCode == sortBatch.SortingLineCode);
            //优化的分拣线的可用烟道
            var channel = channelQuery.Where(c => c.SortingLineCode == sortingLine.SortingLineCode && c.IsActive == "1");

            //是否使用整件线
            bool isUseWholePieceSortingLine = sortingLineQuery.Where(s => s.ProductType == "3").Count() > 0;

            //大小品种烟道划分比例
            double channelAllotScale = Convert.ToDouble(systemParameterQuery
                                           .Where(s => s.ParameterName == "ChannelAllotScale")
                                           .Select(s => s.ParameterValue).FirstOrDefault());

            //烟道优化后结果
            var channelAllot = channelAllotQuery.Where(c => c.SortBatchId == sortBatch.Id);
            //主单拆完后的结果
            var sortOrderAllot = sortOrderAllotMasterQuery.Where(s => s.SortBatchId == sortBatch.Id);

            if (sortingLine.ProductType == "1" && !isUseWholePieceSortingLine)//正常分拣线(包含整件)
            {
                var sortOrderInfo = sortOrderDispatchQuery.Where(s => s.SortBatchId == sortBatchID)
                        .Join(sortOrderQuery,
                            sod => new { sod.OrderDate, sod.DeliverLineCode },
                            so => new { so.OrderDate, so.DeliverLineCode },
                            (sod, so) => new { sod, so })
                        .Join(sortOrderDetailQuery.Where(s => s.Product.IsAbnormity == "0"),
                            t => t.so.OrderID,
                            sod => sod.OrderID,
                            (t, sod) => new { SortOrderDispatch = t.sod, SortOrder = t.so, SortOrderDetail = sod })
                        .OrderBy(t => t.SortOrder.DeliverLine.DeliverOrder)
                        .ThenBy(t => t.SortOrder.DeliverOrder)
                        .ToArray();

                return;
            }

            if (sortingLine.ProductType == "1" && isUseWholePieceSortingLine)//正常分拣线(排除整件)
            {
                var sortOrderInfo = sortOrderDispatchQuery.Where(s => s.SortBatchId == sortBatchID)
                        .Join(sortOrderQuery,
                            sod => new { sod.OrderDate, sod.DeliverLineCode },
                            so => new { so.OrderDate, so.DeliverLineCode },
                            (sod, so) => new { sod, so })
                        .Join(sortOrderDetailQuery.Where(s => s.Product.IsAbnormity == "0"),
                            t => t.so.OrderID,
                            sod => sod.OrderID,
                            (t, sod) => new { SortOrderDispatch = t.sod, SortOrder = t.so, SortOrderDetail = sod })
                        .OrderBy(t => t.SortOrder.DeliverLine.DeliverOrder)
                        .ThenBy(t => t.SortOrder.DeliverOrder)
                        .ToArray();

                sortOrderInfo.AsParallel().ForAll(s => s.SortOrderDetail.SortQuantity %= 50);

                return;
            }

            if (sortingLine.ProductType == "2")//异型分拣线
            {
                var sortOrderInfo = sortOrderDispatchQuery.Where(s => s.SortBatchAbnormalId == sortBatchID)
                        .Join(sortOrderQuery,
                            sod => new { sod.OrderDate, sod.DeliverLineCode },
                            so => new { so.OrderDate, so.DeliverLineCode },
                            (sod, so) => new { sod, so })
                        .Join(sortOrderDetailQuery.Where(s => s.Product.IsAbnormity == "1"),
                            t => t.so.OrderID,
                            sod => sod.OrderID,
                            (t, sod) => new { SortOrderDispatch = t.sod, SortOrder = t.so, SortOrderDetail = sod })
                        .OrderBy(t => t.SortOrder.DeliverLine.DeliverOrder)
                        .ThenBy(t => t.SortOrder.DeliverOrder)
                        .ToArray();

                return;
            }

            if (sortingLine.ProductType == "3")//整件分拣线
            {
                var sortOrderInfo = sortOrderDispatchQuery.Where(s => s.SortBatchPiecesId == sortBatchID)
                        .Join(sortOrderQuery,
                            sod => new { sod.OrderDate, sod.DeliverLineCode },
                            so => new { so.OrderDate, so.DeliverLineCode },
                            (sod, so) => new { sod, so })
                        .Join(sortOrderDetailQuery.Where(s => s.Product.IsAbnormity == "0"),
                            t => t.so.OrderID,
                            sod => sod.OrderID,
                            (t, sod) => new { SortOrderDispatch = t.sod, SortOrder = t.so, SortOrderDetail = sod })
                        .OrderBy(t => t.SortOrder.DeliverLine.DeliverOrder)
                        .ThenBy(t => t.SortOrder.DeliverOrder)
                        .ToArray();

                sortOrderInfo.AsParallel().ForAll(s => s.SortOrderDetail.SortQuantity = s.SortOrderDetail.SortQuantity / 50 * 50);

                return;
            }
            
            if (sortingLine.ProductType == "4")//手工分拣线
            {
                var sortOrderInfo = sortOrderDispatchQuery.Where(s => s.SortBatchManualId == sortBatchID)
                        .Join(sortOrderQuery,
                            sod => new { sod.OrderDate, sod.DeliverLineCode },
                            so => new { so.OrderDate, so.DeliverLineCode },
                            (sod, so) => new { sod, so })
                        .Join(sortOrderDetailQuery.Where(s => s.Product.IsAbnormity == "0"),
                            t => t.so.OrderID,
                            sod => sod.OrderID,
                            (t, sod) => new { SortOrderDispatch = t.sod, SortOrder = t.so, SortOrderDetail = sod })
                        .OrderBy(t => t.SortOrder.DeliverLine.DeliverOrder)
                        .ThenBy(t => t.SortOrder.DeliverOrder)
                        .ToArray();

                sortOrderInfo.AsParallel().ForAll(s => s.SortOrderDetail.SortQuantity = s.SortOrderDetail.RealQuantity - s.SortOrderDetail.SortQuantity);

                return;
            }
        }
    }
}
