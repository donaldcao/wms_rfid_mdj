using System;
using THOK.SMS.DbModel;
using THOK.SMS.Optimize.Interfaces;
using System.Linq;
using THOK.Wms.DbModel;
using System.Collections.Generic;
using Microsoft.Practices.Unity;
using THOK.SMS.Dal.Interfaces;

namespace THOK.SMS.Optimize.Service
{
    public class ChannelOptimizeService : ServiceBase<SortOrder>, IChannelOptimizeService
    {
        [Dependency]
        public IChannelAllotRepository ChannelAllotRepository { get; set; }

        protected override Type LogPrefix
        {
            get { return this.GetType(); }
        }

        public void ChannelAllotOptimize(int sortBatchId, SortOrderDetail[] sortOrderDetails, Channel[] channels, double ChannelAllotScale)
        {
            //分拣线分拣品牌及数量集
            var beAllotProducts = sortOrderDetails.GroupBy(a => new { a.ProductCode, a.ProductName })
                                                 .Select(s => new
                                                 {
                                                     s.Key.ProductCode,
                                                     s.Key.ProductName,
                                                     Quantity = s.Sum(a => a.SortQuantity)
                                                 })
                                                 .OrderByDescending(s => s.Quantity);

            decimal sumQuantity = beAllotProducts.Sum(s => s.Quantity);
            double middleQuantity = Convert.ToDouble(sumQuantity) * ChannelAllotScale;
            //大小品牌分配数量（占用烟道数）
            Dictionary<string, decimal> bigs = new Dictionary<string, decimal>();
            Dictionary<string, decimal> smalls = new Dictionary<string, decimal>();
            //大小品牌
            string[] bigProductCodeArray = new string[beAllotProducts.Count()];
            string[] smallProductCodeArray = new string[beAllotProducts.Count()];

            int itemCount = 0;
            int bigCount = 0;

            //按划分系数划分大小品牌
            foreach (var item in beAllotProducts)
            {
                itemCount++;
                if (bigs.Sum(b => b.Value) <= Convert.ToDecimal(middleQuantity))
                {
                    bigCount++;
                    bigProductCodeArray[itemCount - 1] = item.ProductCode;
                    bigs.Add(item.ProductCode, item.Quantity);
                }
                else
                {
                    smalls.Add(item.ProductCode, item.Quantity);
                }
            }
            //大品牌数量（扣除分配数量用）
            Dictionary<string, decimal> bigQuantitys = new Dictionary<string, decimal>();
            foreach (var item in bigs)
            {
                bigQuantitys.Add(item.Key, item.Value);
            }

            var bigChannels = channels.Where(c => (c.ChannelType == "3" || c.ChannelType == "4"))
                .OrderBy(c => c.OrderNo);
            int canAllotBigChannelCount = bigChannels.Count();

            //按可用大品牌烟道进行二次划分
            if (canAllotBigChannelCount < bigCount)
            {
                for (int i = canAllotBigChannelCount; i < bigCount; i++)
                {
                    smallProductCodeArray[i] = bigProductCodeArray[i];
                    bigs.Remove(bigProductCodeArray[i]);
                    bigProductCodeArray[i] = null;
                }
            }

            string[] channelProducts = new string[canAllotBigChannelCount];
            //依据品牌比重分配烟道
            decimal tempSumChannelCount = 0;
            foreach (string productCode in bigProductCodeArray)
            {
                if (productCode != null)
                {
                    bigs[productCode] = bigs[productCode] * canAllotBigChannelCount / sumQuantity > 1 ? Convert.ToInt32(bigs[productCode] * canAllotBigChannelCount / sumQuantity) : 1;
                    tempSumChannelCount += bigs[productCode];
                }
            }
            //烟道占用数四舍五入分配后多还少补
            if (tempSumChannelCount > canAllotBigChannelCount)
            {
                for (int i = 0; i <= tempSumChannelCount - canAllotBigChannelCount - 1; i++)
                {
                    if (bigs[bigProductCodeArray[i]] > 1)
                    {
                        bigs[bigProductCodeArray[i]] -= 1;
                    }
                    else
                    {
                        bigs[bigs.First(b => b.Value == bigs.Max(m => m.Value)).Key] -= 1;
                    }
                }
            }
            else if (tempSumChannelCount < canAllotBigChannelCount)
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
            //验证分配是否正确  
            if (tempChannelCount != canAllotBigChannelCount)
            {
                throw new Exception("Error：大品种占用烟道数与实际不符！");
            }
            //是则继续进行大品种单品牌多烟道数量拆分
            else
            {
                //大品种烟道优化（New）   采用大品种烟道组A,B(C...)平均分配原则
                var channelGroups = channels.Select(c => c.GroupNo).Distinct();
                Dictionary<int, int> groupQuantity = new Dictionary<int, int>();
                Dictionary<int, int> groupCount = new Dictionary<int, int>();
                Dictionary<string, int> usedChannelGroupNo = new Dictionary<string, int>();
                foreach (var groupNo in channelGroups)
                {
                    groupQuantity.Add(groupNo, 0);
                    groupCount.Add(groupNo, 0);
                }
                //单品牌多烟道组保证不在同一烟道组
                string tempProductCode = "";
                while (bigs.Max(v => v.Value) > 0)
                {
                    string productCode = null;
                    bool isChangeProduct = true;
                    int groupNo = 0;
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
                    if (productCode == tempProductCode)
                    {
                        isChangeProduct = false;
                    }
                    tempProductCode = productCode;
                    //如果品牌未更换 要保证该品牌分配在多个烟道组上
                    if (!isChangeProduct)
                    {
                        var channelGroup = ChannelAllotRepository.GetQueryable().Where(c => c.ProductCode == tempProductCode).Select(c => c.channel.GroupNo).Distinct();
                        int tempCount = groupQuantity.Where(g => !channelGroup.Contains(g.Key)).Count();
                        if (tempCount > 0)
                        {
                            groupNo = groupQuantity.Where(g => !channelGroup.Contains(g.Key)).OrderBy(g => g.Value).ToDictionary(g => g.Key, g => g.Value).First().Key;
                        }
                        //如果已经保证多烟道组或者换品牌 该品牌分配数量用来调节两线平衡
                        else
                        {
                            groupNo = groupQuantity.OrderBy(g => g.Value).ToDictionary(g => g.Key, g => g.Value).First().Key;
                        }
                    }
                    else
                    {
                        groupNo = groupQuantity.OrderBy(g => g.Value).ToDictionary(g => g.Key, g => g.Value).First().Key;
                    }
                    //分配的组烟道不够用则切换至有空余烟道的组
                    if (groupCount[groupNo] == bigChannels.Where(c => c.GroupNo == groupNo).Count())
                    {
                        do
                        {
                            groupQuantity.Remove(groupNo);
                            groupNo = groupQuantity.OrderBy(g => g.Value).ToDictionary(g => g.Key, g => g.Value).First().Key;
                        } while (groupCount[groupNo] == bigChannels.Where(c => c.GroupNo == groupNo).Count());

                    }
                    //确认分配  往烟道分配表写数据
                    var beAllotBigChannel = bigChannels.Where(c => c.GroupNo == groupNo && (usedChannelGroupNo.Count == 0 || usedChannelGroupNo.Keys.Contains(c.ChannelCode) == false)).FirstOrDefault();
                    ChannelAllot addChannelAllot = new ChannelAllot();
                    addChannelAllot.SortBatchId = sortBatchId;
                    addChannelAllot.ChannelCode = beAllotBigChannel.ChannelCode;
                    string productName = beAllotProducts.Where(p => p.ProductCode == productCode).Select(p => p.ProductName).FirstOrDefault();
                    addChannelAllot.ProductCode = productCode;
                    addChannelAllot.ProductName = productName;
                    addChannelAllot.Quantity = quantity;
                    ChannelAllotRepository.Add(addChannelAllot);
                    usedChannelGroupNo.Add(beAllotBigChannel.ChannelCode, groupNo);
                    groupQuantity[groupNo] += quantity;
                    groupCount[groupNo] += 1;
                }
                ChannelAllotRepository.SaveChanges();

                //小品种增加大品种零烟
                foreach (var item in bigQuantitys)
                {
                    if (item.Value > 0)
                    {
                        smalls.Add(item.Key, item.Value);
                    }
                }
                smalls = smalls.OrderByDescending(s => s.Value).ToDictionary(s => s.Key, s => s.Value);

                //小品种烟道够不够用判断
                var canAllotMixChannels = channels.Where(c => c.ChannelType == "5").OrderBy(c => c.OrderNo);
                int canAllotMixChannelCount = canAllotMixChannels.Count();
                var canAllotSingleSmallChannels = channels.Where(c => c.ChannelType == "2").OrderBy(c => c.OrderNo);
                int canAllotSmallChannelCount = canAllotSingleSmallChannels.Count();
                if (canAllotMixChannelCount == 0 && canAllotSmallChannelCount < smalls.Count)
                {
                    throw new Exception("Error：小品种占用烟道数与实际不符！");
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
                    addChannelAllot.SortBatchId = sortBatchId;
                    addChannelAllot.ChannelCode = beAllotSingleChannel.ChannelCode;
                    string productName = productCode == null ? null : beAllotProducts.Where(p => p.ProductCode == productCode).Select(p => p.ProductName).FirstOrDefault();
                    addChannelAllot.ProductCode = productCode;
                    addChannelAllot.ProductName = productName;
                    addChannelAllot.Quantity = quantity;
                    ChannelAllotRepository.Add(addChannelAllot);
                }
                //混合烟道优化分配
                if (smalls.Count > 0)
                {
                    if (canAllotMixChannelCount > 0)
                    {
                        Dictionary<string, int> mixChannelQuantity = new Dictionary<string, int>();
                        foreach (var mixChannel in canAllotMixChannels)
                        {
                            string channelCode = mixChannel.ChannelCode;
                            mixChannelQuantity.Add(channelCode, 0);
                        }
                        foreach (var small in smalls)
                        {
                            if (small.Value > 0)
                            {

                                var mixChannel = mixChannelQuantity.OrderBy(m => m.Value).ToDictionary(m => m.Key, m => m.Value).First();
                                string channelCode = mixChannel.Key;
                                string productCode = small.Key;
                                int quantity = Convert.ToInt32(small.Value);
                                mixChannelQuantity[channelCode] += quantity;
                                ChannelAllot addChannelAllot = new ChannelAllot();
                                addChannelAllot.SortBatchId = sortBatchId;
                                addChannelAllot.ChannelCode = channelCode;
                                string productName = productCode == null ? null : beAllotProducts.Where(p => p.ProductCode == productCode).Select(p => p.ProductName).FirstOrDefault();
                                addChannelAllot.ProductCode = productCode;
                                addChannelAllot.ProductName = productName;
                                addChannelAllot.Quantity = quantity;
                                ChannelAllotRepository.Add(addChannelAllot);

                            }
                        }
                    }
                }
                ChannelAllotRepository.SaveChanges();
            }
        }
    }
}
