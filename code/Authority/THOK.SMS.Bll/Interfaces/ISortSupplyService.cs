﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using THOK.SMS.DbModel;

namespace THOK.SMS.Bll.Interfaces
{
    public interface ISortSupplyService : IService<SortSupply>
    {
        System.Data.DataTable GetSortSupply(int page, int rows, SortSupply sortSupply);

        object GetDetails(int page, int rows, SortSupply sortSupply);
    }
}
