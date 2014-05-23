using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using THOK.SMS.DbModel;
using Microsoft.Practices.Unity;
using THOK.SMS.Bll.Interfaces;
using THOK.Common.WebUtil;
using THOK.Security;
using THOK.Common.NPOI.Models;
using THOK.Common.NPOI.Service;

namespace Wms.Controllers.SMS
{
    public class DeliverAllotController : Controller
    {
        [Dependency]
        public IDeliverLineAllotService DeliverLineAllotServer { get; set; }
        //
        // GET: /DeliverLineAllot/

        public ActionResult Index(string moduleID)
        {
            ViewBag.hasSearch = true;
            ViewBag.hasPrint = true;
            ViewBag.hasHelp = true;
            ViewBag.ModuleID = moduleID;
            return View();
        }
        public ActionResult Details(int page, int rows, FormCollection collection)
        {
            DeliverLineAllot deliverLineAllot = new DeliverLineAllot();
            deliverLineAllot.DeliverLineAllotCode = collection["DeliverLineAllotCode"] ?? "";
            deliverLineAllot.DeliverLineCode = collection["DeliverLineCode"] ?? "";
            deliverLineAllot.Status = collection["Status"] ?? "";
            string BatchSortId=collection["BatchSortId"]??"";
            if (BatchSortId != "" && BatchSortId != null)
            {
                deliverLineAllot.BatchSortId = Convert.ToInt32(BatchSortId);
            }
            var deliverLineAllotDetail = DeliverLineAllotServer.GetDetails(page, rows, deliverLineAllot);
            return Json(deliverLineAllotDetail, "text", JsonRequestBehavior.AllowGet);
        }
        public ActionResult SearchPage()
        {
            return View();
        }

        //打印
        public FileStreamResult CreateExcelToClient()
        {
            int page=0, rows=0;

            string DeliverLineAllotCode = Request.QueryString["DeliverLineAllotCode"] ?? "";
            int BatchSortId =Convert.ToInt32(Request.QueryString["BatchSortId"] ?? "");
            string DeliverLineCode = Request.QueryString["DeliverLineCode"] ?? "";
            int DeliverQuantity = Convert.ToInt32(Request.QueryString["DeliverQuantity"] ?? "");
            string Status = Request.QueryString["Status"] ?? "";

            DeliverLineAllot deliverLineAllot = new DeliverLineAllot();
            deliverLineAllot.DeliverLineAllotCode = DeliverLineAllotCode;
            deliverLineAllot.BatchSortId = BatchSortId;
            deliverLineAllot.DeliverLineCode = DeliverLineCode;
            deliverLineAllot.DeliverQuantity = DeliverQuantity;
            deliverLineAllot.Status = Status;

            ExportParam ep = new ExportParam();
            ep.DT1 = DeliverLineAllotServer.GetDeliverLineAllot(page, rows, deliverLineAllot);
            ep.HeadTitle1 = "线路分配";
            return PrintService.Print(ep);
        }
    }
}
