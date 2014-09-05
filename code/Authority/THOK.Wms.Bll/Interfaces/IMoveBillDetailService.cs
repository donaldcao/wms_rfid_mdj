﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using THOK.Wms.DbModel;

namespace THOK.Wms.Bll.Interfaces
{
    public interface IMoveBillDetailService :IService<MoveBillDetail>
    {
        object GetDetails(int page, int rows, string BillNo);

        bool Add(MoveBillDetail moveBillDetail, out string strResult);

        bool Delete(string ID, out string strResult);

        bool Save(MoveBillDetail moveBillDetail, out string strResult);

        System.Data.DataTable GetDetailsTable(int page, int rows, string billNo);

        System.Data.DataTable GetMoveBillDetail(int page, int rows, string BillNo, bool isAbnormity, bool isGroup, out string sortingName);

        object GetMoveBillMaster();

        object SearchMoveBillDetail(string billNo, int page, int rows);

        bool EditAllot(string id, string status, string operater, out string strResult);

        System.Data.DataTable MoveBillDetailTable(string billNo, int page, int rows);
    }
}
