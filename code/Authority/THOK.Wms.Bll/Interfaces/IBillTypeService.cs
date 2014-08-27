using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using THOK.Wms.DbModel;

namespace THOK.Wms.Bll.Interfaces
{
    public interface IBillTypeService : IService<BillType>
    {
        object GetDetails(int page, int rows, string billClass, string isActive);

        bool Add(BillType billtype);

        bool Delete(string billtypeCode);

        bool Save(BillType billtype);

        DataTable BillTypeTable(int page, int rows, string billClass, string isActive);
    }
}
