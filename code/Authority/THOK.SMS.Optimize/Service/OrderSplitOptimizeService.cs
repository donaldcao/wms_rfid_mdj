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
using THOK.SMS.Optimize.Model;

namespace THOK.SMS.Optimize.Service
{
    public class OrderSplitOptimizeService : ServiceBase<SortOrder>, IOrderSplitOptimizeService
    {
        [Dependency]
        public ISortOrderAllotMasterRepository SortOrderAllotMasterRepository { get; set; }

        [Dependency]
        public ISortOrderAllotDetailRepository SortOrderAllotDetailRepository { get; set; }

        protected override Type LogPrefix
        {
            get { return this.GetType(); }
        }

        public void OrderSplitOptimize(int sortBatchId, string[] deliverLineCodes, SortOrder[] sortOrders, SortOrderDetail[] sortOrderDetails)
        {
            int packNo = 0;
            int customerOrder = 0;

            foreach (var deliverLineCode in deliverLineCodes)
            {
                int customerDeliverOrder = 0;
                var sortOrdersArray = sortOrders.Where(s => s.DeliverLineCode == deliverLineCode)
                                                .OrderBy(s => s.DeliverOrder)
                                                .ToArray();

                foreach (var sortOrder in sortOrdersArray)
                {
                    var sortOrderDetailArray = sortOrderDetails.Where(s => s.OrderID == sortOrder.OrderID)
                                                               .OrderByDescending(s => s.SortQuantity)
                                                               .ToArray();

                    int orderQuantity = Convert.ToInt32(sortOrderDetailArray.Sum(c => c.SortQuantity));
                    if (orderQuantity == 0)
                    {
                        continue;
                    }
                    Dictionary<int, int> BagQuantity = new Dictionary<int, int>();

                    int splitQuantity = 25;
                    int bagCount = orderQuantity / splitQuantity;
                    if (orderQuantity % splitQuantity > 0 && orderQuantity % splitQuantity < 5 && orderQuantity > splitQuantity)
                    {
                        for (int i = 1; i <= bagCount - 1; i++)
                        {
                            BagQuantity.Add(++packNo, splitQuantity);
                        }
                        BagQuantity.Add(++packNo, splitQuantity - 5);
                        BagQuantity.Add(++packNo, orderQuantity % splitQuantity + 5);
                    }
                    else
                    {
                        for (int i = 1; i <= bagCount; i++)
                        {
                            BagQuantity.Add(++packNo, splitQuantity);
                        }
                        if (orderQuantity % splitQuantity > 0)
                        {
                            BagQuantity.Add(++packNo, orderQuantity % splitQuantity);
                        }
                    }

                    customerOrder += 1;
                    customerDeliverOrder += 1;

                    //往分拣订单分配主表添加数据
                    foreach (var item in BagQuantity)
                    {
                        SortOrderAllotMaster addSortOrderAllotMaster = new SortOrderAllotMaster();
                        addSortOrderAllotMaster.SortBatchId = sortBatchId;
                        addSortOrderAllotMaster.OrderId = sortOrder.OrderID;
                        addSortOrderAllotMaster.PackNo = item.Key;
                        addSortOrderAllotMaster.Quantity = item.Value;
                        addSortOrderAllotMaster.CustomerCode = sortOrder.CustomerCode;
                        addSortOrderAllotMaster.CustomerName = sortOrder.CustomerName;
                        addSortOrderAllotMaster.DeliverLineCode = sortOrder.DeliverLineCode;
                        addSortOrderAllotMaster.CustomerInfo = "";
                        addSortOrderAllotMaster.CustomerOrder = customerOrder;
                        addSortOrderAllotMaster.CustomerDeliverOrder = customerDeliverOrder;
                        addSortOrderAllotMaster.ExportNo = 0;
                        addSortOrderAllotMaster.StartTime = DateTime.Now;
                        addSortOrderAllotMaster.FinishTime = DateTime.Now;
                        addSortOrderAllotMaster.Status = "01";
                        SortOrderAllotMasterRepository.Add(addSortOrderAllotMaster);
                    }
                }
            }
            SortOrderAllotMasterRepository.SaveChanges();
        }

        public void OrderDetailSplitOptimize(int sortBatchId, string[] deliverLineCodes, SortOrder[] sortOrders, SortOrderDetail[] sortOrderDetails, ChannelAllot[] channelAllots, SortOrderAllotMaster[] sortOrderAllots)
        {
            var channelGroupInfos = channelAllots.Select(c => c.channel.GroupNo)
                                                 .Distinct()
                                                 .Select(g => new ChannelGroupInfo { ChannelGroup = g, Quantity = 0 })
                                                 .ToArray();

            var channelAllotInfos = channelAllots.Select(c => new ChannelAllotInfo
            {
                Id = c.Id,
                ChannelCode = c.ChannelCode,
                GroupNo = c.channel.GroupNo,
                OrderNo = c.channel.OrderNo,
                ProductCode = c.ProductCode,
                Quantity = ((c.Quantity - c.channel.RemainQuantity) / 50) * 50,
                RemainQuantity = c.channel.RemainQuantity + ((c.Quantity - c.channel.RemainQuantity) % 50),
                ChannelCapacity = c.channel.ChannelCapacity
            })
                .ToArray();

            foreach (var deliverLineCode in deliverLineCodes)
            {
                var sortOrdersArray = sortOrders.Where(s => s.DeliverLineCode == deliverLineCode)
                                                .OrderBy(s => s.DeliverOrder)
                                                .ToArray();

                foreach (var sortOrder in sortOrdersArray)
                {
                    var packInfos = sortOrderAllots.Where(s => s.OrderId == sortOrder.OrderID)
                                                   .OrderBy(s => s.PackNo)
                                                   .Select(s => new PackInfo { Id = s.Id, PackNo = s.PackNo, Quantity = s.Quantity, SortOrderAllot = s })
                                                   .ToArray();

                    var sortOrderDetailInfos = sortOrderDetails.Where(s => s.OrderID == sortOrder.OrderID && s.SortQuantity > 0)
                        .OrderByDescending(s => s.SortQuantity)
                        .Select(s => new SortOrderDetailInfo
                        {
                            ProductCode = s.ProductCode,
                            ProductName = s.ProductName,
                            Quantity = s.SortQuantity
                        })
                        .ToArray();

                    //主单对应细单数据分配优化
                    foreach (var packInfo in packInfos)
                    {
                        foreach (var channelGroupInfo in channelGroupInfos)
                        {
                            if (packInfo.Quantity == 0)
                                break;

                            var tmp1 = sortOrderDetailInfos.Where(s => s.Quantity > 0
                                    && (channelAllotInfos.Where(c => c.GroupNo == channelGroupInfo.ChannelGroup)).Any(t => t.ProductCode == s.ProductCode))
                                .ToArray();

                            foreach (var sortOrderDetailInfo in tmp1)
                            {
                                if (packInfo.Quantity == 0)
                                    break;

                                var tmp2 = channelAllotInfos.Where(c => c.ProductCode == sortOrderDetailInfo.ProductCode
                                                                && c.GroupNo == channelGroupInfo.ChannelGroup)
                                                            .OrderBy(c => c.OrderNo)
                                                            .ToArray();

                                while (packInfo.Quantity > 0 && sortOrderDetailInfo.Quantity > 0 && tmp2.Sum(t => t.Quantity + t.RemainQuantity) > 0)
                                {
                                    foreach (var channelAllotInfo in tmp2)
                                    {
                                        if (packInfo.Quantity == 0 || sortOrderDetailInfo.Quantity == 0)
                                            break;

                                        decimal allotQuantity = 0;

                                        if (channelAllotInfo.Quantity >= 50)
                                        {
                                            allotQuantity = sortOrderDetailInfo.Quantity < (channelAllotInfo.RemainQuantity - (channelAllotInfo.ChannelCapacity - 50)) ? sortOrderDetailInfo.Quantity : (channelAllotInfo.RemainQuantity - (channelAllotInfo.ChannelCapacity - 50));
                                        }
                                        else
                                        {
                                            allotQuantity = sortOrderDetailInfo.Quantity < channelAllotInfo.RemainQuantity ? sortOrderDetailInfo.Quantity : channelAllotInfo.RemainQuantity;
                                        }

                                        allotQuantity = packInfo.Quantity < allotQuantity ? packInfo.Quantity : allotQuantity;

                                        if (allotQuantity > 0 && channelAllotInfo.OrderNo == tmp2.Where(t => t.Quantity + t.RemainQuantity > 0).Min(t => t.OrderNo))
                                        {
                                            packInfo.Quantity -= (int)allotQuantity;
                                            sortOrderDetailInfo.Quantity -= allotQuantity;
                                            channelAllotInfo.RemainQuantity -= (int)allotQuantity;
                                            channelGroupInfo.Quantity += (int)allotQuantity;

                                            SortOrderAllotDetail addSortOrderAllotDetail = new SortOrderAllotDetail();
                                            addSortOrderAllotDetail.MasterId = packInfo.Id;
                                            addSortOrderAllotDetail.ChannelCode = channelAllotInfo.ChannelCode;
                                            addSortOrderAllotDetail.ProductCode = sortOrderDetailInfo.ProductCode;
                                            addSortOrderAllotDetail.ProductName = sortOrderDetailInfo.ProductName;
                                            addSortOrderAllotDetail.Quantity = (int)allotQuantity;
                                            //SortOrderAllotDetailRepository.Add(addSortOrderAllotDetail);
                                            packInfo.SortOrderAllot.SortOrderAllotDetails.Add(addSortOrderAllotDetail);
                                        }

                                        if (channelAllotInfo.RemainQuantity <= (channelAllotInfo.ChannelCapacity - 50) && channelAllotInfo.Quantity >= 50)
                                        {
                                            channelAllotInfo.RemainQuantity += 50;
                                            channelAllotInfo.Quantity -= 50;
                                            channelAllotInfo.OrderNo = tmp2.Max(t => t.OrderNo) + 1;
                                        }
                                    }
                                }
                            }
                        }

                        if (packInfo.Quantity > 0)
                        {
                            throw new Exception("优化出现错误，烟包分配细单时出错，有数量未分配！");
                        }
                    }
                }
            }
            SortOrderAllotDetailRepository.SaveChanges();
        }
    }
}
