using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using THOK.SMS.DbModel;
using THOK.Wms.DbModel;

namespace THOK.SMS.Optimize.Interfaces
{
    public interface IOrderOptimizeService : IService<SortOrder>
    {
        bool OrderAllotOptimize(string orderDate ,out string strResult);
    }
}
