using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using THOK.Wms.DbModel;

namespace THOK.Wms.Bll.Interfaces
{
    public interface ISortOrderDispatchService : IService<SortOrderDispatch>
    {
        object GetDetails(int page, int rows, string OrderDate, string WorkStatus, string SortStatus, string SortingLineCode);

        object GetWorkStatus();

        object GetBatchStatus();

        object GetNormalBatch();

        object GetAbnormalBatch();

        object GetPieceBatch();

        object GetManualBatch();

        int GetActiveSortingLine(string productType);

        bool Add(string DeliverLineCodes, string orderDate);

        bool Add(string SortingLineCode, string DeliverLineCodes,string orderDate);

        bool Delete(string id,out string errorInfo);

        bool Save(SortOrderDispatch sortDispatch);
        bool Edit(string id, string SortingLineCode);

        System.Data.DataTable GetSortOrderDispatch(int page, int rows, string OrderDate, string WorkStatus, string SortStatus, string SortingLineCode);
    }
}
