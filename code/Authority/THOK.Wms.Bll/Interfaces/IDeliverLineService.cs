using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using THOK.Wms.DbModel;

namespace THOK.Wms.Bll.Interfaces
{
    public interface IDeliverLineService : IService<DeliverLine>
    {
        object GetDetails(int page, int rows, string DeliverLineCode, string CustomCode, string DeliverLineName, string DistCode, string DeliverOrder, string IsActive);

        bool Add(DeliverLine deliverLine, out string strResult);

        object D_Details(int page, int rows, string QueryString, string Value);

        bool Delete(string DeliverLineCode);

        bool Edit(string DeliverLineCode,string DeliverOrder, out string strResult);

        System.Data.DataTable GetDeliverLineInfo(int page, int rows, string DeliverLineCode, string DistCode, string CustomCode, string IsActive, string DeliverLinename);

        object C_Details(int page, int rows, string QueryString, string Value);
    }
}
