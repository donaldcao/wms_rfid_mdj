using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using THOK.SMS.Bll.Models;
using System.Web.Script.Serialization;
using Microsoft.Practices.Unity;
using THOK.SMS.Bll.Interfaces;

namespace Wms.Controllers.SMS.SortManage
{
    public class SortTaskController : Controller
    {
        [Dependency]
        public ISortTaskService SortTaskService { get; set; }

        public ActionResult CreateNewSupplyTask(int supplyCachePositionNo,int vacancyQuantity,DateTime orderdate,int batchNO)
        {
            string errorInfo = string.Empty;
            bool bResult = SortTaskService.CreateNewSupplyTask(supplyCachePositionNo, vacancyQuantity, orderdate, batchNO, out errorInfo);
            return Json(new Result { IsSuccess = bResult, Message = errorInfo }, "application/json", JsonRequestBehavior.AllowGet);
        }
    }
}
