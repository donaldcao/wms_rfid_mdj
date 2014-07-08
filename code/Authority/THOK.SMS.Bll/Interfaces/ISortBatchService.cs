using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using THOK.SMS.DbModel;

namespace THOK.SMS.Bll.Interfaces
{
    public interface ISortBatchService : IService<SortBatch>
    {
        object GetDetails(int page, int rows, SortBatch sortBatch, string sortingLineName, string sortingLineType);

        bool Add(string dispatchId, out string strResult);
        bool Save(SortBatch SortBatch, out string strResult);
        bool Delete(int SortBatchId, out string strResult);
        System.Data.DataTable GetSortBatch(int page, int rows, string SortBatchId);
    }
}
