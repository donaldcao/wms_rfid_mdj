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
using System.Transactions;

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

        [Dependency]
        public IDeliverLineAllotService DeliverLineAllotService { get; set; }

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
            IQueryable<Product> product = ProductRepository.GetQueryable();
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
                    IsWarehousSortIntegration = parameterValue ?? "0"
                })).ToArray();
            return new { total = sortOrderDetails.Count(), rows = sortOrderDetails.ToArray() };
        }

        public object GetDeliverLine(string orderDate)
        {
            IQueryable<SortOrder> sortOrderQuery = SortOrderRepository.GetQueryable();
            var sortOrderDetails = sortOrderQuery.Where(s => s.OrderDate.Equals(orderDate))
                .GroupBy(s => new { s.DeliverLineCode, s.DeliverLine.DeliverLineName, s.DeliverLine.DistCode, s.DeliverLine.DeliverOrder })
                .AsEnumerable()
                .OrderBy(a => a.Key.DeliverOrder)
                .Select(s => new
                {
                    DeliverLineCode = s.Key.DeliverLineCode,
                    DeliverLineName = s.Key.DeliverLineName,
                    Quantity = s.Sum(a => a.QuantitySum),
                    DistCode = s.Key.DistCode,
                    State = GetDeliverLineAllotState(s.Key.DeliverLineCode)[0],
                    SortingLineCode = GetDeliverLineAllotState(s.Key.DeliverLineCode)[1],
                    DeliverOrder = s.Key.DeliverOrder
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
                    .OrderBy(a => a.DeliverLine.DeliverOrder)
                    .Select(a => new
                    {
                        a.ID,
                        a.DeliverLineCode,
                        a.DeliverLine.DeliverLineName,
                        Quantity = sortOrderQuery.Where(b => b.OrderDate.Equals(orderDate) && b.DeliverLineCode.Equals(a.DeliverLineCode)).GroupBy(b => b.DeliverLineCode).Select(b => b.Sum(x => x.QuantitySum)),
                        State = GetDeliverLineAllotState(a.DeliverLineCode)[0],
                        SortingLineCode = GetDeliverLineAllotState(a.DeliverLineCode)[1],
                        DeliverOrder = a.DeliverLine.DeliverOrder
                    }).Where(a => a.State.Equals("未分配"))
                    .Select(a => a).ToArray();
                return sortOrderDispatchDetails;
            }
            else//只有分拣
            {
                var sortOrderDeliverLine = sortOrderQuery.Where(a => a.OrderDate.Equals(orderDate))
                    .GroupBy(a => new { a.DeliverLineCode, a.DeliverLine.DeliverLineName, a.DeliverLine.DeliverOrder })
                    .AsEnumerable()
                    .OrderBy(a => a.Key.DeliverOrder)
                    .Select(a => new
                    {
                        a.Key.DeliverLineCode,
                        a.Key.DeliverLineName,
                        Quantity = a.Sum(b => b.QuantitySum),
                        State = GetDeliverLineAllotState(a.Key.DeliverLineCode)[0],
                        SortingLineCode = GetDeliverLineAllotState(a.Key.DeliverLineCode)[1],
                        DeliverOrder = a.Key.DeliverOrder
                    }).Where(a => a.State.Equals("未分配"))
                    .Select(a => a).ToArray();
                return sortOrderDeliverLine;
                //IQueryable<DeliverLine> deliverLineQuery = DeliverLineRepository.GetQueryable();
            }
        }

        private string[] GetDeliverLineAllotState(string deliverLineCode)
        {
            string[] array = new string[2];
            IQueryable<DeliverLineAllot> deliverLineAllotQuery = DeliverLineAllotRepository.GetQueryable();
            var deliverLineState = deliverLineAllotQuery.Where(a => a.DeliverLineCode.Contains(deliverLineCode)).Select(a => new { a.Status, a.batchSort.SortingLineCode }).ToArray();
            if (deliverLineState.Count() > 0)
            {
                array[0] = deliverLineState[0].Status == "01" ? "已分配" : deliverLineState[0].Status == "01" ? "已优化" : "已完成";
                array[1] = deliverLineState[0].SortingLineCode;
            }
            else
            {
                array[0] = "未分配";
                array[1] = "";
            }
            return array;
        }

        public bool EditDeliverLine(DeliverLine deliverLine, out string strResult)
        {
            strResult = string.Empty;
            bool result = false;
            DeliverLine deliver = DeliverLineRepository.GetQueryable().FirstOrDefault(a => a.DeliverLineCode.Equals(deliverLine.DeliverLineCode));
            if (deliver != null)
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

        public bool UpdateDeliverLineAllot(string orderDate, string deliverLineCodes, string userName, out string strResult)
        {
            strResult = string.Empty;
            bool bResult = false;
            DateTime date = DateTime.ParseExact(orderDate, "yyyyMMdd", System.Globalization.CultureInfo.CurrentCulture);
            string[] deliverLineCodeArray = deliverLineCodes.Substring(0, deliverLineCodes.Length - 1).Split(',').ToArray();
            IQueryable<SortOrderDispatch> sortOrderDispatchQuery = SortOrderDispatchRepository.GetQueryable();
            var sortOrderDispatchDetails = sortOrderDispatchQuery.Where(a => deliverLineCodeArray.Contains(a.DeliverLineCode) && a.OrderDate.Equals(orderDate) && a.IsActive.Equals("1"))
                .Select(a => new
                {
                    a.OrderDate,
                    a.DeliverLineCode,
                    a.SortingLineCode
                }).ToArray();
            if (sortOrderDispatchDetails.Count() > 0)
            {
                using (var scope = new TransactionScope())
                {
                    //获取batchId
                    var sortingLineCodes = sortOrderDispatchDetails.GroupBy(a => a.SortingLineCode).Select(a => a.Key).ToArray();
                    int batchId = GetBatchId(date, orderDate, userName, sortingLineCodes, out strResult);
                    if (batchId == -1)
                    {
                        return false;
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
                        int batchSortId = batchSortDetails.FirstOrDefault(a => a.SortingLineCode.Equals(sortOrderDispatchDetails.FirstOrDefault(b => b.DeliverLineCode.Equals(item)).SortingLineCode)).BatchSortId;
                        DeliverLineAllot deliverLineAllot = new DeliverLineAllot();
                        deliverLineAllot.DeliverLineAllotCode = batchSortId.ToString() + "-" + item;
                        deliverLineAllot.BatchSortId = batchSortId;
                        deliverLineAllot.DeliverLineCode = item;
                        deliverLineAllot.DeliverQuantity = Convert.ToInt32(sortOrderDetails.FirstOrDefault(a => a.DeliverLineCode.Equals(item)).Quantity);
                        deliverLineAllot.Status = "01";
                        bool result = DeliverLineAllotService.Add(deliverLineAllot, out strResult);
                        if (!result)
                        {
                            return false;
                        }
                    }
                    bResult = true;
                    scope.Complete();
                }
            }
            return bResult;
        }

        public bool UpdateDeliverLineAllot(string orderDate, string deliverLineCodes, string sortingLineCode, string userName, out string strResult)
        {
            strResult = string.Empty;
            bool bResult = false;
            DateTime date = DateTime.ParseExact(orderDate, "yyyyMMdd", System.Globalization.CultureInfo.CurrentCulture);
            string[] deliverLineCodeArray = deliverLineCodes.Substring(0, deliverLineCodes.Length - 1).Split(',').ToArray();
            string[] sortingLineCodeArray = sortingLineCode.Substring(0, sortingLineCode.Length - 1).Split(',').ToArray();
            using (var scope = new TransactionScope())
            {
                //获取batchId
                int batchId = GetBatchId(date, orderDate, userName, sortingLineCodeArray, out strResult);
                if (batchId == -1)
                {
                    return false;
                }
                //更新配送线路分配表
                IQueryable<BatchSort> batchSortQuery = BatchSortRepository.GetQueryable();
                var batchSortDetails = batchSortQuery.Where(a => a.BatchId.Equals(batchId)).Select(a => new { a.BatchSortId, a.SortingLineCode }).ToArray();
                IQueryable<SortOrder> sortOrderQuery = SortOrderRepository.GetQueryable();
                var sortOrderDetails = sortOrderQuery.Where(s => s.OrderDate.Equals(orderDate))
                    .Where(a => deliverLineCodeArray.Contains(a.DeliverLineCode))
                    .GroupBy(s => s.DeliverLineCode)
                    .Select(s => new
                    {
                        DeliverLineCode = s.Key,
                        Quantity = s.Sum(a => a.QuantitySum),
                    }).OrderByDescending(a => a.Quantity).ToArray();
                IQueryable<DeliverLineAllot> deliverLineAllotQuery = DeliverLineAllotRepository.GetQueryable();
                for (int i = 0; i < sortOrderDetails.Count(); i++)
                {
                    var batchSortId = batchSortDetails
                        .Where(a => sortingLineCodeArray.Contains(a.SortingLineCode))
                        .Select(a => new
                        {
                            a.BatchSortId,
                            Quantity = deliverLineAllotQuery.GroupBy(b => b.BatchSortId).Select(b => new { b.Key, Quantity = b.Sum(c => c.DeliverQuantity) }).ToArray().FirstOrDefault(b => b.Key.Equals(a.BatchSortId)) == null ?
                                       0 : deliverLineAllotQuery.GroupBy(b => b.BatchSortId).Select(b => new { b.Key, Quantity = b.Sum(c => c.DeliverQuantity) }).ToArray().FirstOrDefault(b => b.Key.Equals(a.BatchSortId)).Quantity
                        }).ToArray().OrderBy(a => a.Quantity).FirstOrDefault().BatchSortId;
                    DeliverLineAllot deliverLineAllot = new DeliverLineAllot();
                    deliverLineAllot.DeliverLineAllotCode = batchSortId.ToString() + "-" + sortOrderDetails[i].DeliverLineCode;
                    deliverLineAllot.BatchSortId = batchSortId;
                    deliverLineAllot.DeliverLineCode = sortOrderDetails[i].DeliverLineCode;
                    deliverLineAllot.DeliverQuantity = Convert.ToInt32(sortOrderDetails.FirstOrDefault(a => a.DeliverLineCode.Equals(sortOrderDetails[i].DeliverLineCode)).Quantity);
                    deliverLineAllot.Status = "01";
                    bool result = DeliverLineAllotService.Add(deliverLineAllot, out strResult);
                    if (!result)
                    {
                        return false;
                    }
                }
                bResult = true;
                scope.Complete();
            }
            return bResult;
        }

        private int GetBatchId(DateTime date, string orderDate, string userName, Array sortingLineCodes, out string strResult)
        {
            strResult = string.Empty;
            var batchNoArray = BatchRepository.GetQueryable().Where(a => a.OrderDate.Equals(date))
                        .OrderByDescending(a => a.BatchNo)
                        .Select(a => new { a.BatchId, a.BatchNo, a.Status }).ToArray();
            int batchId;
            int batchNo = batchNoArray.Count() > 0 ? batchNoArray[0].BatchNo + 1 : 1;
            if (batchNoArray.FirstOrDefault(a => a.Status.Equals("01")) == null)//不存在初始化批次，重新加入一个批次
            {
                //更新批次信息表
                Batch batch = new Batch();
                batch.OrderDate = date;
                batch.BatchNo = batchNo;
                batch.BatchName = orderDate + "第" + batchNo + "批次";
                batch.OperateDate = date.AddDays(1);
                batch.OptimizeSchedule = 2;
                batch.Description = "1";
                batch.Status = "01";
                bool result = BatchService.Add(batch, userName, out strResult);
                if (!result)
                {
                    return -1;
                }
                IQueryable<Batch> batchQuery = BatchRepository.GetQueryable();
                batchId = batchQuery.FirstOrDefault(a => a.BatchNo.Equals(batchNo) && a.OrderDate.Equals(date)).BatchId;
            }
            else
            {
                batchId = batchNoArray.FirstOrDefault(a => a.Status.Equals("01")).BatchId;
            }
            //更新批次分拣表
            IQueryable<BatchSort> batchSortQuery = BatchSortRepository.GetQueryable();
            foreach (var item in sortingLineCodes)
            {
                var batchSortDetails = batchSortQuery.AsEnumerable().FirstOrDefault(a => a.BatchId.Equals(batchId) && a.SortingLineCode.Equals(item.ToString()));
                if (batchSortDetails == null)
                {
                    BatchSort batchSort = new BatchSort();
                    batchSort.BatchId = batchId;
                    batchSort.SortingLineCode = item.ToString();
                    batchSort.Status = "01";
                    bool result = BatchSortService.Add(batchSort, out strResult);
                    if (!result)
                    {
                        return -1;
                    }
                }
            }
            return batchId;
        }
    }
}