using System;
using System.Web.Mvc;
using THOK.SMS.Bll.Models;
using Microsoft.Practices.Unity;
using THOK.SMS.REST.Interfaces;

namespace Wms.Controllers.REST.SMS
{
    public class SupplyController : Controller
    {
        [Dependency]
        public ISupplyService SupplyService { get; set; }

        public ActionResult CreateSupplyTask(int position, int quantity, DateTime orderdate, int batchNo)
        {
            string errorInfo = string.Empty;
            bool bResult = SupplyService.CreateSupplyTask(position, quantity, orderdate, batchNo, out errorInfo);
            return Json(new Result { IsSuccess = bResult, Message = errorInfo }, "application/json", JsonRequestBehavior.AllowGet);
        }

        public ActionResult AssignTaskOriginPositionAddress()
        {
            string errorInfo = string.Empty;
            bool bResult = SupplyService.AssignTaskOriginPositionAddress(out errorInfo);
            return Json(new Result { IsSuccess = bResult, Message = errorInfo }, "application/json", JsonRequestBehavior.AllowGet);
        }

        public ActionResult CheckSupplyPositionStorage()
        {
            string errorInfo = string.Empty;
            bool bResult = SupplyService.CheckSupplyPositionStorage(out errorInfo);
            return Json(new Result { IsSuccess = bResult, Message = errorInfo }, "application/json", JsonRequestBehavior.AllowGet);
        }
    }
}
