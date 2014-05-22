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

         bool OptimizeAllot(int batchNo, out string strResult);
    }
}
