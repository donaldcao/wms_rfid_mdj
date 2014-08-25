using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using THOK.SMS.Bll.Service;
using THOK.SMS.Bll.Models;

namespace THOK.SMS.Bll.Interfaces
{
    public interface ISortTaskService
    {
        bool CreateNewSupplyTask(int supplyCachePositionNo, int vacancyQuantity, DateTime orderdate, int batchNO, out string errorInfo);
    }
}
