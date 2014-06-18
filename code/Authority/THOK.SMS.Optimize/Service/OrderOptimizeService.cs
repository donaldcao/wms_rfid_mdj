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
            var batchsorts = batchsortquery.Where(a => a.batch.OrderDate == date).Select(b => b.BatchSortId).Distinct().ToArray();
            
            foreach (int batchSortCode in batchsorts)//遍历每条批次分拣线
            {
                IQueryable<ChannelAllot> channelallotquery = ChannelAllotRepository.GetQueryable();
                //if (channelallotquery.Where(a => a.BatchSortId == batchSortCode).Count()>0)
                //{
                //    continue;
                //}
                var deliverLines = DeliverLineAllotRepository.GetQueryable()
                    .Where(a => a.BatchSortId == batchSortCode)
                    .Select(a=>a.DeliverLineCode).ToArray();
                if (deliverLines.Count() > 0)
                {
                    IQueryable<SortOrder> sortorderquery = SortOrderRepository.GetQueryable();
                    //分拣线分拣品牌及数量集
                    var beAllotOrders = sortorderquery
                        .Where(s => s.OrderDate == orderDate && deliverLines.Contains(s.DeliverLineCode))
                        .OrderBy(s => s.DeliverLine.DeliverOrder);
                    int packNo = 0;
                    int customerOrder = 0;
                    int customerDeliverOrder = 0;
                    string tempDeliverLine = "当前配送线路";
                    foreach (var singleOrder in beAllotOrders)
                    {
                        string deliverLineCode = singleOrder.DeliverLineCode;
                        if (deliverLineCode != tempDeliverLine)
                        {
                            customerDeliverOrder += 1;
                        }
                        IQueryable<SortOrderDetail> sortorderdetailquery = SortOrderDetailRepository.GetQueryable();
                        var beAllotOrderDetails = sortorderdetailquery.Where(s => s.OrderID == singleOrder.OrderID).OrderByDescending(s => s.RealQuantity);
                        int orderQuantity = Convert.ToInt32(beAllotOrderDetails.Sum(c => c.RealQuantity));
                        //主单拆包
                        Dictionary<int, int> BagQuantity = new Dictionary<int, int>();
                        int splitQuantity = 25;
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
                            if (orderQuantity % 25 > 0 && orderQuantity % 25 < 5)
                            {
                                splitQuantity = 20;
                            }

                            if (bagCount == 1)
                            {
                                BagQuantity.Add(1, orderQuantity);
                            }
                            else if (bagCount == 2)
                            {
                                BagQuantity.Add(1, splitQuantity);
                                if (orderQuantity % splitQuantity > 0)
                                {
                                    BagQuantity.Add(2, orderQuantity % splitQuantity);
                                }
                            }
                            else
                            {
                                for (int i = 1; i <= bagCount - 2; i++)
                                {
                                    BagQuantity.Add(i, 25);
                                }
                                BagQuantity.Add(bagCount - 1, splitQuantity);
                                BagQuantity.Add(bagCount, orderQuantity % splitQuantity);
                            }
                        }
                        for (int i = 1; i < BagQuantity.Count; i++)
                        {
                            packNo += 1;
                            SortOrderAllotMaster addSortOrderAllotMaster = new SortOrderAllotMaster();
                            addSortOrderAllotMaster.BatchSortId = batchSortCode;
                            addSortOrderAllotMaster.batchSort = batchsortquery.Where(b => b.BatchSortId == batchSortCode).FirstOrDefault();
                            addSortOrderAllotMaster.OrderId = singleOrder.OrderID;
                            addSortOrderAllotMaster.PackNo = packNo;
                            addSortOrderAllotMaster.CustomerOrder = customerOrder;
                            addSortOrderAllotMaster.CustomerDeliverOrder = customerDeliverOrder;
                            addSortOrderAllotMaster.ExportNo = 0;
                            addSortOrderAllotMaster.OrderId = singleOrder.OrderID;
                            addSortOrderAllotMaster.Quantity = BagQuantity[i];
                            addSortOrderAllotMaster.StartTime = DateTime.Now;
                            addSortOrderAllotMaster.FinishTime = DateTime.Now;
                            addSortOrderAllotMaster.Status = "01";
                            addSortOrderAllotMaster.OrderMasterCode = batchSortCode + "-" + packNo;
                            SortOrderAllotMasterRepository.Add(addSortOrderAllotMaster);
                            SortOrderAllotMasterRepository.SaveChanges();
                        }
                        
                        ////明细拆包
                        //foreach (var singleOrderDetail in beAllotOrderDetails)
                        //{
                        //    int middleQuantity = 0;
                        //    int quantity = Convert.ToInt32(singleOrderDetail.RealQuantity);
                        //    int bagQuantity = BagQuantity[BagQuantity.Min(b => b.Key)];
                        //    if (true)
                        //    {
                                
                        //    }
                        //}
                    }
                }
            }
            strResult = "false";
            return true;
        }
    }
}
