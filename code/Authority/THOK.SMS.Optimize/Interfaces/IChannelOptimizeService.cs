using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using THOK.SMS.DbModel;
using THOK.Wms.DbModel;

namespace THOK.SMS.Optimize.Interfaces
{
    public interface IChannelOptimizeService : IService<SortOrder>
    {
        bool ChannelAllot(string orderDate ,out string strResult);

        object GetBatchSort(string orderDate);
    }
}
