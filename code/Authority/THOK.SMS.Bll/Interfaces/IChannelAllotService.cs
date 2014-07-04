using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using THOK.SMS.DbModel;

namespace THOK.SMS.Bll.Interfaces
{
    public interface IChannelAllotService : IService<ChannelAllot>
    {
        System.Data.DataTable GetChannelAllot(int page, int rows, string orderDate, string batchNo, string sortingLineCode, string productCode,string text);

        object GetDetails(int page, int rows, string orderDate, string batchNo, string sortingLineCode, string productCode);

        object Details(int page, int rows, string orderDate, string batchNo, string sortingLineCode, string productCode);

    }
}
