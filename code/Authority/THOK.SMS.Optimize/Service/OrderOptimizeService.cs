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
        public IBatchRepository BatchRepository { get; set; }

        [Dependency]
        public IBatchSortRepository BatchSortRepository { get; set; }

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

        [Dependency]
        public IDeliverLineAllotRepository DeliverLineAllotRepository { get; set; }

        protected override Type LogPrefix
        {
            get { return this.GetType(); }
        }

        public bool OrderAllotOptimize(string orderDate, out string strResult)
        {

            DateTime date = DateTime.ParseExact(orderDate, "yyyyMMdd", System.Globalization.CultureInfo.CurrentCulture);
            IQueryable<BatchSort> batchsortquery = BatchSortRepository.GetQueryable();
            var batchsorts = batchsortquery
                .Where(a => a.batch.OrderDate == date)
                .Select(b => b.BatchSortId)
                .Distinct().ToArray();
            foreach (int batchSortCode in batchsorts)//遍历每条批次分拣线
            {
                var deliverLines = DeliverLineAllotRepository.GetQueryable()
                    .Where(a => a.BatchSortId == batchSortCode)
                    .Select(a => a.DeliverLineCode).ToArray();
                if (deliverLines.Count() > 0)
                {
                    IQueryable<SortOrder> sortorderquery = SortOrderRepository.GetQueryable();
                    //分拣线分拣品牌及数量集
                    var beAllotOrders = sortorderquery
                        .Where(s => s.OrderDate == orderDate && deliverLines.Contains(s.DeliverLineCode))
                        .OrderBy(s => s.DeliverLine.DeliverOrder);
                    IQueryable<ChannelAllot> channelallotquery = ChannelAllotRepository.GetQueryable();
                    var channelAllots = channelallotquery.Where(c => c.BatchSortId == batchSortCode);
                    Dictionary<string, int> channelRemainquantity = new Dictionary<string, int>();
                    foreach (var channelAllot in channelAllots)
                    {
                        channelRemainquantity.Add(channelAllot.ChannelAllotCode, channelAllot.RealQuantity);
                    }
                    //中间参数
                    int packNo = 0;
                    int customerOrder = 0;
                    int customerDeliverOrder = 0;
                    int detailPackNo = 0;
                    int tempBagOrder = 1;//当前第几包
                    string tempDeliverLine = "当前配送线路";
                    string tempOrderId = "当前订单编号";
                    foreach (var singleOrder in beAllotOrders)
                    {
                        string deliverLineCode = singleOrder.DeliverLineCode;
                        if (deliverLineCode != tempDeliverLine)
                        {
                            customerDeliverOrder += 1;
                            tempDeliverLine = deliverLineCode;
                        }
                        customerOrder += 1;
                        IQueryable<SortOrderDetail> sortorderdetailquery = SortOrderDetailRepository.GetQueryable();
                        var beAllotOrderDetails = sortorderdetailquery
                            .Where(s => s.OrderID == singleOrder.OrderID)
                            .OrderByDescending(s => s.RealQuantity);
                        int orderQuantity = Convert.ToInt32(beAllotOrderDetails.Sum(c => c.RealQuantity));
                        //主单拆包
                        Dictionary<int, int> BagQuantity = new Dictionary<int, int>();
                        int splitQuantity = 25;
                        if (orderQuantity % 25 > 0 && orderQuantity % 25 < 5)
                        {
                            splitQuantity = 20;
                        }
                        int bagCount = orderQuantity / 25;
                        if (orderQuantity % 25 == 0)
                        {
                            for (int i = 1; i <= bagCount; i++)
                            {
                                BagQuantity.Add(i, 25);
                            }
                        }
                        else
                        {
                            bagCount += 1;
                            if (bagCount == 1)
                            {
                                BagQuantity.Add(1, orderQuantity);
                            }
                            else if (bagCount == 2)
                            {
                                BagQuantity.Add(1, splitQuantity);
                                BagQuantity.Add(2, orderQuantity % splitQuantity);
                            }
                            else
                            {
                                for (int i = 1; i <= bagCount - 2; i++)
                                {
                                    BagQuantity.Add(i, 25);
                                }
                                BagQuantity.Add(bagCount - 1, splitQuantity);

                                BagQuantity.Add(bagCount, orderQuantity % 25 + (splitQuantity == 20 ? 5 : 0));
                            }
                        }
                        #region 遍历插入主表
                        //for (int i = 1; i <= bagCount; i++)
                        //{
                        //    packNo += 1;
                        //    SortOrderAllotMaster addSortOrderAllotMaster = new SortOrderAllotMaster();
                        //    addSortOrderAllotMaster.BatchSortId = batchSortCode;
                        //    addSortOrderAllotMaster.OrderId = singleOrder.OrderID;
                        //    addSortOrderAllotMaster.PackNo = packNo;
                        //    addSortOrderAllotMaster.CustomerOrder = customerOrder;
                        //    addSortOrderAllotMaster.CustomerDeliverOrder = customerDeliverOrder;
                        //    addSortOrderAllotMaster.ExportNo = 0;
                        //    addSortOrderAllotMaster.Quantity = BagQuantity[i];
                        //    addSortOrderAllotMaster.StartTime = DateTime.Now;
                        //    addSortOrderAllotMaster.FinishTime = DateTime.Now;
                        //    addSortOrderAllotMaster.Status = "01";
                        //    addSortOrderAllotMaster.OrderMasterCode = batchSortCode + "-" + packNo;
                        //    SortOrderAllotMasterRepository.Add(addSortOrderAllotMaster);
                        //}
                        #endregion
                        
                        //明细拆包
                        foreach (var singleOrderDetail in beAllotOrderDetails)
                        {
                            int quantity = Convert.ToInt32(singleOrderDetail.RealQuantity);
                            do
                            {
                                int allotQuantity = 0;
                                int bagOrder = BagQuantity.Min(b => b.Key);
                                if (tempOrderId != singleOrder.OrderID)
                                {
                                    detailPackNo += 1;
                                    tempOrderId = singleOrder.OrderID;
                                }
                                else if (tempBagOrder != bagOrder)
                                {
                                    detailPackNo += 1;
                                    tempBagOrder = bagOrder;
                                }
                                int bagQuantity = BagQuantity[BagQuantity.Min(b => b.Key)];
                                allotQuantity = quantity > bagQuantity ? bagQuantity : quantity;
                                BagQuantity[BagQuantity.Min(b => b.Key)] -= allotQuantity;
                                quantity -= allotQuantity;
                                if (BagQuantity[BagQuantity.Min(b => b.Key)] == 0)
                                {
                                    BagQuantity.Remove(BagQuantity.Min(b => b.Key));
                                }
                                string productCode = singleOrderDetail.ProductCode;
                                do
                                {
                                    int realAllotQuantity = 0;
                                    var channelAllotCodes = channelAllots
                                        .Where(c => c.ProductCode.Equals(productCode))
                                        .Select(c => c.ChannelAllotCode);
                                    var productRemainquantity = channelRemainquantity
                                        .Where(c => channelAllotCodes.Contains(c.Key))
                                        .OrderByDescending(g => g.Value)
                                        .ToDictionary(c => c.Key, c => c.Value);
                                    var realAllotChannel = productRemainquantity
                                        .Where(p => p.Value % 50 > 0);
                                    string channelAllotCode;
                                    int channelQuantity;
                                    if (realAllotChannel.Count() > 0)
                                    {
                                        channelAllotCode = realAllotChannel.First().Key;
                                        channelQuantity = realAllotChannel.First().Value;
                                    }
                                    else
                                    {
                                        channelAllotCode = productRemainquantity.First().Key;
                                        channelQuantity = productRemainquantity.First().Value;
                                    }
                                    realAllotQuantity = channelQuantity > allotQuantity ? allotQuantity : channelQuantity;
                                    allotQuantity -= realAllotQuantity;
                                    channelRemainquantity[channelAllotCode] -= realAllotQuantity;
                                    SortOrderAllotDetail addSortOrderAllotDetail = new SortOrderAllotDetail();
                                    addSortOrderAllotDetail.ChannelCode = channelAllots
                                        .Where(c => c.ChannelAllotCode == channelAllotCode)
                                        .Select(c => c.ChannelCode).First();
                                    addSortOrderAllotDetail.OrderMasterCode = batchSortCode + "-" + detailPackNo;
                                    addSortOrderAllotDetail.ProductCode = productCode;
                                    addSortOrderAllotDetail.ProductName = channelAllots
                                        .Where(c => c.ChannelAllotCode == channelAllotCode)
                                        .Select(c => c.ProductName).First();
                                    addSortOrderAllotDetail.Quantity = realAllotQuantity;
                                    addSortOrderAllotDetail.OrderDetailCode = batchSortCode + "-" + detailPackNo + "-" + productCode;
                                    SortOrderAllotDetailRepository.Add(addSortOrderAllotDetail);
                                } while (allotQuantity > 0);
                            } while (quantity > 0);
                        }
                    }
                }
            }
            //SortOrderAllotMasterRepository.SaveChanges();
            SortOrderAllotDetailRepository.SaveChanges();
            strResult = "false";
            return true;
        }
    }
}
