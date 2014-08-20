using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using THOK.SMS.DbModel;

namespace THOK.SMS.Bll.Interfaces
{
    public interface ISupplyPositionService
    {
        object GetDetails(int page, int rows, SupplyPosition supplyPosition);
        bool Add(SupplyPosition supplyPosition, out string strResult);
        bool Save(SupplyPosition supplyPosition, out string strResult);
        bool Delete(int id, out string strResult);
    }
}
