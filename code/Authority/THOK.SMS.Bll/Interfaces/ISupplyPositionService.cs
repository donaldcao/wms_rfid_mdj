using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using THOK.SMS.DbModel;

namespace THOK.SMS.Bll.Interfaces
{
    public interface ISupplyPositionService
    {
        object GetDetails(int page, int rows, SupplyPosition entity);
        object GetDetails(int page, int rows, string QueryString, string Value);
        bool Add(SupplyPosition entity, out string strResult);
        bool Save(SupplyPosition entity, out string strResult);
        bool Delete(int id, out string strResult);
        DataTable GetTable(int page, int rows, SupplyPosition entity);
    }
}
