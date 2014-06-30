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
using System.Collections.Generic;

namespace THOK.SMS.Optimize.Service
{
    public class OrderOptimizeService : ServiceBase<SortOrder>, IOrderOptimizeService
    {
        [Dependency]
        public ISortBatchRepository SortBatchRepository { get; set; }

        [Dependency]
        public ISortingLineRepository SortingLineRepository { get; set; }

        [Dependency]
        public ISortOrderRepository SortOrderRepository { get; set; }

        [Dependency]
        public ISortOrderDetailRepository SortOrderDetailRepository { get; set; }

        [Dependency]
        public ISortOrderAllotMasterRepository SortOrderAllotMasterRepository { get; set; }

        [Dependency]
        public ISortOrderAllotDetailRepository SortOrderAllotDetailRepository { get; set; }

        [Dependency]
        public IChannelRepository ChannelRepository { get; set; }

        [Dependency]
        public IChannelAllotRepository ChannelAllotRepository { get; set; }

        [Dependency]
        public ISystemParameterRepository SystemParameterRepository { get; set; }

        //[Dependency]
        //public IDeliverLineAllotRepository DeliverLineAllotRepository { get; set; }

        protected override Type LogPrefix
        {
            get { return this.GetType(); }
        }

        public bool OrderAllotOptimize(string orderDate, out string strResult)
        {
            //var sortBatchQuery = SortBatchRepository.GetQueryable();
            //var deliverLineAllot = DeliverLineAllotRepository.GetQueryable();
            //var channelAllotQuery = ChannelAllotRepository.GetQueryable();
            //var sortOrderQuery = SortOrderRepository.GetQueryable();
            //var sortOrderDetailQuery = SortOrderDetailRepository.GetQueryable();

            //DateTime date = DateTime.ParseExact(orderDate, "yyyyMMdd", System.Globalization.CultureInfo.CurrentCulture);
            //var sortBatchIds = sortBatchQuery.Where(a => a.batch.OrderDate == date)
            //                               .Select(b => b.SortBatchId)
            //                               .Distinct()
            //                               .ToArray();

            ////遍历每条批次分拣线
            //foreach (int sortBatchId in sortBatchIds)
            //{
            //    var deliverLines = deliverLineAllot.Where(a => a.SortBatchId == sortBatchId)
            //                                       .Select(a => a.DeliverLineCode);

            //    var sortOrders = sortOrderQuery.Where(s => s.OrderDate == orderDate && deliverLines.Contains(s.DeliverLineCode))
            //                                      .OrderBy(s => s.DeliverLine.DeliverOrder)
            //                                      .ThenBy(s => s.DeliverOrder)
            //                                      .ToArray();

            //    var channelAllots = channelAllotQuery.Where(c => c.SortBatchId == sortBatchId)
            //                                         .ToArray();

            //    Dictionary<string, int> channelRemainQuantity = channelAllots.ToDictionary(c => c.ChannelAllotCode, c => c.RealQuantity);

            //    int packNo = 0;

            //    int customerOrder = 0;
            //    int customerDeliverOrder = 0;

                
            //    string tempDeliverLine = "";    //当前配送线路
            //    string tempOrderId = "";        //当前订单编号

            //    foreach (var sortOrder in sortOrders)
            //    {
            //        var sortOrderDetails = sortOrderDetailQuery.Where(s => s.OrderID == sortOrder.OrderID)
            //                                                   .OrderByDescending(s => s.RealQuantity)
            //                                                   .ToArray();

            //        int orderQuantity = Convert.ToInt32(sortOrderDetails.Sum(c => c.RealQuantity));

            //        #region  主单拆包

            //        Dictionary<int, int> BagQuantity = new Dictionary<int, int>();

            //        int splitQuantity = 25;
            //        int bagCount = orderQuantity / splitQuantity;
            //        if (orderQuantity % splitQuantity > 0 && orderQuantity % splitQuantity < 5 && orderQuantity > splitQuantity)
            //        {
            //            for (int i = 1; i <= bagCount - 1; i++)
            //            {
            //                BagQuantity.Add(++packNo, splitQuantity);
            //            }
            //            BagQuantity.Add(++packNo, splitQuantity - 5);
            //            BagQuantity.Add(++packNo, orderQuantity % splitQuantity + 5);
            //        }
            //        else
            //        {
            //            for (int i = 1; i <= bagCount; i++)
            //            {
            //                BagQuantity.Add(++packNo, splitQuantity);
            //            }
            //            if (orderQuantity % splitQuantity > 0)
            //            {
            //                BagQuantity.Add(++packNo, orderQuantity % splitQuantity);
            //            }
            //        }

            //        string deliverLineCode = sortOrder.DeliverLineCode;
            //        if (deliverLineCode != tempDeliverLine)
            //        {
            //            customerDeliverOrder = 0;
            //            tempDeliverLine = deliverLineCode;
            //        }

            //        customerOrder += 1;
            //        customerDeliverOrder += 1;

            //        foreach (var item in BagQuantity)
            //        {
            //            SortOrderAllotMaster addSortOrderAllotMaster = new SortOrderAllotMaster();
            //            addSortOrderAllotMaster.SortBatchId = sortBatchId;
            //            addSortOrderAllotMaster.OrderId = sortOrder.OrderID;
            //            addSortOrderAllotMaster.PackNo = item.Key;
            //            addSortOrderAllotMaster.CustomerOrder = customerOrder;
            //            addSortOrderAllotMaster.CustomerDeliverOrder = customerDeliverOrder;
            //            addSortOrderAllotMaster.ExportNo = 0;
            //            addSortOrderAllotMaster.Quantity = item.Value;
            //            addSortOrderAllotMaster.StartTime = DateTime.Now;
            //            addSortOrderAllotMaster.FinishTime = DateTime.Now;
            //            addSortOrderAllotMaster.Status = "01";
            //            addSortOrderAllotMaster.OrderMasterCode = sortBatchId + "-" + item.Key;
            //            SortOrderAllotMasterRepository.Add(addSortOrderAllotMaster);
            //        }                    

            //        #endregion

            //        #region 明细拆包

            //        //foreach (var sortOrderDetail in sortOrderDetails)
            //        //{
            //        //    int quantity = Convert.ToInt32(sortOrderDetail.RealQuantity);

            //        //    while (quantity > 0)
            //        //    {
            //        //        int bagQuantity = BagQuantity[BagQuantity.Min(b => b.Key)];
            //        //        int allotQuantity = quantity > bagQuantity ? bagQuantity : quantity;

            //        //        BagQuantity[BagQuantity.Min(b => b.Key)] -= allotQuantity;
            //        //        quantity -= allotQuantity;

            //        //        int detailPackNo = BagQuantity.Min(b => b.Key);

            //        //        if (BagQuantity[BagQuantity.Min(b => b.Key)] == 0)
            //        //        {
            //        //            BagQuantity.Remove(BagQuantity.Min(b => b.Key));
            //        //        }
                            
            //        //        string productCode = sortOrderDetail.ProductCode;

            //        //        while (allotQuantity > 0)
            //        //        {
            //        //            int realAllotQuantity = 0;
            //        //            var channelAllotCodes = channelAllots.Where(c => c.ProductCode.Equals(productCode))
            //        //                                                 .Select(c => c.ChannelAllotCode);

            //        //            var productRemainQuantity = channelRemainQuantity.Where(c => channelAllotCodes.Contains(c.Key))
            //        //                                                             .OrderByDescending(g => g.Value);

            //        //            var realAllotChannel = productRemainQuantity.Where(p => p.Value % 50 > 0);

            //        //            string channelAllotCode;
            //        //            int channelQuantity;
            //        //            if (realAllotChannel.Count() > 0)
            //        //            {
            //        //                channelAllotCode = realAllotChannel.First().Key;
            //        //                channelQuantity = realAllotChannel.First().Value;
            //        //            }
            //        //            else
            //        //            {
            //        //                channelAllotCode = productRemainQuantity.First().Key;
            //        //                channelQuantity = productRemainQuantity.First().Value;
            //        //            }

            //        //            realAllotQuantity = channelQuantity > allotQuantity ? allotQuantity : channelQuantity;

            //        //            allotQuantity -= realAllotQuantity;
            //        //            channelRemainQuantity[channelAllotCode] -= realAllotQuantity;

            //        //            if (channelRemainQuantity[channelAllotCode] == 0)
            //        //            {
            //        //                channelRemainQuantity.Remove(channelAllotCode);
            //        //            }

            //        //            SortOrderAllotDetail addSortOrderAllotDetail = new SortOrderAllotDetail();
            //        //            addSortOrderAllotDetail.OrderMasterCode = sortBatchId + "-" + detailPackNo;
            //        //            addSortOrderAllotDetail.OrderDetailCode = sortBatchId + "-" + detailPackNo + "-" + channelAllotCode;

            //        //            addSortOrderAllotDetail.ChannelCode = channelAllots.Where(c => c.ChannelAllotCode == channelAllotCode)
            //        //                                                               .Select(c => c.ChannelCode)
            //        //                                                               .First();                                
            //        //            addSortOrderAllotDetail.ProductCode = productCode;
            //        //            addSortOrderAllotDetail.ProductName = channelAllots.Where(c => c.ChannelAllotCode == channelAllotCode)
            //        //                                                               .Select(c => c.ProductName)
            //        //                                                               .First();
            //        //            addSortOrderAllotDetail.Quantity = realAllotQuantity;                                
            //        //            SortOrderAllotDetailRepository.Add(addSortOrderAllotDetail);
            //        //        }                             
            //        //    }
            //        //}
                    
            //        #endregion
            //    }
            //}

            //SortOrderAllotMasterRepository.SaveChanges();
            strResult = "false";
            return true;
        }
    }
}
