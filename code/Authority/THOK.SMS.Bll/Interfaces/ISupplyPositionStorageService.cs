using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using THOK.SMS.DbModel;

namespace THOK.SMS.Bll.Interfaces
{
    public interface ISupplyPositionStorageService
    {
        object GetDetails(int page, int rows, SupplyPositionStorage entity);
        bool Add(SupplyPositionStorage entity, out string strResult);
        bool Save(SupplyPositionStorage entity, out string strResult);
        bool Delete(int id, out string strResult);
        DataTable GetTable(int page, int rows, SupplyPosition entity);
    }
}
