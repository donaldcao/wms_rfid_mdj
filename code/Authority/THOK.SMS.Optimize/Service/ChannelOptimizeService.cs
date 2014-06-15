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
    public class ChannelOptimizeService : ServiceBase<SortOrder>, IChannelOptimizeService
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

        public bool ChannelAllotOptimize(string orderDate ,out string strResult)
        {
            DateTime date = DateTime.ParseExact(orderDate, "yyyyMMdd", System.Globalization.CultureInfo.CurrentCulture);
            IQueryable<BatchSort> batchsortquery = BatchSortRepository.GetQueryable();
            var batchsorts = batchsortquery.Where(a => a.batch.OrderDate == date).Select(b => b.BatchSortId).Distinct().ToArray();
            IQueryable<SystemParameter> systemParameterQuery = SystemParameterRepository.GetQueryable();
            //大小品种烟道划分比例
            double ChannelAllotScale = Convert.ToDouble(systemParameterQuery.FirstOrDefault(s => s.ParameterName.Equals("ChannelAllotScale")).ParameterValue);
            foreach (int batchSortCode in batchsorts)//遍历每条批次分拣线
            {
                IQueryable<ChannelAllot> channelallotquery = ChannelAllotRepository.GetQueryable();
                if (channelallotquery.Where(a => a.BatchSortId == batchSortCode).Count()>0)
                {
                    continue;
                }
                var deliverLines = DeliverLineAllotRepository.GetQueryable()
                    .Where(a => a.BatchSortId == batchSortCode)
                    .Select(a=>a.DeliverLineCode).ToArray();
                if (deliverLines.Count() > 0)
                {
                    IQueryable<SortOrderDetail> sortorderdetailquery = SortOrderDetailRepository.GetQueryable();
                    //分拣线分拣品牌及数量集
                    var beAllotProducts = sortorderdetailquery
                        .Where(a => deliverLines.Contains(a.SortOrder.DeliverLineCode))
                        .GroupBy(a => new { a.ProductCode, a.ProductName })
                        .Select(s => new
                        {
                            s.Key.ProductCode,
                            s.Key.ProductName,
                            Quantity = s.Sum(a => a.RealQuantity)
                        }).OrderByDescending(s => s.Quantity);
                    decimal sumQuantity = beAllotProducts.Sum(s => s.Quantity);
                    double middleQuantity = Convert.ToDouble(sumQuantity) * ChannelAllotScale;
                    Dictionary<string, decimal> bigs = new Dictionary<string, decimal>();
                    Dictionary<string, decimal> smalls = new Dictionary<string, decimal>();
                    string[] bigProductCodeArray = new string[beAllotProducts.Count()];
                    string[] smallProductCodeArray = new string[beAllotProducts.Count()];
                    decimal tempQuantity = 0;
                    int itemCount = 0;
                    int bigCount = 0;
                    //按划分系数划分大小品牌
                    foreach (var item in beAllotProducts)
                    {
                        itemCount++;
                        tempQuantity += item.Quantity;
                        if (tempQuantity <= Convert.ToDecimal(middleQuantity))
                        {
                            bigCount++;
                            bigProductCodeArray[itemCount - 1] = item.ProductCode;
                            bigs.Add(item.ProductCode, item.Quantity);
                        }
                        else
                        {
                            //smallProductCodeArray[itemCount - 1] = item.ProductCode;
                            smalls.Add(item.ProductCode, item.Quantity);
                        }
                    }
                    Dictionary<string, decimal> bigQuantitys = new Dictionary<string, decimal>();
                    foreach (var item in bigs)
                    {
                        bigQuantitys.Add(item.Key, item.Value);
                    }

                    string sortLineCode = batchsortquery.Where(b => b.BatchSortId == batchSortCode).Select(b => b.SortingLineCode).FirstOrDefault();
                    IQueryable<Channel> channelquery = ChannelRepository.GetQueryable();
                    var canAllotBigChannels = channelquery.Where(c => (c.ChannelType == "3" || c.ChannelType == "4") && c.SortingLineCode.Equals(sortLineCode) && c.Status=="1")
                        .OrderBy(c => c.OrderNo);
                    int canAllotBigChannelCount = canAllotBigChannels.Count();
                    //按可用大品牌烟道进行二次划分
                    if (canAllotBigChannelCount < bigCount)
                    {
                        for (int i = canAllotBigChannelCount; i < bigCount; i++)
                        {
                            smallProductCodeArray[i] = bigProductCodeArray[i];
                            bigProductCodeArray[i] = string.Empty;
                        }
                    }
                    string[] canAllChannelProducts = new string[canAllotBigChannelCount];
                    //依据品牌比重分配烟道
                    decimal tempSumChannelCount = 0;
                    foreach (string productCode in bigProductCodeArray)
                    {
                        if (productCode != null)
                        {
                            bigs[productCode] =bigs[productCode] * canAllotBigChannelCount / sumQuantity > 1 ? Convert.ToInt32(bigs[productCode] * canAllotBigChannelCount / sumQuantity) : 1; ;
                            tempSumChannelCount += bigs[productCode];
                        }
                    }
                    if (tempSumChannelCount>canAllotBigChannelCount)
                    {
                        for (int i = 0; i <= tempSumChannelCount - canAllotBigChannelCount - 1; i++)
                        {
                            bigs[bigProductCodeArray[i]] = bigs[bigProductCodeArray[i]] - 1;
                        }
                    }
                    else if (tempSumChannelCount<canAllotBigChannelCount)
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
                    if (tempChannelCount == canAllotBigChannelCount)//验证分配是否正确
                    {
                        //继续进行大品种单品牌多烟道数量拆分
                        foreach (var beAllotBigChannel in canAllotBigChannels)
                        {
                            ChannelAllot addChannelAllot = new ChannelAllot();
                            addChannelAllot.BatchSortId = batchSortCode;
                            addChannelAllot.ChannelCode = beAllotBigChannel.ChannelCode;
                            addChannelAllot.batchSort = batchsortquery.Where(b => b.BatchSortId == batchSortCode).FirstOrDefault();
                            addChannelAllot.channel = channelquery.Where(c => c.ChannelCode == beAllotBigChannel.ChannelCode).FirstOrDefault();
                            addChannelAllot.InQuantity = 0;
                            addChannelAllot.OutQuantity = 0;
                            addChannelAllot.RemainQuantity = 0;
                            string productCode = null;
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
                            if (productCode == null)
                            {
                                strResult = "Error：无品牌可分配   无法继续执行！";
                                return false;
                            }
                            string productName = beAllotProducts.Where(p => p.ProductCode == productCode).Select(p => p.ProductName).FirstOrDefault();
                            addChannelAllot.ProductCode = productCode;
                            addChannelAllot.ProductName = productName;
                            addChannelAllot.RealQuantity = quantity;
                            addChannelAllot.ChannelAllotCode = batchSortCode + "-" + beAllotBigChannel.ChannelCode + "-" + productCode;
                            ChannelAllotRepository.Add(addChannelAllot);
                            ChannelAllotRepository.SaveChanges();
                        }
                        //小品种增加大品种零烟
                        foreach (var item in bigQuantitys)
                        {
                            if (item.Value > 0)
                            {
                                smalls.Add(item.Key, item.Value);
                            }
                        }
                        smalls = smalls.OrderByDescending(s => s.Value).ToDictionary(s => s.Key, s => s.Value);
                        int smallCount = 0;
                        foreach (var item in smalls)
                        {
                            smallCount++;
                            smallProductCodeArray[itemCount - 1] = item.Key;
                        }
                        //按划分系数划分大小品牌
                        var canAllotMixChannels = channelquery.Where(c => (c.ChannelType == "5") && c.SortingLineCode.Equals(sortLineCode) && c.Status == "1")
                        .OrderBy(c => c.OrderNo);
                        int canAllotMixChannelCount = canAllotMixChannels.Count();
                        var canAllotSingleSmallChannels = channelquery.Where(c => (c.ChannelType == "2") && c.SortingLineCode.Equals(sortLineCode) && c.Status == "1")
                        .OrderBy(c => c.OrderNo);
                        int canAllotSmallChannelCount = canAllotSingleSmallChannels.Count();
                        if (canAllotMixChannelCount == 0 && canAllotSmallChannelCount < smallCount)
                        {
                            strResult = "Error：没有足够的烟道可以分配  无法继续执行！";
                            return false;
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
                            addChannelAllot.BatchSortId = batchSortCode;
                            addChannelAllot.ChannelCode = beAllotSingleChannel.ChannelCode;
                            addChannelAllot.batchSort = batchsortquery.Where(b => b.BatchSortId == batchSortCode).FirstOrDefault();
                            addChannelAllot.channel = channelquery.Where(c => c.ChannelCode == beAllotSingleChannel.ChannelCode).FirstOrDefault();
                            addChannelAllot.InQuantity = 0;
                            addChannelAllot.OutQuantity = 0;
                            addChannelAllot.RemainQuantity = 0;
                            string productName = productCode == null ? null : beAllotProducts.Where(p => p.ProductCode == productCode).Select(p => p.ProductName).FirstOrDefault();
                            addChannelAllot.ProductCode = productCode;
                            addChannelAllot.ProductName = productName;
                            addChannelAllot.RealQuantity = quantity;
                            addChannelAllot.ChannelAllotCode = batchSortCode + "-" + beAllotSingleChannel.ChannelCode + "-" + productCode;
                            ChannelAllotRepository.Add(addChannelAllot);
                            ChannelAllotRepository.SaveChanges();
                        }
                        //混合烟道优化分配
                        if (smalls.Count > 0)
                        {
                            if (canAllotMixChannelCount > 0)
                            {
                                Dictionary<string, int> mixChannelQuantity = new Dictionary<string, int>();
                                foreach (var channel in canAllotMixChannels)
                                {
                                    string channelCode = channel.ChannelCode;
                                    mixChannelQuantity.Add(channelCode, 0);
                                }
                                foreach (var small in smalls)
                                {
                                    if (small.Value > 0)
                                    {

                                        var channel = mixChannelQuantity.OrderBy(m => m.Value).ToDictionary(m => m.Key, m => m.Value).First();
                                        string channelCode = channel.Key;
                                        string productCode = small.Key;
                                        int quantity = Convert.ToInt32(small.Value);
                                        mixChannelQuantity[channelCode] += quantity;
                                        ChannelAllot addChannelAllot = new ChannelAllot();
                                        addChannelAllot.BatchSortId = batchSortCode;
                                        addChannelAllot.ChannelCode = channelCode;
                                        addChannelAllot.batchSort = batchsortquery.Where(b => b.BatchSortId == batchSortCode).FirstOrDefault();
                                        addChannelAllot.channel = channelquery.Where(c => c.ChannelCode == channelCode).FirstOrDefault();
                                        addChannelAllot.InQuantity = 0;
                                        addChannelAllot.OutQuantity = 0;
                                        addChannelAllot.RemainQuantity = 0;
                                        string productName = productCode == null ? null : beAllotProducts.Where(p => p.ProductCode == productCode).Select(p => p.ProductName).FirstOrDefault();
                                        addChannelAllot.ProductCode = productCode;
                                        addChannelAllot.ProductName = productName;
                                        addChannelAllot.RealQuantity = quantity;
                                        addChannelAllot.ChannelAllotCode = batchSortCode + "-" + channelCode + "-" + productCode;
                                        ChannelAllotRepository.Add(addChannelAllot);
                                        ChannelAllotRepository.SaveChanges();
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        strResult = "Error：烟道分配出错  无法继续执行！";
                        return false;
                    }
                }
            }
            strResult = "false";
            return true;
        }
        /// <summary>
        /// 获取日期批次下的分拣线信息  
        /// </summary>
        /// <param name="orderDate"></param>
        /// <returns></returns>
        public object GetBatchSort(string orderDate)
        {
            DateTime date = DateTime.ParseExact(orderDate, "yyyyMMdd", System.Globalization.CultureInfo.CurrentCulture);
            IQueryable<BatchSort> batchsortquery = BatchSortRepository.GetQueryable();
            IQueryable<Batch> batchquery = BatchRepository.GetQueryable(); 
            IQueryable<SortingLine> sortlingquery = SortingLineRepository.GetQueryable();
            var batchsorts = batchsortquery.Where(a=> a.batch.OrderDate==date).Select(b=>new
            {
                b.BatchId,
                b.BatchSortId,
                b.batch.BatchName,
                b.batch.BatchNo,
                b.batch.OrderDate,
                b.SortingLineCode,
                b.Status
            });
            var batchsort = batchsorts.OrderBy(a => a.BatchSortId).ToArray().Select(b => new
                {
                    b.BatchId,
                    b.BatchSortId,
                    b.BatchName,
                    b.BatchNo,
                    OrderDate = b.OrderDate.ToString("yyyy-MM-dd"),
                    b.SortingLineCode,
                    SortingLineName = sortlingquery.Where(a => a.SortingLineCode == b.SortingLineCode).Select(a => a.SortingLineName),
                    SortingLineType = sortlingquery.Where(a => a.SortingLineCode == b.SortingLineCode).Select(a => a.SortingLineType == "1" ? "分拣线" : "其他"),
                    b.Status
                });
            return batchsort;
        }

        public object GetChannelAllot(string batchSortId)
        {
            IQueryable<ChannelAllot> channelallotquery = ChannelAllotRepository.GetQueryable();
            int bSortId=Convert.ToInt32(batchSortId);
            var channelallots = channelallotquery.Where(c => c.BatchSortId == bSortId).OrderBy(c=>c.channel.OrderNo).ToArray();
            return channelallots;
        }
    }
}
