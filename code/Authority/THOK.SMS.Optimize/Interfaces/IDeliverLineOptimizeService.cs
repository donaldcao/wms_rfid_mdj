using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using THOK.SMS.DbModel;
using THOK.Wms.DbModel;

namespace THOK.SMS.Optimize.Interfaces
{
    public interface IDeliverLineOptimizeService : IService<SortOrder>
    {
        object GetOrderInfo();

        object GetDeliverLine(string orderDate);

        object GetUnAllotDeliverLine(string orderDate);

        bool OptimizeAllot(int batchNo, out string strResult);

        bool EditDeliverLine(DeliverLine deliverLine,out string strResult);

        bool UpdateDeliverLineAllot(string orderDate, string deliverLineCodes,string userName,out string strResult);

        bool UpdateDeliverLineAllot(string orderDate, string deliverLineCodes,string sortingLineCode, string userName, out string strResult);
    }
}
