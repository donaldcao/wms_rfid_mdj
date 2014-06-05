using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using THOK.SMS.DbModel;
using THOK.Wms.DbModel;

namespace THOK.SMS.Optimize.Interfaces
{
    public interface ISortOrderDownService : IService<SortOrder>
    {
        bool IsWarehousSortIntegration(out string strResult);
        bool DownSortOrder(string beginDate, string endDate);
        bool DownLoad(string beginDate, string endDate, out string strResult);
    }
}