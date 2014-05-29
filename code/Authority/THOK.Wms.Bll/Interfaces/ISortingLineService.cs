﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using THOK.Wms.DbModel;

namespace THOK.Wms.Bll.Interfaces
{
    public interface ISortingLineService : IService<SortingLine>
    {
       object GetDetails(int page, int rows, string sortingLineCode, string sortingLineName, string SortingLineType, string IsActive);

       object GetDetailsForSort(int page, int rows);

        bool Add(SortingLine sortingLine);

        bool Delete(string sortingLineCode);

        bool Save(SortingLine sortingLine);

        object GetSortLine();

        System.Data.DataTable GetSortingLine(int page, int rows, string sortingLineCode);
    }
}
