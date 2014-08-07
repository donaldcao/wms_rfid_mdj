﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using THOK.Wms.DbModel;

namespace THOK.Wms.Bll.Interfaces
{
    public interface ISortOrderSearchService : IService<SortOrder>
    {
        object GetDetails(int page, int rows, string orderID, string orderDate, string customerCode, string customerName, string deliverLineCode);

        System.Data.DataTable GetSortOrderSearchInfo(int page, int rows, string orderID, string orderDate, string customerCode, string customerName, string deliverLineCode);
    }
}
