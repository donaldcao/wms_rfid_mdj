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
    public class ChannelAllotController : Controller
    {
        [Dependency]
        public IChannelAllotService ChannelAllotServer { get; set; }
        //
        // GET: /ChannelAllot/

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
            ChannelAllot channelAllot = new ChannelAllot();

            channelAllot.ChannelAllotCode = collection["ChannelAllotCode"] ?? "";
            channelAllot.ChannelCode = collection["ChannelCode"] ?? "";
            channelAllot.ProductCode = collection["ProductCode"] ?? "";
            channelAllot.ProductName = collection["ProductName"] ?? "";

            string BatchSortId = collection["BatchSortId"] ?? "";
            if (BatchSortId != "" && BatchSortId != null)
            {
                channelAllot.BatchSortId = Convert.ToInt32(BatchSortId);
            }

            string InQuantity = collection["InQuantity"] ?? "";
            if (InQuantity != "" && InQuantity != null)
            {
                channelAllot.InQuantity = Convert.ToInt32(InQuantity);
            }

            string OutQuantity = collection["OutQuantity"] ?? "";
            if (OutQuantity != "" && OutQuantity != null)
            {
                channelAllot.OutQuantity = Convert.ToInt32(OutQuantity);
            }

            string RealQuantity = collection["RealQuantity"] ?? "";
            if (RealQuantity != "" && RealQuantity != null)
            {
                channelAllot.RealQuantity = Convert.ToInt32(RealQuantity);
            }

            string RemainQuantity = collection["RemainQuantity"] ?? "";
            if (RemainQuantity != "" && RemainQuantity != null)
            {
                channelAllot.RemainQuantity = Convert.ToInt32(RemainQuantity);
            }

            var channelAllotDetail = ChannelAllotServer.GetDetails(page, rows, channelAllot);
            return Json(channelAllotDetail, "text", JsonRequestBehavior.AllowGet);

        }

        public ActionResult SearchPage()
        {
            return View();
        }

        //打印
        public FileStreamResult CreateExcelToClient()
        {
            int page = 0, rows = 0;

            string ChannelAllotCode = Request.QueryString["ChannelAllotCode"] ?? "";
            int BatchSortId = Convert.ToInt32(Request.QueryString["BatchSortId"] ?? "");
            string ChannelCode = Request.QueryString["ChannelCode"] ?? "";
            string ProductCode = Request.QueryString["ProductCode"] ?? "";
            string ProductName = Request.QueryString["ProductName"] ?? "";
            int InQuantity = Convert.ToInt32(Request.QueryString["InQuantity"] ?? "");
            int OutQuantity = Convert.ToInt32(Request.QueryString["OutQuantity"] ?? "");
            int RealQuantity = Convert.ToInt32(Request.QueryString["RealQuantity"] ?? "");
            int RemainQuantity = Convert.ToInt32(Request.QueryString["RemainQuantity"] ?? "");


            ChannelAllot channelAllot = new ChannelAllot();
            channelAllot.ChannelAllotCode = ChannelAllotCode;
            channelAllot.BatchSortId = BatchSortId;
            channelAllot.ChannelCode = ChannelCode;
            channelAllot.ProductCode = ProductCode;
            channelAllot.ProductName = ProductName;
            channelAllot.InQuantity = InQuantity;
            channelAllot.OutQuantity = OutQuantity;
            channelAllot.RealQuantity = RealQuantity;
            channelAllot.RemainQuantity = RemainQuantity;

            ExportParam ep = new ExportParam();
            ep.DT1 = ChannelAllotServer.GetChannelAllot(page, rows, channelAllot);
            ep.HeadTitle1 = "烟道分配";
            return PrintService.Print(ep);
        }
    }
}
