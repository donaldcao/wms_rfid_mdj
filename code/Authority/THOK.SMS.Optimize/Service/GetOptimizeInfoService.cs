using System;
using THOK.SMS.DbModel;
using THOK.SMS.Optimize.Interfaces;
using System.Linq;
using THOK.Wms.DbModel;
using System.Collections.Generic;
using Microsoft.Practices.Unity;
using THOK.SMS.Dal.Interfaces;
using THOK.Wms.Dal.Interfaces;
using THOK.Authority.Dal.Interfaces;

namespace THOK.SMS.Optimize.Service
{

    public class GetOptimizeInfoService : ServiceBase<SortOrder>, IGetOptimizeInfoService
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

        #endregion

        protected override Type LogPrefix
        {
            get { return this.GetType(); }
        }

        public SortBatch GetSortBatch(int sortBatchId)
        {
            return SortBatchRepository.GetQueryable().FirstOrDefault(s => s.Id == sortBatchId);
        }

        public SortingLine GetSortingLine(string sortingLineCode)
        {
            return SortingLineRepository.GetQueryable().FirstOrDefault(s => s.SortingLineCode == sortingLineCode);
        }

        public Channel[] GetChannel(string sortingLineCode)
        {
            return ChannelRepository.GetQueryable().Where(c => c.SortingLineCode == sortingLineCode && c.IsActive == "1").ToArray();
        }

        public bool GetIsUseWholePieceSortingLine()
        {
            return SortingLineRepository.GetQueryable().Where(s => s.ProductType == "3").Count() > 0;
        }

        public double GetChannelAllotScale()
        {
            try
            {
                return Convert.ToDouble(SystemParameterRepository.GetQueryable()
                                   .Where(s => s.ParameterName == "ChannelAllotScale")
                                   .Select(s => s.ParameterValue)
                                   .FirstOrDefault());
            }
            catch (Exception)
            {

                return 0.9;
            }

            
        }

        public string[] GetDeliverLine(int sortBatchId, string productType)
        {
            if (productType=="1")
            {
                //正常分拣线
                return SortOrderDispatchRepository.GetQueryable()
                                                  .Where(s => s.SortBatchId == sortBatchId)
                                                  .OrderBy(s => s.DeliverLineNo)
                                                  .Select(s=>s.DeliverLineCode)
                                                  .ToArray();
            }
            if (productType == "2")
            {
                //异型分拣线未实现 
                return null;
            }
            if (productType == "3")
            {
                //整件分拣线未实现 
                return null;
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

        public SortOrder[] GetSortOrder(string orderDate, string[] deliverLineCodes)
        {
            return SortOrderRepository.GetQueryable()
                                      .Where(s => s.OrderDate == orderDate && deliverLineCodes.Contains(s.DeliverLineCode))
                                      .ToArray();
        }

        public SortOrderDetail[] GetSortOrderDetail(SortOrder[] sortOrders, string productType, bool isUseWholePieceSortingLine)
        {
            if (productType =="1")
            {
                if (isUseWholePieceSortingLine)
                {
                    var orderIds = sortOrders.Select(s => s.OrderID);
                    var SortOrderDetail = SortOrderDetailRepository.GetQueryable()
                                                                   .Where(d => orderIds.Contains(d.OrderID) && d.Product.IsAbnormity == "0")
                                                                   .ToArray();
                    SortOrderDetail.AsParallel().ForAll(d => d.SortQuantity %= 50);
                    return SortOrderDetail.Where(d=> d.SortQuantity > 0).ToArray();
                }
                else
                {
                    var orderIds = sortOrders.Select(s => s.OrderID);
                    return SortOrderDetailRepository.GetQueryable()
                                                    .Where(d => orderIds.Contains(d.OrderID) && d.Product.IsAbnormity == "0")
                                                    .ToArray();
                }
            }
            if (productType =="2")
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

        public ChannelAllot[] GetChannelAllot(int sortBatchId)
        {
            return ChannelAllotRepository.GetQueryable().Where(c => c.SortBatchId == sortBatchId).ToArray();
        }

        public SortOrderAllotMaster[] GetSortOrderAllotMaster(int sortBatchId)
        {
            return SortOrderAllotMasterRepository.GetQueryable().Where(c => c.SortBatchId == sortBatchId).ToArray();
        }
    }
}
