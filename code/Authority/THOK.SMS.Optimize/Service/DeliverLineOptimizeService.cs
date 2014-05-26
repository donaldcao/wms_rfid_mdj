using System;
using THOK.SMS.DbModel;
using THOK.SMS.Dal.Interfaces;
using THOK.SMS.Optimize.Interfaces;
using System.Linq;
using THOK.Common.Entity;
using THOK.Wms.Dal.Interfaces;
using THOK.Wms.DbModel;
using Microsoft.Practices.Unity;
using THOK.Authority.Dal.Interfaces;
using THOK.Authority.DbModel;

namespace THOK.SMS.Optimize.Service
{
    public class DeliverLineOptimizeService : ServiceBase<SortOrder>, IDeliverLineOptimizeService
    {
        [Dependency]
        public ISortOrderRepository SortOrderRepository { get; set; }

        [Dependency]
        public ISortOrderDetailRepository SortOrderDetailRepository { get; set; }

        [Dependency]
        public IProductRepository ProductRepository { get; set; }

        [Dependency]
        public ISystemParameterRepository SystemParameterRepository { get; set; }

        [Dependency]
        public IDeliverLineAllotRepository DeliverLineAllotRepository { get; set; }

        protected override Type LogPrefix
        {
            get { return this.GetType(); }
        }

        public bool OptimizeAllot(int batchNo, out string strResult)
        {
            strResult = "";
            return true;
        }

        public object GetOrderInfo()
        {
            IQueryable<SortOrder> sortOrderQuery = SortOrderRepository.GetQueryable();
            IQueryable<SortOrderDetail> sortOrderDetailQuery = SortOrderDetailRepository.GetQueryable();
            IQueryable<Product> product=ProductRepository.GetQueryable();
            IQueryable<SystemParameter> systemParameterQuery = SystemParameterRepository.GetQueryable();
            var parameterValue = systemParameterQuery.FirstOrDefault(s => s.ParameterName.Equals("IsWarehousSortIntegration")).ParameterValue;
            var sortOrderDetails = (sortOrderQuery.GroupBy(s => s.OrderDate)
                .Select(s => new
                {
                    OrderDate = s.Key,
                    Explanation = s.Key + "分拣订单信息",
                    SumQuantity = s.Sum(x => x.QuantitySum),
                    AbnormityQuantity = sortOrderDetailQuery
                        .Where(d => (sortOrderQuery.Where(a => a.OrderDate.Equals(s.Key)).Select(a => a.OrderID)).Contains(d.OrderID)
                                && (product.Where(p => p.IsAbnormity.Equals("1")).Select(p => p.ProductCode)).Contains(d.ProductCode))
                        .Sum(a => a.DemandQuantity),
                    DeliverLineCount = sortOrderQuery.Where(a => a.OrderDate.Equals(s.Key)).GroupBy(a => a.DeliverLineCode).Count(),
                    IsWarehousSortIntegration = (parameterValue == null || parameterValue != "1") ? "否" : "是"
                })).ToArray();
            return new { total=sortOrderDetails.Count(),rows=sortOrderDetails.ToArray()};
        }

        public object GetDeliverLine(string orderDate)
        {
            IQueryable<SortOrder> sortOrderQuery = SortOrderRepository.GetQueryable();
            var sortOrderDetails = sortOrderQuery.Where(s => s.OrderDate.Equals(orderDate))
                .GroupBy(s =>new {s.DeliverLineCode,s.DeliverLine.DeliverLineName,s.DeliverLine.DistCode,s.DeliverLine.DeliverOrder} )
                .AsEnumerable()
                .Select(s => new
                {
                    DeliverLineCode = s.Key.DeliverLineCode,
                    DeliverLineName=s.Key.DeliverLineName,
                    Quantity=s.Sum(a=>a.QuantitySum),
                    DistCode=s.Key.DistCode,
                    State = GetDeliverLineAllotState(s.Key.DeliverLineCode),
                    DeliverOrder=s.Key.DeliverOrder
                }).ToArray();
            return sortOrderDetails;
        }

        private string GetDeliverLineAllotState(string deliverLineCode)
        {
            string state = "";
            IQueryable<DeliverLineAllot> deliverLineAllotQuery = DeliverLineAllotRepository.GetQueryable();
            var deliverLineState = deliverLineAllotQuery.Where(a => a.DeliverLineCode.Contains(deliverLineCode)).Select(a => a.Status).ToArray();
            if (deliverLineState.Count() > 0)
            {
                state = deliverLineState[0].ToString() == "01" ? "已分配" : "未分配";
            }
            else
            {
                state = "未分配";
            }
            return state;
        }
    }
}
