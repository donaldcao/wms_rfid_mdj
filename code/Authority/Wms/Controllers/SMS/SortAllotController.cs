using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.Practices.Unity;
using THOK.SMS.Optimize.Interfaces;

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
    }
}
