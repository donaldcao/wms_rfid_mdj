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
using THOK.SMS.Bll.Interfaces;

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

        [Dependency]
        public IDeliverLineRepository DeliverLineRepository { get; set; }

        [Dependency]
        public ISortOrderDispatchRepository SortOrderDispatchRepository { get; set; }

        [Dependency]
        public IUserRepository UserRepository { get; set; }

        [Dependency]
        public IBatchService BatchService { get; set; }

        [Dependency]
        public IBatchRepository BatchRepository { get; set; }

        [Dependency]
        public IBatchSortService BatchSortService { get; set; }

        [Dependency]
        public IBatchSortRepository BatchSortRepository { get; set; }

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
                    IsWarehousSortIntegration = parameterValue ??"0"
                })).ToArray();
            return new { total=sortOrderDetails.Count(),rows=sortOrderDetails.ToArray()};
        }

        public object GetDeliverLine(string orderDate)
        {
            IQueryable<SortOrder> sortOrderQuery = SortOrderRepository.GetQueryable();
            var sortOrderDetails = sortOrderQuery.Where(s => s.OrderDate.Equals(orderDate))
                .GroupBy(s =>new {s.DeliverLineCode,s.DeliverLine.DeliverLineName,s.DeliverLine.DistCode,s.DeliverLine.DeliverOrder} )
                .AsEnumerable()
                .OrderBy(a=>a.Key.DeliverOrder)
                .Select(s => new
                {
                    DeliverLineCode = s.Key.DeliverLineCode,
                    DeliverLineName=s.Key.DeliverLineName,
                    Quantity=s.Sum(a=>a.QuantitySum),
                    DistCode=s.Key.DistCode,
                    State = GetDeliverLineAllotState(s.Key.DeliverLineCode)[0],
                    SortingLineCode = GetDeliverLineAllotState(s.Key.DeliverLineCode)[1],
                    DeliverOrder=s.Key.DeliverOrder
                }).ToArray();
            return sortOrderDetails;
        }

        public object GetUnAllotDeliverLine(string orderDate)
        {
            IQueryable<SystemParameter> systemParameterQuery = SystemParameterRepository.GetQueryable();
            IQueryable<SortOrder> sortOrderQuery = SortOrderRepository.GetQueryable();
            var parameterValue = systemParameterQuery.FirstOrDefault(s => s.ParameterName.Equals("IsWarehousSortIntegration")).ParameterValue;
            if (parameterValue == "1")//仓储分拣一体化
            {
                IQueryable<SortOrderDispatch> sortOrderDispatchQuery = SortOrderDispatchRepository.GetQueryable();
                var sortOrderDispatchDetails = sortOrderDispatchQuery.Where(a => a.OrderDate.Equals(orderDate))
                    .AsEnumerable()
                    .OrderBy(a=>a.DeliverLine.DeliverOrder)
                    .Select(a => new
                    {
                        a.ID,
                        a.DeliverLineCode,
                        a.DeliverLine.DeliverLineName,
                        Quantity = sortOrderQuery.Where(b => b.OrderDate.Equals(orderDate) && b.DeliverLineCode.Equals(a.DeliverLineCode)).GroupBy(b => b.DeliverLineCode).Select(b => b.Sum(x => x.QuantitySum)),
                        State = GetDeliverLineAllotState(a.DeliverLineCode)[0],
                        SortingLineCode = GetDeliverLineAllotState(a.DeliverLineCode)[1],
                        DeliverOrder = a.DeliverLine.DeliverOrder
                    }).ToArray();
                return sortOrderDispatchDetails;
            }
            return null;
        }

        private string[] GetDeliverLineAllotState(string deliverLineCode)
        {
            string[] array = new string[2];
            IQueryable<DeliverLineAllot> deliverLineAllotQuery = DeliverLineAllotRepository.GetQueryable();
            var deliverLineState = deliverLineAllotQuery.Where(a => a.DeliverLineCode.Contains(deliverLineCode)).Select(a => new { a.Status, a.batchSort.SortingLineCode }).ToArray();
            if (deliverLineState.Count() > 0)
            {
                array[0] = deliverLineState[0].ToString() == "01" ? "已分配" : "未分配";
                array[1] = deliverLineState[0].Status == "01" ? deliverLineState[0].SortingLineCode : "";
            }
            else
            {
                array[0] = "未分配";
                array[1] = "";
            }
            return array;
        }

        public bool EditDeliverLine(DeliverLine deliverLine,out string strResult)
        {
            strResult = string.Empty;
            bool result=false;
            DeliverLine deliver = DeliverLineRepository.GetQueryable().FirstOrDefault(a => a.DeliverLineCode.Equals(deliverLine.DeliverLineCode));
            if (deliver!=null)
            {
                try
                {
                    deliver.DeliverOrder = deliverLine.DeliverOrder;
                    DeliverLineAllotRepository.SaveChanges();
                    result = true;
                }
                catch (Exception ex)
                {
                    strResult = "原因：" + ex.Message;
                }
            }
            return result;
        }

        public bool UpdateDeliverLineAllot(string orderDate, string deliverLineCodes,string userName,out string strResult)
        {
            strResult = string.Empty;
            bool bResult = false;
            DateTime date = DateTime.ParseExact(orderDate, "yyyyMMdd", System.Globalization.CultureInfo.CurrentCulture);
            string[] deliverLineCodeArray = deliverLineCodes.Substring(0, deliverLineCodes.Length - 1).Split(',').ToArray();
            IQueryable<SortOrderDispatch> sortOrderDispatchQuery = SortOrderDispatchRepository.GetQueryable();
            var sortOrderDispatchDetails = sortOrderDispatchQuery.Where(a => deliverLineCodeArray.Contains(a.DeliverLineCode)&& a.OrderDate.Equals(orderDate)&&a.IsActive.Equals("1"))
                .Select(a => new
                {
                    a.OrderDate,
                    a.DeliverLineCode,
                    a.SortingLineCode
                }).ToArray();
            if (sortOrderDispatchDetails.Count() > 0)
            {
                bool result;
                //更新批次信息表
                var batchNoArray = BatchRepository.GetQueryable().Where(a => a.OrderDate.Equals(date))
                    .OrderByDescending(a => a.BatchNo)
                    .Select(a => a.BatchNo).ToArray();
                int batchNo = batchNoArray.Count() > 0 ? batchNoArray[0] + 1 : 1;
                Batch batch = new Batch();
                batch.OrderDate = date;
                batch.BatchNo = batchNo;
                batch.BatchName = orderDate + "第" + batchNo + "批次";
                batch.OperateDate = date.AddDays(1);
                batch.OptimizeSchedule = 2;
                batch.Description = "1";
                batch.Status = "01";
                result=BatchService.Add(batch, userName, out strResult);
                if (!result)
                {
                    return false;
                }
                //更新批次分拣表
                var sortingLineCodes = sortOrderDispatchDetails.GroupBy(a => a.SortingLineCode).Select(a => a.Key).ToArray();
                IQueryable<Batch> batchQuery=BatchRepository.GetQueryable();
                var batchId = batchQuery.FirstOrDefault(a => a.BatchNo.Equals(batchNo) && a.OrderDate.Equals(date)).BatchId;
                foreach (var item in sortingLineCodes)
                {
                    BatchSort batchSort = new BatchSort();
                    batchSort.BatchId = batchId;
                    batchSort.SortingLineCode = item;
                    batchSort.Status = "01";
                    result = BatchSortService.Add(batchSort, out strResult);
                    if (!result)
                    {
                        return false;
                    }
                }
                //更新配送线路分配表
                IQueryable<BatchSort> batchSortQuery = BatchSortRepository.GetQueryable();
                var batchSortDetails = batchSortQuery.Where(a => a.BatchId.Equals(batchId)).Select(a => new { a.BatchSortId, a.SortingLineCode }).ToArray();
                IQueryable<SortOrder> sortOrderQuery = SortOrderRepository.GetQueryable();
                var sortOrderDetails = sortOrderQuery.Where(s => s.OrderDate.Equals(orderDate))
                    .GroupBy(s => s.DeliverLineCode)
                    .Select(s => new
                    {
                        DeliverLineCode = s.Key,
                        Quantity = s.Sum(a => a.QuantitySum),
                    }).ToArray();
                foreach (var item in deliverLineCodeArray)
                {
                    int batchSortId=batchSortDetails.FirstOrDefault(a=>a.SortingLineCode.Equals(item)).BatchSortId;
                    DeliverLineAllot deliverLineAllot = new DeliverLineAllot();
                    deliverLineAllot.DeliverLineAllotCode = batchSortId.ToString() + "-" + item;
                    deliverLineAllot.BatchSortId = batchSortId;
                    deliverLineAllot.DeliverLineCode = item;
                    deliverLineAllot.DeliverQuantity = Convert.ToInt32(sortOrderDetails.FirstOrDefault(a => a.DeliverLineCode.Equals(item)).Quantity);
                    deliverLineAllot.Status = "01";
                }

            }
            return bResult;
        }
    }
}
