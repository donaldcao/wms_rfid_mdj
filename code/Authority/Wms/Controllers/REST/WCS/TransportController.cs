using System;
using System.Web.Mvc;
using Microsoft.Practices.Unity;
using THOK.WCS.REST.Interfaces;
using THOK.WCS.REST.Models;

namespace Wms.Controllers.REST.WCS
{
    public class TransportController : Controller
    {
        [Dependency]
        public ITransportService TransportService { get; set; }

        public ActionResult GetSrmTask(string name, int travelPos, int liftPos)
        {
            string errorInfo = string.Empty;
            var result = TransportService.GetSrmTask(name, travelPos, liftPos, out errorInfo);
            return Json(new RestResult { IsSuccess = result != null, Message = errorInfo, Data = result }, "application/json", JsonRequestBehavior.AllowGet);
        }

        public ActionResult CancelTask(int taskid)
        {
            string errorInfo = string.Empty;
            bool bResult = TransportService.CancelTask(taskid, out errorInfo);
            return Json(new RestResult { IsSuccess = bResult, Message = errorInfo }, "application/json", JsonRequestBehavior.AllowGet);
        }

        public ActionResult FinishTask(int taskid, string operatorName)
        {
            string errorInfo = string.Empty;
            bool bResult = TransportService.FinishTask(taskid, operatorName, out errorInfo);
            return Json(new RestResult { IsSuccess = bResult, Message = errorInfo }, "application/json", JsonRequestBehavior.AllowGet);
        }
    }
}
