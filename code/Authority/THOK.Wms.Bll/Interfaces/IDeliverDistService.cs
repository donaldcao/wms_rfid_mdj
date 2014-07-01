using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using THOK.Wms.DbModel;

namespace THOK.Wms.Bll.Interfaces
{
    public interface IDeliverDistService : IService<DeliverDist>
    {
        object GetDetails(int page, int rows, string DistCode, string CustomCode, string DistName, string IsActive); // string CompanyCode, string UniformCode,  

       bool Add(DeliverDist deliverDist, out string strResult);

       object S_Details(int page, int rows, string QueryString, string Value);

       bool Save(string DistCode,string DeliverOrder, out string strResult);

       bool Delete(string DistCode);

       System.Data.DataTable GetDeliverDistInfo(int page, int rows, string DistCode);
    }
}
