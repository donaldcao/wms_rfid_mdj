using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using THOK.SMS.DbModel;

namespace THOK.SMS.Bll.Interfaces
{
    public interface IHandSupplyService : IService<HandSupply>
    {
        object GetDetails(int page, int rows, HandSupply sortSupply);

        System.Data.DataTable GetSortSupply(int page, int rows, HandSupply sortSupply);
    }
}
