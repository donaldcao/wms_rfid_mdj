using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using THOK.SMS.DbModel;

namespace THOK.SMS.Bll.Interfaces
{
    public interface ISortBatchService : IService<SortBatch>
    {
        object GetDetails(int page, int rows, SortBatch sortBatch, string sortingLineName);

        bool Add(string dispatchId, out string strResult);

        bool Edit(SortBatch sortBatch, string IsRemoveOptimization, out string strResult);

        bool Delete(string id, out string strResult);

        System.Data.DataTable GetSortBatch(int page, int rows, string SortBatchId);
    }
}
