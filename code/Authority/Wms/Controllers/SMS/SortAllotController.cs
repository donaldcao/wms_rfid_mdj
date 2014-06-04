using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.Practices.Unity;
using THOK.SMS.Optimize.Interfaces;
using THOK.Wms.DbModel;
using THOK.Common.WebUtil;

namespace Wms.Controllers.SMS
{
    public class SortAllotController : Controller
    {
        [Dependency]
        public IDeliverLineOptimizeService DeliverLineOptimizeService { get; set; }
        //
        // GET: /SortAllot/

        public ActionResult Index(string moduleID)
        {
            ViewBag.hasSearch = true;
            ViewBag.hasAllot = true;
            ViewBag.hasAdd = true;
            ViewBag.hasEdit = true;
            ViewBag.hasDelete = true;
            ViewBag.hasPrint = true;
            ViewBag.hasHelp = true;
            ViewBag.ModuleID = moduleID;
            return View();
        }

        public ActionResult OrderInformation()
        {
            var orderInfo = DeliverLineOptimizeService.GetOrderInfo();
            return Json(orderInfo, "text", JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetDeliverLine(int page, int rows, string orderDate)
        {
            if (orderDate == null)
            {
                return null;
            }
            var deliverLineDetail = DeliverLineOptimizeService.GetDeliverLine(orderDate);
            return Json(deliverLineDetail, "text", JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetUnAllotDeliverLine(int page, int rows, string orderDate)
        {
            if (orderDate == null)
            {
                return null;
            }
            var unAllotDeliverLine = DeliverLineOptimizeService.GetUnAllotDeliverLine(orderDate);
            return Json(unAllotDeliverLine, "text", JsonRequestBehavior.AllowGet);
        }

        public ActionResult EditDeliverLine(DeliverLine deliverLine)
        {
            string strResult = string.Empty;
            bool bResult = DeliverLineOptimizeService.EditDeliverLine(deliverLine,out strResult);
            string msg = bResult ? "修改成功" : "修改失败";
            return Json(JsonMessageHelper.getJsonMessage(bResult, msg, strResult), "text", JsonRequestBehavior.AllowGet);
        }

        public ActionResult UpdateDeliverLineAllot(string orderDate, string deliverLineCodes)
        {
            string strResult = string.Empty;
            bool bResult = DeliverLineOptimizeService.UpdateDeliverLineAllot(orderDate, deliverLineCodes, this.User.Identity.Name.ToString(), out strResult);
            string msg=bResult?"分配成功":"分配失败";
            return Json(JsonMessageHelper.getJsonMessage(bResult,msg,strResult), "text", JsonRequestBehavior.AllowGet);
        }
    }
}
