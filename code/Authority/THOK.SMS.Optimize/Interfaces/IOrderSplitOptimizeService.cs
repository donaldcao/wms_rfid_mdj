using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using THOK.SMS.DbModel;
using THOK.Wms.DbModel;

namespace THOK.SMS.Optimize.Interfaces
{
    public interface IOrderSplitOptimizeService : IService<SortOrder>
    {
        void OrderSplitOptimize(int sortBatchId, string[] deliverLineCodes, SortOrder[] sortOrders, SortOrderDetail[] sortOrderDetails);

        void OrderDetailSplitOptimize(int sortBatchId, string[] deliverLineCodes, SortOrder[] sortOrders, SortOrderDetail[] sortOrderDetails, ChannelAllot[] channelAllots, SortOrderAllotMaster[] sortOrderAllots);
    }
}
