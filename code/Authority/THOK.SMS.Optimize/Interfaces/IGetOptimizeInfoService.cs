using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using THOK.SMS.DbModel;
using THOK.Wms.DbModel;

namespace THOK.SMS.Optimize.Interfaces
{
    public interface IGetOptimizeInfoService : IService<SortOrder>
    {
        double GetChannelAllotScale();

        bool GetIsUseWholePieceSortingLine();

        SortBatch GetSortBatch(int sortBatchId);

        SortingLine GetSortingLine(string sortingLineCode);

        Channel[] GetChannel(string sortingLineCode);

        string[] GetDeliverLine(int sortBatchId, string productType);

        SortOrder[] GetSortOrder(string orderDate, string[] deliverLineCodes);

        SortOrderDetail[] GetSortOrderDetail(SortOrder[] sortOrders, string productType, bool isUseWholePieceSortingLine);

        ChannelAllot[] GetChannelAllot(int sortBatchId);

        SortOrderAllotMaster[] GetSortOrderAllotMaster(int sortBatchId);
    }
}
