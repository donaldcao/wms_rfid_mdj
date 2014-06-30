using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using THOK.SMS.DbModel;

namespace THOK.SMS.Bll.Interfaces
{
    public interface ISortBatchService : IService<SortBatch>
    {
        //object GetDetails(int page, int rows, SortBatch SortBatch);


        object GetDetails(int page, int rows, string Status, string BatchNo, string BatchName, string OrderDate);

        object GetBatch(int page, int rows, string queryString, string value);

        bool Add(SortBatch SortBatch, out string strResult);
        bool Save(SortBatch SortBatch, out string strResult);
        bool Delete(int SortBatchId, out string strResult);
        System.Data.DataTable GetSortBatch(int page, int rows, string SortBatchId);
    }
}
