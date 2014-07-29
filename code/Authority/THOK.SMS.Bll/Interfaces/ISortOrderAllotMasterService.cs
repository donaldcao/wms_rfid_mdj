﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using THOK.SMS.DbModel;
using System.Data;

namespace THOK.SMS.Bll.Interfaces
{
    public interface ISortOrderAllotMasterService : IService<SortOrderAllotMaster>
    {
        DataTable GetSortOrderAllotMaster(int page,int rows,string batchNo, string orderId, string status);

        object GetDetails(int page, int rows, string orderDate, string batchNo, string sortingLineCode, string deliverLineCode, string customerCode, string status);
    }
}